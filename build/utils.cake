
FilePath FindToolInPath(string tool)
{
    var pathEnv = EnvironmentVariable("PATH");
    if (string.IsNullOrEmpty(pathEnv) || string.IsNullOrEmpty(tool)) return tool;

    var paths = pathEnv.Split(new []{ IsRunningOnUnix() ? ':' : ';'},  StringSplitOptions.RemoveEmptyEntries);
    var returnValue = paths.Select(path => new DirectoryPath(path).CombineWithFilePath(tool)).FirstOrDefault(filePath => FileExists(filePath.FullPath));
    return returnValue;
}

DirectoryPath HomePath()
{
    return IsRunningOnWindows()
        ? new DirectoryPath(EnvironmentVariable("HOMEDRIVE") +  EnvironmentVariable("HOMEPATH"))
        : new DirectoryPath(EnvironmentVariable("HOME"));
}

void ReplaceTextInFile(FilePath filePath, string oldValue, string newValue, bool encrypt = false)
{
    Information("Replacing {0} with {1} in {2}", oldValue, !encrypt ? newValue : "******", filePath);
    var file = filePath.FullPath.ToString();
    System.IO.File.WriteAllText(file, System.IO.File.ReadAllText(file).Replace(oldValue, newValue));
}

void SetRubyGemPushApiKey(string apiKey)
{
    // it's a hack, creating a credentials file to be able to push the gem
    var workDir = "./src/ApplicationNameRubyGem";
    var gemHomeDir = HomePath().Combine(".gem");
    var credentialFile = new FilePath(workDir + "/credentials");
    EnsureDirectoryExists(gemHomeDir);
    ReplaceTextInFile(credentialFile, "$api_key$", apiKey, true);
    CopyFileToDirectory(credentialFile, gemHomeDir);
}

GitVersion GetVersion(BuildParameters parameters)
{
    var dllFile = GetFiles($"{MyProject.ProjectDir}/bin/{parameters.Configuration}/{MyProject.TargetFrameworks.First()}/{MyProject.ProjectName}.dll").FirstOrDefault();
    var settings = new GitVersionSettings
    {
        OutputType = GitVersionOutput.Json,
    //    ToolPath = FindToolInPath(IsRunningOnUnix() ? "dotnet" : "dotnet.exe"),
        ArgumentCustomization = args => dllFile + " " + args.Render()
    };

    var gitVersion = GitVersion(settings);
    if (!parameters.IsLocalBuild && !(parameters.IsRunningOnAzurePipeline && parameters.IsPullRequest))
    {
        settings.UpdateAssemblyInfo = true;
        settings.LogFilePath = "console";
        settings.OutputType = GitVersionOutput.BuildServer;

        GitVersion(settings);
    }else{
        settings.UpdateAssemblyInfo = true;
	    GitVersion(settings);
    }
    return gitVersion;
}

void Build(string configuration,string solutionFile)
{
    DotNetCoreRestore(solutionFile); 
    MSBuild(solutionFile, settings =>
    {
        settings.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .WithTarget("Build")
            .WithProperty("POSIX", IsRunningOnUnix().ToString());
    });
}




void DockerBuild(DockerImage dockerImage, BuildParameters parameters)
{
    var (os, distro, targetframework) = dockerImage;
    var workDir = DirectoryPath.FromString($"./src/Docker/{os}/{distro}/{targetframework}");

    var sourceDir = parameters.Paths.Directories.ArtifactsBin.Combine(targetframework);


    CopyDirectory(sourceDir, workDir.Combine("content"));

    var tags = GetDockerTags(dockerImage, parameters);

    var buildSettings = new DockerImageBuildSettings
    {
        Rm = true,
        Tag = tags,
        File = $"{workDir}/Dockerfile",
        BuildArg = new []{ $"contentFolder=/content" },
        // Pull = true,
        // Platform = platform // TODO this one is not supported on docker versions < 18.02
    };

    DockerBuild(buildSettings, workDir.ToString());
}

