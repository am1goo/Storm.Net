![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/am1goo/Storm.Net)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/am1goo/Storm.Net)
![GitHub License](https://img.shields.io/github/license/am1goo/Storm.Net)

![NuGet Version](https://img.shields.io/nuget/v/am1goo.Storm.Net)
![NuGet Downloads](https://img.shields.io/nuget/dt/am1goo.Storm.Net)

## Storm.Net
**STORM (Strong Typed Object Referenced Model)** - another way to make a deal with configuration files. \
It's easy to use and give to you unlimited power to control every field or property in your file.

## Why I made it?
I have been work in video game industry at least 10 year and I've see how many errors was produced by regular game designers who works with `JSON` config files. Each error can produce bugs and our most large time-eaters - tasks in trackers! \
As you know, a lot of games used `JSON` format to declare game data and all of those files can be huge! \
For example: [GTA 5 game config](https://nee.lv/2021/02/28/How-I-cut-GTA-Online-loading-times-by-70/) has **63k items** and **10mb size**! \
And I've decided that it could be easy to modify if it will have some tips like type definitions.

## Syntax
Parameter key and his type shoud be separate by `:` colon symbol. \
Value should be next after `=` equals sign.

- Primitive and custom types, also enums: `{param name}`:`{type}` = `{value}`
- Arrays: `{array name}` = `[ .. any values between brackets .. ]`
- Objects: `{object name}` = `{ .. any properties between braces .. }`

## Built-in primitive types:
- `b` - boolean
- `bt` - byte
- `sbt` - signed byte
- `s` - short (int16)
- `us` - unsigned short (uint16)
- `i` - integer (int32)
- `ui` - unsigned integer (uint32)
- `l` - long (int64)
- `ul` - unsigned long (uint64)
- `f` - single aka float
- `d` - double
- `dec` - decimal
- `t` - string
- `e` - enums
- `url` - path to external file

## File association and format
*.storm* - is preferred extension for STORM files

```storm
#this is a comment

#b - means "boolean" and can be one of case-insensitives values 'yes' or '1' or 'tRuE' or 'no' or '0' or 'fAlSe'
param_1:b = yes

#i - means "integer"
param_2:i = -123

#f - means "float"
param_3:f = 32.32

#t - means "text" or "string"
param_4:t = this is a single line

param_5:t = "and this is a multi line
also called \"The King of Strings\".
It's not a joke!"

#objects should be mapped w/o any type
obj_1 = {
  field_1:t = "I'm a first field"
  field_2:s = 123
  field_3 = {
    arg_1:i = 123
    arg_2:sbt = 4
  }
}

#arrays should be mapped w/o any type as well
#also all array values can no have names
array_1 = [
  :t = "first element"
  :t = second element
  :t = 3rd element
]

#any inline objects should be mapped via type 'url'
obj = {
  internal:url = file:///home/user/internal.storm
  external:url = http://youhostname.domain/files/external.storm
}
```

## Features
- All types of primitive types
- Enum deserialization from strings or numbers
- Single-line and multi-line strings
- Ability to inline any amount of inherit files (from local files or web servers)
- Customization of deserialization process (you can add support to any your custom types!)

## Installation
You can install NuGet package [am1goo.Storm.Net](https://www.nuget.org/packages/am1goo.Storm.Net/) into a .NET project.
1. Open a command prompt and go to your's project folder
2. Use the following command to install the Storm.Net package:
```dotnetcli
dotnet add package am1goo.Storm.Net
```
3. After the command completes, open the *.csproj* file in Visual Studio to see the added NuGet package reference:
```xml
<ItemGroup>
  <PackageReference Include="am1goo.Storm.Net" Version="1.0.0" />
</ItemGroup>
```
    
## Example
```csharp
using Storm;
using Storm.Serializers;

public class Example
{
  var settings = new StormSettings(StormSettings.Options.IgnoreCase, new List<IStormConverter>
  {
    new UrlStormConverter(), //added ability to load parts of data from other files
  });

  var serializer = new StormSerializer();
  var obj = await serializer.DeserializeFileAsync("Examples/test-file.storm", settings);
  /* do something with obj */
}
```

## Requirements
- Visual Studio 2019 or higher
- .NET App 2.0 or higher
- .NET Standard 2.1 or higher
- .NET Core 5.0 and higher

## Contribute
Contribution in any form is very welcome. Bugs, feature requests or feedback can be reported in form of Issues.
