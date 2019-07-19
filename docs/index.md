# DotNetHelper.FastMember.Extension

#### *An object instance creator & object mapper that uses Fast Member for reflection purposes. Works with dynamic, generics, and anonymous types*  

|| [**View on Github**][Github] || 


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
[Github]: https://github.com/TheMofaDe/DotNetHelper.FastMember.Extension


<!-- Documentation Links. -->
[Docs]: https://themofade.github.io/DotNetHelper.FastMember.Extension/index.html
[Docs-API]: https://themofade.github.io/DotNetHelper.FastMember.Extension/api/DotNetHelper.FastMember.Extension.Attribute.html
[Docs-Tutorials]: https://themofade.github.io/DotNetHelper.FastMember.Extension/tutorials/index.html
[Docs-samples]: https://dotnet.github.io/docfx/
[Changelogs]: https://dotnet.github.io/docfx/