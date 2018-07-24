# octavia

A dotnet core app to concat css files into one file. Hopefully doesn't have any bugs in it.

# usage

    -src: required - specify a source folder to watch.
    -dest: required - speficy a destination file to compile into.
    -no-regions: optional - prevent #region and #endregion comments.
    -ext: optional - specify an extension to watch. Default: "css"

# example
    dotnet octavia.dll -src "src" -dest "dist/output.css" -no-regions -ext "scss"

# Why did I wrote this

I made it for my other project, wikitables, so I can work in separate css files. I know there are probably npm apps that do the same thing, but I wanted to write my own with a semi-reusable command-line arguments parser. It... kinda works.