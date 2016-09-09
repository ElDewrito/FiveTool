# FiveTool

FiveTool is an unofficial modding toolkit for Halo 5.
The goal is to eventually create a powerful Lua-based system which can be used to inspect and modify game assets.

Visual Studio 2015 is required to build FiveTool and related projects.

## ModuleExtractor

You can use ModuleExtractor to extract the .module files that the Windows Store version of the game uses.
(Actually getting a .module file is left as an exercise for the reader.)

The easiest way to use it is to drag-and-drop a module file onto the program.
You can also run it from the command line by passing in a path to a .module file and an optional output folder.

There is currently no way to rebuild modules after individual files have been edited. Stay tuned!
