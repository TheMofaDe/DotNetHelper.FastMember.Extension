Task("Generate-Docs")
.WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,  "Generate-Docs will only run on windows agent.")
.WithCriteria<BuildParameters>((context, parameters) => parameters.IsLocalBuild,  "Generate-Docs will only run during a local build")
.Does<BuildParameters>((parameters) => 
{
	DocFxMetadata("./docs/docfx.json");
	DocFxBuild("./docs/docfx.json");
	if(DirectoryExists(parameters.Paths.Directories.Artifacts))
	Zip("./docs/_site/", parameters.Paths.Directories.Artifacts + "/docfx.zip");
});