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
