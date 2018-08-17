![octavia logo](images/logo.png)

![release version number](https://img.shields.io/github/release/bodzaital/octavia.svg)
![dotnet core version 2.1](https://img.shields.io/badge/dotnet%20core-2.1-blue.svg)
![apache 2 license](https://img.shields.io/github/license/bodzaital/octavia.svg)

## What

A dotnet core app that concatenates css files into one.

## How

Make sure you have dotnet core 2.1 installed. [Download the latest dotnet core runtime here](https://www.microsoft.com/net/download).

`dotnet octavia.dll [arguments]`

| Argument || Description | Example |
| --- | --- | --- | --- |
| `-src` | *required* | specify a source folder to watch | `-src "source"` |
| `-dest` | *required* | specify a destination file to compile into | `-dest "publish/ouput.css"` |
| `-no-regions` | *optional* | prevent #region and #endregion comments | `-no-regions` |
| `-ext` | *optional* | specify an extension to watch, default: `css` | `-ext "scss"` |
| `-conf` | *optional* | specify a configuration file, see below | `-conf conf.txt` |

**Example: starts a watch on folder "source" and outputs into "publish/output.css":**
```
dotnet octavia.dll -src "source" -dest "publish/output.css"
```

The output file always needs to exists before running the watcher.

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
