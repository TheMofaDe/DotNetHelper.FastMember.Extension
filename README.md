# DotNetHelper.FastMember.Extension


#### *An object instance creator & object mapper that uses Fast Member for reflection purposes. Works with dynamic, generics, and anonymous types* 

|| [**Documentation**][Docs] • [**API**][Docs-API] • [**Tutorials**][Docs-Tutorials] ||  [**Change Log**][Changelogs] • || [**View on Github**][Github]|| 

| AppVeyor | AzureDevOps |
| :-----: | :-----: |
| [![Build status](https://ci.appveyor.com/api/projects/status/DotNetHelper.FastMember.Extension?svg=true)](https://ci.appveyor.com/project/TheMofaDe/DotNetHelper.FastMember.Extension)  | [![Build Status](https://dev.azure.com/Josephmcnealjr0013/DotNetHelper.FastMember.Extension/_apis/build/status/TheMofaDe.DotNetHelper.FastMember.Extension?branchName=master)](https://dev.azure.com/Josephmcnealjr0013/DotNetHelper.ObjectToSql/_build/latest?definitionId=5&branchName=master)  

| Package  | Tests | Code Coverage |
| :-----:  | :---: | :------: |
| ![Build Status][nuget-downloads]  | ![Build Status][tests]  | [![codecov](https://codecov.io/gh/TheMofaDe/DotNetHelper.FastMember.Extension/branch/master/graph/badge.svg)](https://codecov.io/gh/TheMofaDe/DotNetHelper.FastMember.Extension) |



## GET & SET Dynamic, Generics, and Anonymous Object Values 
```csharp
public class Employee {
      public FirstName { get; set; }
      public LastName  { get; set; }
}

// CREATE A GENERIC, DYNAMIC, & ANONYMOUS OBJECT 
            var employee = new Employee() { FirstName = "generic" };
            dynamic dynamicEmployee = new ExpandoObject();
            var anonymousEmployee = new { FirstName = "I'm so Anonymous" };

// SET PROPERTY VALUE FOR GENERICS & DYNAMICS OBJECTS
            ExtFastMember.SetMemberValue(employee, "FirstName", "I'm so generic");
            ExtFastMember.SetMemberValue(dynamicEmployee, "FirstName", "I'm so Dynamic");

// GET PROPERTY VALUES FOR GENERICS & DYNAMICS & ANONYMOUS OBJECTS
            Console.WriteLine(ExtFastMember.GetMemberValue(employee,"FirstName")); // PRINTS : I'm so generic
            Console.WriteLine(ExtFastMember.GetMemberValue(dynamicEmployee, "FirstName"));  // PRINTS : I'm so Dynamic
            Console.WriteLine(ExtFastMember.GetMemberValue(anonymousEmployee, "FirstName"));  // PRINTS : I'm so Anonymous
```






## Documentation
For more information, please refer to the [Officials Docs][Docs]

<!-- Links. -->
## Solution Template
[![badge](https://img.shields.io/badge/Built%20With-DotNet--Starter--Template-orange.svg)](https://github.com/TheMofaDe/DotNet-Starter-Template)


<!-- Links. -->

[1]:  https://gist.github.com/davidfowl/ed7564297c61fe9ab814

[Cake]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[Azure DevOps]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[AppVeyor]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[GitVersion]: https://gitversion.readthedocs.io/en/latest/
[Nuget]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[Chocolately]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[WiX]: http://wixtoolset.org/
[DocFx]: https://dotnet.github.io/docfx/
[Github]: https://github.com/TheMofaDe/DotNetHelper.FastMember.Extension


<!-- Documentation Links. -->
[Docs]: https://themofade.github.io/DotNetHelper.FastMember.Extension/index.html
[Docs-API]: https://themofade.github.io/DotNetHelper.FastMember.Extension/api/DotNetHelper.FastMember.Extension.html
[Docs-Tutorials]: https://themofade.github.io/DotNetHelper.FastMember.Extension/tutorials/index.html
[Docs-samples]: https://dotnet.github.io/docfx/
[Changelogs]: https://dotnet.github.io/docfx/


<!-- BADGES. -->

[nuget-downloads]: https://img.shields.io/nuget/dt/DotNetHelper.FastMember.Extension.svg?style=flat-square
[tests]: https://img.shields.io/appveyor/tests/TheMofaDe/DotNetHelper.FastMember.Extension.svg?style=flat-square
[coverage-status]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper.FastMember.Extension/_apis/build/status/TheMofaDe.DotNetHelper.FastMember.Extension?branchName=master&jobName=Windows


