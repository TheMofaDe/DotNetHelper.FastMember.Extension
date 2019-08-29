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