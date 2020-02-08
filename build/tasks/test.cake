
Task("Test")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,  "Unit tests will only run on windows agent.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledUnitTests, "Unit tests were disabled.")
    .IsDependentOn("Build")
    .Does<BuildParameters>((parameters) =>
{
 

    // run using dotnet test
    var projects = GetFiles("./tests/**/*.Tests.csproj");
	
    foreach(var project in projects)
    {

        foreach(var targetFramework in MyProject.TargetFrameworks){

		var tf = targetFramework.Replace("netstandard","netcoreapp");
	    if(tf.Contains("netcoreapp")){ // SEE https://github.com/nunit/nunit-console/issues/487
		    continue;
		}
	    var testAssemblies = GetFiles("./tests/**/bin/" + parameters.Configuration + "/" + tf + "/*.Tests.dll");

		var nunitSettings = new NUnit3Settings
		{
		    Results = new List<NUnit3Result> { new NUnit3Result { FileName = parameters.Paths.Files.TestResultOutputFilePath  } }
		};
		if(IsRunningOnUnix()) {
		    nunitSettings.Where = "cat!=NoMono";
		    nunitSettings.Agents = 1;
		}

       OpenCover(tool => {
			tool.NUnit3(testAssemblies, nunitSettings);
        },
        parameters.Paths.Files.TestCoverageOutputFilePath,
        new OpenCoverSettings(){
            LogLevel = OpenCoverLogLevel.Info,
			OldStyle = false,
			MergeOutput = true
        }     
        .WithFilter("+[*]*")
		.WithFilter("-[*.Tests*]*")
	    .WithFilter("-[*NUnit3.*]*")
		);

        }
		    
     }
	  if(parameters.IsLocalBuild){
	   ReportGenerator(parameters.Paths.Files.TestCoverageOutputFilePath,parameters.Paths.Directories.TestCoverageOutput + "/" + "htmlreports");
	  }

});