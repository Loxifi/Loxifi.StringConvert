A cursed extension library that attempts to convert from a string, to any and all possible target types. It does this by using a combination of type specific hard coded logic, and reflection to identify possible parse or conversion methods. See readme for more

Supports loose conversions like "yes" and "no" or "1" and "0" for bools, as well as int/string values for enums.

Usage example

```
bool myBool = "yes".Convert<bool>();

float myFloat = "0.005".Convert<float>();

MyEnum myEnum = "2".Convert<MyEnum>();

```

Additional information will be provided on full release
