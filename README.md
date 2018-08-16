# octavia

![release version number](https://img.shields.io/github/release/bodzaital/octavia.svg)
![dotnet core version 2.1](https://img.shields.io/badge/dotnet%20core-2.1-blue.svg)
![apache 2 license](https://img.shields.io/github/license/bodzaital/octavia.svg)

## What

A dotnet core app that concatenates css files into one.

## How

`dotnet octavia.dll [arguments]`

| Argument || Description |
| --- | --- | --- |
| `-src` | *required*  |specify a source folder to watch |
| `-dest` | *required*  |specify a destination file to compile into |
| `-no-regions` | *optional*  |prevent #region and #endregion comments |
| `-ext` | *optional*  |specify an extension to watch, default: `css` |
| `-conf` | *optional*  |specify a configuration file |

## Conf file

List the files in the source folder in the order you'd like to include them.


**Folder structure:**
```
source
   └ variables.css
   └ root.css
   └ body.css
   └ reset.css
```

**Conf file:**
```
root
reset
body
```

In this example, the program will only include the above three files in that order.
