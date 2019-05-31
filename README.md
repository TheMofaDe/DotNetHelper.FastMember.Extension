# DotNet Starter Template


| Package  | Tests | Coverage |
| :-----:  | :---: | :------: |
| ![Build Status][nuget-downloads]  | ![Build Status][tests]  | [![Coverage Status](https://coveralls.io/repos/github/TheMofaDe/DotNetHelper.FastMember.Extension/badge.svg)](https://coveralls.io/github/TheMofaDe/DotNetHelper.FastMember.Extension) |

### *Azure DevOps*
| Windows | Linux | MacOS |
| :-----: | :-----: | :---: | 
| ![Build Status][azure-windows]  | ![Build Status][azure-linux]  | ![Build Status][azure-macOS] 

### *AppVeyor*
| Windows |
| :-----: | 
| ![Build Status][appveyor-windows]


#####  DotNet Starter Template is a starter kit for building .NET libraries and application. This project defined by the following things

1. Using a folder structure that is based on 
[Microsoft Standard][1]. and that is also heavily used through-out the open source community.
          
| Folder Name | Description |
| ------ | ------ |
| src | Main projects (the product code) |
| tests | Test projects |
| docs | Documentation stuff, markdown files, help files etc. |
| samples | (optional) - Sample projects |
| lib | Things that can NEVER exist in a nuget package |
| artifacts | Build outputs go here. Doing a build.cmd/build.sh generates artifacts here (nupkgs, dlls, pdbs, etc.) |
| build | Build customizations scripts|
 
<br/> 

2. Using generic scripts that will implement the **DEV** in **DEV**OPS  
     A. You will have to make a few changes to one script file to apply your project specific configurations *eg Project Name & Target Frameworks*  
     
## Getting Started

##### Step #1 
Clone or download this repository
```bash
git clone https://github.com/TheMofaDe/DotNet-Starter-Template.git 
```

##### Step #2 
Open folder in Visual Studio & open the Test.sln file in the root folder and start creating new or add your existing .NET project to the solution 
<br/> 
##### Step #3 
Search & Replace all occurance the text DotNetHelper.FastMember.Extension with your actual project name

##### Step #4 
Rename Test.sln to your actual project name

##### Step #5 
Update the *build/project.cake* file to contain your main project name  

## See it in Action  
###### After completing the steps above. Just execute the build.ps1 to see the following     
* **Versioning** *: version the application using [GitVersion]*  
- **Clean** *: remove any & all files from a previous build of the current version of the application*   
+ **Build** *: compiles the application*     
- **Test** *: execution of unit test*  
+ **Documentation** *: generation of documention of source code API into a static html thats customizable using [DocFX]*  
+ **Nuget Packages** *: create nuget package for the application*  
- **Zip** *: create zip file that contains the output of the build*  
+ **MSI Installer** *: creates a msi using [WiX]*



## Documentation
For more information, please refer to the [Officials Docs][2]

Created Using [DotNet-Starter-Template](http://themofade.github.io/DotNet-Starter-Template) 


<!-- Links. -->

[1]:  https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[2]: http://themofade.github.io/DotNetHelper.FastMember.Extension

[Cake]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[Azure DevOps]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[AppVeyor]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[GitVersion]: https://gitversion.readthedocs.io/en/latest/
[Nuget]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[Chocolately]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[WiX]: http://wixtoolset.org/
[DocFx]: https://dotnet.github.io/docfx/



<!-- BADGES. -->

[nuget-downloads]: https://img.shields.io/nuget/dt/DotNetHelper.FastMember.Extension.svg?style=flat-square
[tests]: https://img.shields.io/appveyor/tests/themofade/DotNetHelper.FastMember.Extension.svg?style=flat-square
[coverage-status]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper.FastMember.Extension/_apis/build/status/TheMofaDe.DotNetHelper.FastMember.Extension?branchName=master&jobName=Windows

[azure-windows]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper.FastMember.Extension/_apis/build/status/TheMofaDe.DotNetHelper.FastMember.Extension?branchName=master&jobName=Windows
[azure-linux]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper.FastMember.Extension/_apis/build/status/TheMofaDe.DotNetHelper.FastMember.Extension?branchName=master&jobName=Linux
[azure-macOS]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper.FastMember.Extension/_apis/build/status/TheMofaDe.DotNetHelper.FastMember.Extension?branchName=master&jobName=macOS

[appveyor-windows]: https://ci.appveyor.com/project/TheMofaDe/DotNetHelper.FastMember.Extension/branch/master
