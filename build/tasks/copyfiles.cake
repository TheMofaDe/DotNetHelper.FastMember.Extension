Task("Copy-Files")
    .IsDependentOn("Test")
    .IsDependentOn("Generate-Docs")
    .Does<BuildParameters>((parameters) =>
{

    foreach(var targetFramework in MyProject.TargetFrameworks){    
    
    var outputDir = parameters.Paths.Directories.ArtifactsBin.Combine(targetFramework);
    // .NET CORE 
    DotNetCorePublish($"{MyProject.ProjectDir}{MyProject.ProjectName}.csproj", new DotNetCorePublishSettings
    {
        Framework = targetFramework, 
        NoRestore = true,
        NoBuild = false,
        Configuration = parameters.Configuration,
        OutputDirectory =  outputDir,
        MSBuildSettings = parameters.MSBuildSettings
    });

    // Copy license & Copy XML (since publish does not do this anymore)
    var licenseFile = "./LICENSE";
    if (FileExists($"{licenseFile}"))
    {
      CopyFileToDirectory($"{licenseFile}",  outputDir);
    }

    var xmlFile = $"{MyProject.ProjectDir}bin/{parameters.Configuration}/{targetFramework}/{MyProject.ProjectName}.xml";
    if (FileExists($"{xmlFile}"))
    {
      CopyFileToDirectory($"{xmlFile}",  outputDir);
    }

    }
});