void DockerPush(DockerImage dockerImage, BuildParameters parameters)
{
    var tags = GetDockerTags(dockerImage, parameters);

    foreach (var tag in tags)
    {
        DockerPush(tag);
    }
}

string DockerRunImage(DockerContainerRunSettings settings, string image, string command, params string[] args)
{
    if (string.IsNullOrEmpty(image))
    {
        throw new ArgumentNullException("image");
    }
    var runner = new GenericDockerRunner<DockerContainerRunSettings>(Context.FileSystem, Context.Environment, Context.ProcessRunner, Context.Tools);
    List<string> arguments = new List<string> { image };
    if (!string.IsNullOrEmpty(command))
    {
        arguments.Add(command);
        if (args.Length > 0)
        {
            arguments.AddRange(args);
        }
    }

    var result = runner.RunWithResult("run", settings ?? new DockerContainerRunSettings(), r => r.ToArray(), arguments.ToArray());
    return string.Join("\n", result);
}

void DockerTestRun(DockerContainerRunSettings settings, BuildParameters parameters, string image, string command, params string[] args)
{
    Information($"Testing image: {image}");
    var output = DockerRunImage(settings, image, command, args);

    var version = DeserializeJson<GitVersion>(output);

    Assert.Equal(parameters.Version.GitVersion.FullSemVer, version.FullSemVer);
}

string[] GetDockerTags(DockerImage dockerImage, BuildParameters parameters) {
    var name = $"{MyProject.RepositoryOwner}/{MyProject.RepositoryName}";
    var (os, distro, targetframework) = dockerImage;

    var tags = new List<string> {
        $"{name}:{parameters.Version.Version}-{os}-{distro}-{targetframework}",
        $"{name}:{parameters.Version.SemVersion}-{os}-{distro}-{targetframework}",
    };

    if (distro == "debian" && targetframework == "netcoreapp2.1" || distro == "nano") {
        tags.AddRange(new[] {
            $"{name}:{parameters.Version.Version}-{os}",
            $"{name}:{parameters.Version.SemVersion}-{os}",

            $"{name}:{parameters.Version.Version}-{targetframework}",
            $"{name}:{parameters.Version.SemVersion}-{targetframework}",

            $"{name}:{parameters.Version.Version}-{os}-{targetframework}",
            $"{name}:{parameters.Version.SemVersion}-{os}-{targetframework}",
        });

        if (parameters.IsStableRelease())
        {
            tags.AddRange(new[] {
                $"{name}:latest",
                $"{name}:latest-{os}",
                $"{name}:latest-{targetframework}",
                $"{name}:latest-{os}-{targetframework}",
                $"{name}:latest-{os}-{distro}-{targetframework}",
            });
        }
    }

    return tags.ToArray();
}

void GetReleaseNotes(FilePath outputPath, DirectoryPath workDir, string repoToken)
{
    var toolPath = Context.Tools.Resolve("GitReleaseNotes.exe");

    var arguments = new ProcessArgumentBuilder()
                .Append(workDir.ToString())
                .Append("/OutputFile")
                .Append(outputPath.ToString())
                .Append("/RepoToken")
                .Append(repoToken);

    StartProcess(toolPath, new ProcessSettings { Arguments = arguments, RedirectStandardOutput = true }, out var redirectedOutput);

    Information(string.Join("\n", redirectedOutput));
}

void UpdateTaskVersion(FilePath taskJsonPath, string taskId, GitVersion gitVersion)
{
    var taskJson = ParseJsonFromFile(taskJsonPath);
    taskJson["id"] = taskId;
    taskJson["version"]["Major"] = gitVersion.Major.ToString();
    taskJson["version"]["Minor"] = gitVersion.Minor.ToString();
    taskJson["version"]["Patch"] = gitVersion.Patch.ToString();
    SerializeJsonToPrettyFile(taskJsonPath, taskJson);
}
