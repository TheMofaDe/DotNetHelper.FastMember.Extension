# DotNetHelper.FastMember.Extension

An object instance creator & object mapper that uses Fast Member for reflection purposes.
Works with dynamic, generics, and anonymous types

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



## Set Dynamic, Generics, and Anonymous Object Values 
```csharp
public class Employee {
      public FirstName { get; set; }
      public LastName  { get; set; }
}

// SET AS GENERIC
          var employee = new Employee(){ FirstName = "Joseph" };
          ExtFastMember.SetMemberValue(employee,"FirstName","Joseph");
// SET AS DYNAMIC 
          var employee = new ExpandoObject();
          ExtFastMember.SetMemberValue(employee,"FirstName","Joseph");
// SET AS ANONYMOUS
          var employee = new { FirstName = "YEET"};
          ExtFastMember.SetMemberValue(employee,"FirstName","Joseph"); 
          // NOT SUPPORTED THROWS EXCEPTION SEE https://stackoverflow.com/a/30242237/2445462          
```




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
