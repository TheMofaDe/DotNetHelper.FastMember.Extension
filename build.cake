// Install modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.1.0

// Install addins.
#addin "nuget:?package=Cake.Gitter&version=0.9.0"
#addin "nuget:?package=Cake.Docker&version=0.9.6"
#addin "nuget:?package=Cake.Npm&version=0.15.0"
#addin "nuget:?package=Cake.Incubator&version=3.0.0"
#addin "nuget:?package=Cake.Json&version=3.0.0"
#addin "nuget:?package=Cake.Tfx&version=0.8.0"
#addin "nuget:?package=Cake.Gem&version=0.7.0"
#addin "nuget:?package=Cake.Coverlet&version=2.2.1"
#addin "nuget:?package=Cake.Codecov&version=0.5.0"
#addin "nuget:?package=Newtonsoft.Json&version=9.0.1"
#addin "nuget:?package=xunit.assert&version=2.4.1"
#addin "nuget:?package=Cake.DocFx&version=0.12.0"


// Install tools.
#tool "nuget:?package=NUnit.ConsoleRunner&version=3.9.0"
#tool "nuget:?package=GitReleaseNotes&version=0.7.1"
#tool "nuget:?package=ILRepack&version=2.0.16"
#tool "nuget:?package=Codecov&version=1.1.0"
#tool "nuget:?package=nuget.commandline&version=4.9.2"
#tool "nuget:?package=GitVersion.CommandLine&version=5.0.0-beta2-95"
#tool "nuget:?package=docfx.console&version=2.43.1"
#tool "nuget:?package=WiX.Toolset.UnofficialFork&version=3.11.1"
#tool "nuget:?package=OpenCover&version=4.7.922"
#tool nuget:?package=ReportGenerator&version=4.0.4

// Install .NET Core Global tools.
#tool "dotnet:?package=GitReleaseManager.Tool&version=0.8.0"

// Load other scripts.
#load "./build/parameters.cake"
#load "./build/utils.cake"

