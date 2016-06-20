# windowpositioner
Extremely simple console utility that automatically positions application windows on your monitor in a grid.

## config
Configuration supported through a JSON file in the settings folder. You can specify the processes whose windows to include, as well as which screen index to use. An example configuration looks like:

```csharp
{
  "ProcessNames": [
    "Notepad",
    "Explorer"
  ],
  "ScreenIndex": 0
}
```

## get it
Download a copy from the releases page or clone the source and build it in visual studio. You'll need Visual Studio 2015 and it should compile out of the box.

## license
This utility and its source is licensed under the [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0). Submodules may be licensed differently. Please respect all licenses.
