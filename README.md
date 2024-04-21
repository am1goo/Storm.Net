![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/am1goo/Ston.Net)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/am1goo/Ston.Net)
![GitHub License](https://img.shields.io/github/license/am1goo/Ston.Net)

## Ston.Net
STON (Strong Typed Object Notation) - another way to make a deal with configuration files.
It's easy to use and give to you unlimited power to control every fields in your file.

## File format
```
#this is a comment

#b - means "boolean" and can be one of case-insensitives values 'yes' or '1' or 'tRuE' or 'no' or '0' or 'fAlSe'
param_1:b = yes
#i - means "integer"
param_2:i = -123
#f - means "float"
param_3:f =   32.32
```

## Features
- all types of primitive types
- enum deserialization from strings or numbers
- single-line and multi-line strings
- ability to inline any amount of inherit files (from local files or web servers)
- customization of deserialization process (you can add support to any your custom types!)
- 
## Requirements
- .NET Core 5.0 and higher

## Contribute
Contribution in any form is very welcome. Bugs, feature requests or feedback can be reported in form of Issues.
