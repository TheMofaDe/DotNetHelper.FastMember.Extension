Task("Build")
    .IsDependentOn("Clean")
	.IsDependentOn("Format-Code")
    .Does<BuildParameters>((parameters) =>
{
    Build(parameters.Configuration,MyProject.SolutionFileName);
});