Task("Format-Code")
.WithCriteria<BuildParameters>((context, parameters) => parameters.IsLocalBuild,  "Format-Code will only run during a local build")
    .Does<BuildParameters>((parameters) =>
{
     var exitCode = StartProcess(@"dotnet-format", new ProcessSettings { Arguments = " -w " + MyProject.SolutionDir });
     if (exitCode != 0) throw new Exception("Failed to format code");
}).ReportError(exception =>
{
    Error(exception.Dump());
});
