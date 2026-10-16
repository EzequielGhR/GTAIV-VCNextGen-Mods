from enum import StrEnum
from pydantic import BaseModel, ConfigDict
from typing import List, Optional


class NativeType(StrEnum):
    VOID = "VOID"
    BOOL = "BOOL"
    INT = "INT"
    UINT = "UINT"
    FLOAT = "FLOAT"
    UNKNOWN = "UNKNOWN"
    STRING = "STRING"

    @staticmethod
    def from_string(t: str) -> "NativeType":
        clean = t.lower().strip()
        if clean in ["none", "void"]:
            return NativeType.VOID
        if clean in ["handle", "int", "integer", "pointer"]:
            return NativeType.INT
        if clean in ["bool", "boolean"]:
            return NativeType.BOOL
        if clean == "float":
            return NativeType.FLOAT
        if clean in ["uint", "unsigned int", "unsigned integer"]:
            return NativeType.UINT
        if clean in ["str", "string", "char*", "char *"]:
            return NativeType.STRING

        return NativeType.UNKNOWN
    

class NativeParameter(BaseModel):
    parameter_type: NativeType
    original_type_string: str
    description: Optional[str] = None
    
    model_config = ConfigDict(from_attributes=True)

    def to_dict(self):
        return {
            "type": self.parameter_type.value,
            "description": self.description,
            "raw_type": self.original_type_string
        }


class NativeFunction(BaseModel):
    function_name: str
    return_type: NativeType
    return_type_description: str
    parameters: Optional[List[NativeParameter]] = None
    description: Optional[str] = None
    warning_message: Optional[str] = None

    model_config = ConfigDict(from_attributes=True)

    def to_dict(self):
        return {
            "function": self.function_name,
            "return_type": self.return_type.value,
            "return_description": self.return_type_description,
            "parameters": [p.to_dict() for p in self.parameters] if self.parameters else None,
            "description": self.description,
            "warning_message": self.warning_message
        }
