Task("Zip-Files")
    .IsDependentOn("Copy-Files")
    .Does<BuildParameters>((parameters) =>
{

    foreach(var targetFramework in MyProject.TargetFrameworks){   

		var filename = parameters.Paths.Directories.Artifacts.CombineWithFilePath(MyProject.ProjectName) + "-bin-" + targetFramework + "fx-v" + parameters.Version.SemVersion + ".zip";
		var fullFxDir = parameters.Paths.Directories.ArtifactsBin.Combine(targetFramework);
		var fullFxFiles = GetFiles(fullFxDir.FullPath + "/**/*");
		Zip(fullFxDir, filename, fullFxFiles);
   
    }
});