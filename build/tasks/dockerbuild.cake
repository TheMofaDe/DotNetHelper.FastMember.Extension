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