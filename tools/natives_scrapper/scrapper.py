import os
import json
import requests

from pathlib import Path
from typing import Dict, List, Optional

from bs4 import BeautifulSoup

from models import NativeFunction, NativeParameter, NativeType


BASE_PART = "https://gtamods.com"
BASE_URL = f"{BASE_PART}/wiki/List_of_native_functions_(GTA_IV)"
NATIVE_DATA_PATH = Path(__file__).parent.parent / "native_data"


class NativeNotFoundException(Exception): pass


def get_soup(url: Optional[str] = None) -> BeautifulSoup:
    effective_url = url or BASE_URL
    response = requests.get(effective_url)
    if not response.ok:
        raise Exception(f"Failed to fetch Natives from Wiki url: {effective_url}")

    return BeautifulSoup(response.text, "html.parser")


def parse_main_list(soup: BeautifulSoup) -> Dict[str, str]:
    """
    Returns a dict of native names and their urls to be parsed
    """
    def _is_valid(a) -> bool:
        href = (a or {}).get("href")
        if not href or not isinstance(href, str):
            print("Invalid 'a' tag:", a.text)
            return False
        return (
            href.startswith("/wiki")
            and a.text
            and a.text.strip() == a.text.strip().upper()
            and " " not in a.text
        )
        
    a_tags = soup.select("ul > li > a") or []
    
    a_tags = [a for a in a_tags if _is_valid(a)]
    if not a_tags:
        raise Exception("Could not find any list of natives");

    return {a.text.strip(): f"{BASE_PART}{a.get("href").strip()}" for a in a_tags}


def parse_native_function(soup: BeautifulSoup, name: str) -> NativeFunction:
    desc_part = soup.find("p")
    description = desc_part.text if desc_part else None
    
    table_rows = soup.select("table tr")
    param_count_row = table_rows.pop(0)
    param_count = int(param_count_row.text.split(":")[-1].strip())

    # POP table header
    table_rows.pop(0)
    
    return_type_row = table_rows.pop()
    rtype, rdesc = return_type_row.findAll("td")

    return_type = NativeType.from_string(rtype.text)
    return_type_description = rdesc.text.strip()

    params: List[NativeParameter | None] | None = None
    if not param_count:
        return NativeFunction(
            function_name=name,
            return_type=return_type,
            return_type_description=return_type_description,
            parameters=params,
            description=description
        )

    params = [None] * param_count
    warning_message: str | None = None
    for i, row in enumerate(table_rows[:param_count]):
        try:
            pnum, ptype, pdesc = row.findAll("td")
        except Exception:
            warning_message = f"Expected parameters: {param_count}. Found: {i}. Documentation incomplete"
            break

        position = int(pnum.text.strip(". ")) - 1
        parameter_type = NativeType.from_string(ptype.text)
        parameter_description = pdesc.text.strip()

        params[position] = NativeParameter(
            parameter_type=parameter_type,
            original_type_string=ptype.text.strip(),
            description=parameter_description
        )

    if not all(params):
        null_index = params.index(None)
        assert not any(params[null_index:]), "There are null parameters in between defined parameters"

    return NativeFunction(
        function_name=name,
        return_type=return_type,
        return_type_description=return_type_description,
        parameters=[p for p in params if p],
        description=description,
        warning_message=warning_message
    )


def get_natives_url_ref(force: bool = False) -> dict[str, str]:
    if not os.path.exists(NATIVE_DATA_PATH / "availability.json") or force:
        soup = get_soup()
        data = parse_main_list(soup)

        with open(NATIVE_DATA_PATH / "availability.json", "w") as f:
            f.write(json.dumps(data, indent=2))

        return data

    with open(NATIVE_DATA_PATH / "availability.json", "r") as f:
        return json.load(f)

def get_native_list(force: bool = False) -> List[str]:
    return list(get_natives_url_ref(force).keys())
            

def get_native_data(function_name: str, force: bool = False) -> dict:
    clean_name = function_name.strip().upper()

    natives = get_natives_url_ref()
    if clean_name not in natives:
        natives = get_natives_url_ref(force=True)
    if clean_name not in natives:
        raise NativeNotFoundException(f"The requested native does not exist. Requested: {function_name}. Cleaned: {clean_name}")

    if not os.path.exists(NATIVE_DATA_PATH / f"{clean_name}.json") or force:
        native_url = natives[clean_name]
        soup = get_soup(native_url)
        
        native_function = parse_native_function(soup, clean_name)
        native_data = native_function.to_dict()

        with open(NATIVE_DATA_PATH / f"{clean_name}.json", "w") as f:
            f.write(json.dumps(native_data, indent=2))

        return native_data

    with open(NATIVE_DATA_PATH / f"{clean_name}.json", "r") as f:
        return json.load(f)