using Xunit;
//////////////////////////////////////////////////////////////////////
// PARAMETERS
//////////////////////////////////////////////////////////////////////
bool publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup<BuildParameters>(context =>
{
    var parameters = BuildParameters.GetParameters(Context);

    Build(parameters.Configuration,MyProject.SolutionFileName);
    var gitVersion = GetVersion(parameters);
    parameters.Initialize(context, gitVersion);

    if (parameters.IsMainBranch && (context.Log.Verbosity != Verbosity.Diagnostic)) {
        Information("Increasing verbosity to diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    Information("Building version {0} of {3} ({1}, {2})",
        parameters.Version.SemVersion,
        parameters.Configuration,
        parameters.Target,
        MyProject.ProjectName);

    Information("Repository info : IsMainRepo {0}, IsMainBranch {1}, IsTagged: {2}, IsPullRequest: {3}",
        parameters.IsMainRepo,
        parameters.IsMainBranch,
        parameters.IsTagged,
        parameters.IsPullRequest);

    return parameters;
});

Teardown<BuildParameters>((context, parameters) =>
{
    try
    {
        Information("Starting Teardown...");

        Information("Repository info : IsMainRepo {0}, IsMainBranch {1}, IsTagged: {2}, IsPullRequest: {3}",
            parameters.IsMainRepo,
            parameters.IsMainBranch,
            parameters.IsTagged,
            parameters.IsPullRequest);

        if(context.Successful)
        {

        }

        Information("Finished running tasks.");
    }
    catch (Exception exception)
    {
        Error(exception.Dump());
    }
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

#region Build

Task("Clean")
    .Does<BuildParameters>((parameters) =>
{
    Information("Cleaning directories..");

    CleanDirectories($"{MyProject.ProjectDir}bin/" + parameters.Configuration);
    CleanDirectories($"{MyProject.ProjectDir}obj");
    CleanDirectories(parameters.Paths.Directories.ToClean);
});


Task("Build")
    .IsDependentOn("Clean")
    .Does<BuildParameters>((parameters) =>
{
    Build(parameters.Configuration,MyProject.SolutionFileName);
});

#endregion

#region Tests

Task("Test")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,  "Unit tests will only run on windows agent.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledUnitTests, "Unit tests were disabled.")
    .IsDependentOn("Build")
    .Does<BuildParameters>((parameters) =>
{
 

    // run using dotnet test
    var projects = GetFiles("./tests/**/*.Tests.csproj");
	var coverageFile = parameters.Paths.Directories.TestCoverageOutput + $"/CodeCoverage.xml";
    foreach(var project in projects)
    {

        foreach(var targetFramework in MyProject.TargetFrameworks){

		var tf = targetFramework.Replace("netstandard","netcoreapp");

	    var testAssemblies = GetFiles("./tests/**/bin/" + parameters.Configuration + "/" + tf + "/*.Tests.dll");

		var nunitSettings = new NUnit3Settings
		{
		    Results = new List<NUnit3Result> { new NUnit3Result { FileName = parameters.Paths.Directories.TestCoverageOutput + $"/TestResult.xml"  } }
		};
		if(IsRunningOnUnix()) {
		    nunitSettings.Where = "cat!=NoMono";
		    nunitSettings.Agents = 1;
		}

       OpenCover(tool => {
			tool.NUnit3(testAssemblies, nunitSettings);
        },
        new FilePath(coverageFile),
        new OpenCoverSettings(){
            LogLevel = OpenCoverLogLevel.Info,
			OldStyle = true,
			MergeOutput = false
        }     
        //.WithFilter("+[*.Tests*]*")
		//.WithFilter("-[*NUnit3.*]*")
		);

        }
		    
     }
	  //  ReportGenerator(coverageFile,parameters.Paths.Directories.TestCoverageOutput + "/" + "htmlreports");

});


Task("Generate-Docs")
.WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,  "Generate-Docs will only run on windows agent.")
.Does<BuildParameters>((parameters) => 
{
	DocFxMetadata("./docs/docfx.json");
	DocFxBuild("./docs/docfx.json");
	if(DirectoryExists(parameters.Paths.Directories.Artifacts))
	Zip("./docs/_site/", parameters.Paths.Directories.Artifacts + "/docfx.zip");
});


#endregion

//////////////////////////////////////////////////////////////////////
// Package MSI Tasks
//////////////////////////////////////////////////////////////////////

Task("Heat")
  .IsDependentOn("Build")
   .Does<BuildParameters>((parameters) =>{


    foreach(var targetFramework in MyProject.TargetFrameworks){
    DirectoryPath harvestDirectory = Directory($"{MyProject.ProjectDir}bin/{parameters.Configuration}/{targetFramework}/");
    var filePath = File("./wix/installer/Components.wxs");

	var heatSettings = new HeatSettings {
			ArgumentCustomization = args => args.Append("-var var.HeatSourceFilesDir")
												.Append("-dr AppDir")
           ,ComponentGroupName = "NetBinComponents"
	       ,GenerateGuid = true
		   ,SuppressCom = true
		   ,SuppressRegistry = true
		   ,SuppressFragments = true
		   ,SuppressRootDirectory = true
		   ,OutputFile = "./wix/installer/Components.wxs"  
		   ,PreprocessorVariable = "var.HeatSourceFilesDir" 
        };

    WiXHeat(harvestDirectory, filePath,WiXHarvestType.Dir,heatSettings);
    }
  
  
  });


Task("Candle")
   .IsDependentOn("Heat") 
   .Does<BuildParameters>((parameters) =>{


    foreach(var targetFramework in MyProject.TargetFrameworks){
        var files = GetFiles("./wix/**/*.wxs");
        var settings = new CandleSettings {
			ArgumentCustomization = args => args
                .Append("-dHeatSourceFilesDir=" + $"{MyProject.ProjectDir}bin/{parameters.Configuration}/{targetFramework}/")
			  ,
            Verbose = true,
            NoLogo = true,
            OutputDirectory = parameters.Paths.Directories.ArtifactsBin.Combine(targetFramework),
		    Defines = new Dictionary<string, string>(){ 
						 {"SourceDir", parameters.Paths.Directories.ArtifactsBin.Combine(targetFramework).ToString() }
						 ,{"ProductName",  $"{MyProject.ProjectName}" }
						 ,{"Version",  $"{parameters.Version.SemVersion}" }
					     ,{"Manufacturer",  $"{MyProject.Manufacturer}" }
				  }            
        };

        WiXCandle(files, settings);
    }
  });

Task("Light")
  .IsDependentOn("Copy-Files")
  .IsDependentOn("Candle")
     .Does<BuildParameters>((parameters) =>{

             foreach(var targetFramework in MyProject.TargetFrameworks){
      LightSettings settings = new LightSettings {
	  	ArgumentCustomization = args => args
                .Append("-dHeatSourceFilesDir=" + $"{MyProject.ProjectDir}bin/{parameters.Configuration}/{targetFramework}/")
                ,
        Defines = new Dictionary<string, string>(){ 
						 {"SourceDir", parameters.Paths.Directories.ArtifactsBin.Combine(targetFramework).ToString() }
						 ,{"ProductName",  $"{MyProject.ProjectName}" }
						 ,{"Version",  $"{parameters.Version.SemVersion}" }
					     ,{"Manufacturer",  $"{MyProject.Manufacturer}" }
				  },
        OutputFile = parameters.Paths.Directories.Artifacts.ToString() + "/" +  MyProject.ProjectName + "_" + targetFramework +  "_Installer.msi",
        NoLogo = true,
     
        };
    WiXLight(parameters.Paths.Directories.ArtifactsBin.Combine(targetFramework).ToString() + "/*.wixobj", settings);
             }



  });




#region Package

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



Task("Pack-Gem")
    .IsDependentOn("Copy-Files")
    .Does<BuildParameters>((parameters) =>
{
    var workDir = "./src/ApplicationNameRubyGem";

    var gemspecFile = new FilePath(workDir + "/applicationname.gemspec");
    // update version number
    ReplaceTextInFile(gemspecFile, "$version$", parameters.Version.GemVersion);

    var toolPath = FindToolInPath(IsRunningOnWindows() ? "gem.cmd" : "gem");
    GemBuild(gemspecFile, new Cake.Gem.Build.GemBuildSettings()
    {
        WorkingDirectory = workDir,
        ToolPath = toolPath
    });

    CopyFiles(workDir + "/*.gem", parameters.Paths.Directories.BuildArtifact);
});

Task("Pack-Nuget")
    .IsDependentOn("Copy-Files")
    .Does<BuildParameters>((parameters) =>
{
    foreach(var package in parameters.Packages.Nuget)
    {
        if (FileExists(package.NuspecPath)) {
            var artifactPath = MakeAbsolute(parameters.PackagesBuildMap[package.Id]).FullPath;

            var nugetSettings = new NuGetPackSettings
            {
                Version = parameters.Version.SemVersion,
                OutputDirectory = parameters.Paths.Directories.NugetRoot,
                Files = GetFiles(artifactPath + "/**/*.*")
                        .Select(file => new NuSpecContent { Source = file.FullPath, Target = file.FullPath.Replace(artifactPath, "") })
                        .ToArray()
            };

            NuGetPack(package.NuspecPath, nugetSettings);
        }
    }

    var settings = new DotNetCorePackSettings
    {
        Configuration = parameters.Configuration,
        OutputDirectory = parameters.Paths.Directories.NugetRoot,
        NoBuild = true,
        NoRestore = true,
        MSBuildSettings = parameters.MSBuildSettings
    };


    DotNetCorePack($"{MyProject.ProjectDir}", settings);
    // DotNetCorePack($"{MyProject.ProjectDir}{MyProject.ProjectName}.csproj", settings);
});

Task("Pack-Chocolatey")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,  "Pack-Chocolatey works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsMainBranch, "Pack-Chocolatey works only for main branch.")
    .IsDependentOn("Copy-Files")
    .Does<BuildParameters>((parameters) =>
{
    foreach(var package in parameters.Packages.Chocolatey)
    {
        if (FileExists(package.NuspecPath)) {
            var artifactPath = MakeAbsolute(parameters.PackagesBuildMap[package.Id]).FullPath;

            var files = GetFiles(artifactPath + "/**/*.*")
                        .Select(file => new ChocolateyNuSpecContent { Source = file.FullPath, Target = file.FullPath.Replace(artifactPath, "") });
            var txtFiles = (GetFiles("./nuspec/*.txt") + GetFiles("./nuspec/*.ps1"))
                        .Select(file => new ChocolateyNuSpecContent { Source = file.FullPath, Target = file.GetFilename().ToString() });

            ChocolateyPack(package.NuspecPath, new ChocolateyPackSettings {
                Verbose = true,
                Version = parameters.Version.SemVersion,
                OutputDirectory = parameters.Paths.Directories.NugetRoot,
                Files = files.Concat(txtFiles).ToArray()
            });
        }
    }
});

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

Task("Docker-Build")
    .WithCriteria<BuildParameters>((context, parameters) => !parameters.IsRunningOnMacOS, "Docker can be built only on Windows or Linux agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreRelease(), "Docker-Build works only for releases.")
    .IsDependentOn("Copy-Files")
    .Does<BuildParameters>((parameters) =>
{
    var images = parameters.IsRunningOnWindows
            ? parameters.Docker.Windows
            : parameters.IsRunningOnLinux
                ? parameters.Docker.Linux
                : Array.Empty<DockerImage>();

    foreach(var dockerImage in images)
    {
        DockerBuild(dockerImage, parameters);
    }
});

Task("Docker-Test")
    .WithCriteria<BuildParameters>((context, parameters) => !parameters.IsRunningOnMacOS, "Docker can be tested only on Windows or Linux agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreRelease(), "Docker-Test works only for releases.")
    .IsDependentOn("Docker-Build")
    .Does<BuildParameters>((parameters) =>
{
    var currentDir = MakeAbsolute(Directory("."));
    var containerDir = parameters.IsRunningOnWindows ? "c:/repo" : "/repo";
    var settings = new DockerContainerRunSettings
    {
        Rm = true,
        Volume = new[] { $"{currentDir}:{containerDir}" }
    };

    var images = parameters.IsRunningOnWindows
            ? parameters.Docker.Windows
            : parameters.IsRunningOnLinux
                ? parameters.Docker.Linux
                : Array.Empty<DockerImage>();

    foreach(var dockerImage in images)
    {
        var tags = GetDockerTags(dockerImage, parameters);
        foreach (var tag in tags)
        {
            DockerTestRun(settings, parameters, tag, containerDir);
        }
    }
});

Task("Pack")
    .IsDependentOn("Pack-Nuget")
    .IsDependentOn("Pack-Chocolatey")
    .IsDependentOn("Zip-Files")
    .Does<BuildParameters>((parameters) =>
{
    Information("The build artifacts: \n");
    foreach(var artifact in parameters.Artifacts.All)
    {
        if (FileExists(artifact.ArtifactPath)) { Information("Artifact: {0}", artifact.ArtifactPath); }
    }

    foreach(var package in parameters.Packages.All)
    {
        if (FileExists(package.PackagePath)) { Information("Artifact: {0}", package.PackagePath); }
    }
})
    .ReportError(exception =>
{
    Error(exception.Dump());
});

#endregion

#region Publish

Task("Release-Notes")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,       "Release notes are generated only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAppVeyor, "Release notes are generated only on AppVeyor.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease(),        "Release notes are generated only for stable releases.")
    .Does<BuildParameters>((parameters) =>
{
     var releaseNotesExitCode = StartProcess(@"tools\GitReleaseNotes.0.7.1\tools\GitReleaseNotes.exe", new ProcessSettings { Arguments = ". /o RELEASENOTES.md" });
     if (string.IsNullOrEmpty(System.IO.File.ReadAllText("RELEASENOTES.md")))
         System.IO.File.WriteAllText("RELEASENOTES.md", "No issues closed since last release");

     if (releaseNotesExitCode != 0) throw new Exception("Failed to generate release notes");



}).ReportError(exception =>
{
    Error(exception.Dump());
});

Task("Publish-Coverage")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,       "Publish-Coverage works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAppVeyor, "Publish-Coverage works only on AppVeyor.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreRelease(), "Publish-Coverage works only for releases.")
    .IsDependentOn("Test")
    .Does<BuildParameters>((parameters) =>
{
    var coverageFiles = GetFiles(parameters.Paths.Directories.TestCoverageOutput + "/*Coverage.xml");

    var token = parameters.Credentials.CodeCov.Token;
    if(string.IsNullOrEmpty(token)) {
        throw new InvalidOperationException("Could not resolve CodeCov token.");
    }

    foreach (var coverageFile in coverageFiles) {
        // Upload a coverage report using the CodecovSettings.
        Codecov(new CodecovSettings {
            Files = new [] { coverageFile.ToString() },
            Token = token
		//	,Required = true
        });
		Information("Uploading Coverage File --> " + coverageFile.ToString());
    }
});

Task("Publish-AppVeyor")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,  "Publish-AppVeyor works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAppVeyor, "Publish-AppVeyor works only on AppVeyor.")
    .IsDependentOn("Pack")
    .IsDependentOn("Release-Notes")
    .Does<BuildParameters>((parameters) =>
{
    foreach(var artifact in parameters.Artifacts.All)
    {
        if (FileExists(artifact.ArtifactPath)) { AppVeyor.UploadArtifact(artifact.ArtifactPath); }
    }

    foreach(var package in parameters.Packages.All)
    {
        if (FileExists(package.PackagePath)) { AppVeyor.UploadArtifact(package.PackagePath); }
    }

    if (FileExists(parameters.Paths.Directories.TestCoverageOutput + $"/TestResult.xml")) {
        AppVeyor.UploadTestResults(parameters.Paths.Directories.TestCoverageOutput + $"/TestResult.xml" , AppVeyorTestResultsType.NUnit3);
    }
})
.OnError(exception =>
{
    Information("Publish-AppVeyor Task failed, but continuing with next Task...");
    Error(exception.Dump());
    publishingError = true;
});

Task("Publish-AzurePipeline")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,       "Publish-AzurePipeline works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Publish-AzurePipeline works only on AzurePipeline.")
    .WithCriteria<BuildParameters>((context, parameters) => !parameters.IsPullRequest,           "Publish-AzurePipeline works only for non-PR commits.")
    .IsDependentOn("Pack")
    .IsDependentOn("Release-Notes")
    .Does<BuildParameters>((parameters) =>
{
    foreach(var artifact in parameters.Artifacts.All)
    {
        if (FileExists(artifact.ArtifactPath)) { TFBuild.Commands.UploadArtifact(artifact.ContainerName, artifact.ArtifactPath, artifact.ArtifactName); }
    }
    foreach(var package in parameters.Packages.All)
    {
        if (FileExists(package.PackagePath)) { TFBuild.Commands.UploadArtifact("packages", package.PackagePath, package.PackageName); }
    }

    if (FileExists(parameters.Paths.Files.TestCoverageOutputFilePath)) {
        var data = new TFBuildPublishTestResultsData {
            TestResultsFiles = new[] { parameters.Paths.Files.TestCoverageOutputFilePath },
            TestRunner = TFTestRunnerType.NUnit
        };
        TFBuild.Commands.PublishTestResults(data);
    }
})
.OnError(exception =>
{
    Information("Publish-AzurePipeline Task failed, but continuing with next Task...");
    Error(exception.Dump());
    publishingError = true;
});




Task("Publish-DockerHub")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledPublishDocker,     "Publish-DockerHub was disabled.")
    .WithCriteria<BuildParameters>((context, parameters) => !parameters.IsRunningOnMacOS,        "Publish-DockerHub works only on Windows and Linux agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Publish-DockerHub works only on AzurePipeline.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreRelease(), "Publish-DockerHub works only for releases.")
    .IsDependentOn("Docker-Build")
    .IsDependentOn("Docker-Test")
    .Does<BuildParameters>((parameters) =>
{
    var username = parameters.Credentials.Docker.UserName;
    if (string.IsNullOrEmpty(username)) {
        throw new InvalidOperationException("Could not resolve Docker user name.");
    }

    var password = parameters.Credentials.Docker.Password;
    if (string.IsNullOrEmpty(password)) {
        throw new InvalidOperationException("Could not resolve Docker password.");
    }

    DockerLogin(parameters.Credentials.Docker.UserName, parameters.Credentials.Docker.Password);

    var images = parameters.IsRunningOnWindows
            ? parameters.Docker.Windows
            : parameters.IsRunningOnLinux
                ? parameters.Docker.Linux
                : Array.Empty<DockerImage>();

    foreach(var dockerImage in images)
    {
        DockerPush(dockerImage, parameters);
    }

    DockerLogout();
})
.OnError(exception =>
{
    Information("Publish-DockerHub Task failed, but continuing with next Task...");
    Error(exception.Dump());
    publishingError = true;
});

Task("Publish-NuGet")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledPublishNuget,      "Publish-NuGet was disabled.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,       "Publish-NuGet works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAppVeyor, "Publish-NuGet works only on AppVeyor.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreRelease() || parameters.IsMasterBranch, "Publish-NuGet works only for releases.")
    .IsDependentOn("Pack-NuGet")
    .Does<BuildParameters>((parameters) =>
{
    var apiKey = parameters.Credentials.Nuget.ApiKey;
    if(string.IsNullOrEmpty(apiKey)) {
        throw new InvalidOperationException("Could not resolve NuGet API key.");
    }

    var apiUrl = parameters.Credentials.Nuget.ApiUrl;
    if(string.IsNullOrEmpty(apiUrl)) {
        throw new InvalidOperationException("Could not resolve NuGet API url.");
    }

    foreach(var package in parameters.Packages.Nuget)
    {
        if (FileExists(package.PackagePath))
        {
            // Push the package.
            NuGetPush(package.PackagePath, new NuGetPushSettings
            {
                ApiKey = apiKey,
                Source = apiUrl
            });
        }
    }
})
.OnError(exception =>
{
    Information("Publish-NuGet Task failed, but continuing with next Task...");
    Error(exception.Dump());
    publishingError = true;
});

Task("Publish-Chocolatey")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledPublishChocolatey, "Publish-Chocolatey was disabled.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,       "Publish-Chocolatey works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Publish-Chocolatey works only on AzurePipeline.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreRelease(), "Publish-Chocolatey works only for releases.")
    .IsDependentOn("Pack-Chocolatey")
    .Does<BuildParameters>((parameters) =>
{
    var apiKey = parameters.Credentials.Chocolatey.ApiKey;
    if(string.IsNullOrEmpty(apiKey)) {
        throw new InvalidOperationException("Could not resolve Chocolatey API key.");
    }

    var apiUrl = parameters.Credentials.Chocolatey.ApiUrl;
    if(string.IsNullOrEmpty(apiUrl)) {
        throw new InvalidOperationException("Could not resolve Chocolatey API url.");
    }

    foreach(var package in parameters.Packages.Chocolatey)
    {
        if (FileExists(package.PackagePath))
        {
            // Push the package.
            ChocolateyPush(package.PackagePath, new ChocolateyPushSettings
            {
                ApiKey = apiKey,
                Source = apiUrl,
                Force = true
            });
        }
    }
})
.OnError(exception =>
{
    Information("Publish-Chocolatey Task failed, but continuing with next Task...");
    Error(exception.Dump());
    publishingError = true;
});

Task("Publish")
    .IsDependentOn("Publish-AppVeyor")
    .IsDependentOn("Publish-AzurePipeline")
    .IsDependentOn("Publish-Coverage")
    .IsDependentOn("Publish-NuGet")
    .IsDependentOn("Publish-Chocolatey")
    .IsDependentOn("Publish-DockerHub")
//  .IsDependentOn("Light")
    .Finally(() =>
{
    if (publishingError)
    {
        throw new Exception("An error occurred during the publishing of applicationName. All publishing tasks have been attempted."); // TODO :: configurable per project
    }
});

#endregion
Task("Default")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
RunTarget(target);