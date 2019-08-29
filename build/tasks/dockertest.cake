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
