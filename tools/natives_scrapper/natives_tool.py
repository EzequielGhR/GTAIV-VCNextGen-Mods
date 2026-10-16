import json
import sys

from scrapper import get_native_list, get_native_data


USAGE_MSG = f"""USAGE:
    - {sys.argv[0]} list:
        List all available GTA Natives
    - {sys.argv[0]} help:
        Show this help message.
    - {sys.argv[0]} [FUNCTION_NAME]:
        Fetch the specific function name
    - {sys.argv[0]} [FUNCTION_NAME] force:
        Force a fetch of new data about the specified function name.
"""


if __name__ == "__main__":
    if len(sys.argv) < 2 or sys.argv[1].strip().lower() == "help":
        print(USAGE_MSG)
        exit(1)
    command = sys.argv[1].strip()
    if (command.lower() == "list"):
        print("\n".join(get_native_list()))
    elif (command.lower() == "help"):
        print(USAGE_MSG)
    else:
        force = len(sys.argv) > 2 and sys.argv[2].strip().lower() == "force"
        print(f"Fetching data for '{command}'. Force: {force}")
        try:
            data = get_native_data(command, force)
            print(data)
        except Exception as e:
            print(f"There was an error fetching data: {str(e)}")
    