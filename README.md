This is a collection of .NET utilities that can be used with [Critical Manufacturing MES](https://www.criticalmanufacturing.com/).

# DocGenerator

This utility generates documentation in the context of IoT equipment integration. Specifically, it takes the equipment's MasterData JSON file and generates the **Configuration** markdown document.

To configure it, just edit the first three lines of the `Main()` method:

  * Specify the the directory path of target project repository in the first.
  * Specify the name of the equipment in the second.
  * Finally, enter the protocol package to be used in the third line.

If then you run it, a file will be generated in the expected location:

```
UI\Help\src\packages\cmf.docs.area.cree\assets\TechSpec\connectiot\iotequipmenttypes\{equipmentName}\{equipmentName}-Configuration.md
```