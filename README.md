# Convert Project Dependencies to MSBuild Project references

ConvertProjDepToProjRef is a tool to convert project dependencies in a Visual Studio solution file (.sln)
into MSBuild project references (ProjectReference) in project files. It will do 95% of the work.  
Project dependencies in an .sln file are an older method to define build
dependencies and are slower because MSBuild must first create meta projects
files.

## Features

* .NET 9.0 App.

## Build Instructions

Open a console windows and execute the following commands:

```bash
git clone https://github.com/vbaderks/ConvertProjDepToProjRef.git
```

```bash
cd ConvertProjDepToProjRef
```

```bash
dotnet build -c Release
```

## Usage Instructions

* Run the tool on a solution file: ConvertProjDepToProjRef.exe MySolution.sln
* Update C++ projects that only perform custom builds as ConfigurationType “Utility”
* Remove the RuntimeIdentifier element for C# projects that only perform custom builds
* Use a text editor to remove the ProjectSection(ProjectDependencies) = postProject sections from the .sln file.

## Prebuild Executable

As convenience a prebuild executable for Windows x64 is available.
Go to the [releases](https://github.com/vbaderks/ConvertProjDepToProjRef/releases) page and click on
Assets at the bottom to show the files available in a release.
Download the binary file ConvertProjDepToProjRef.exe.

> [!NOTE]
> Microsoft Defender SmartScreen may show a warning about an unrecognised app when running the executable for the first time.
Click on "More Info" + "Run anyway" to continue.  
The executable is signed, but Defender SmartScreen requires an Extended Validation (EV) code signing certificate, which is only available
to commercial organisations, for direct full trust.
