Task("Clean")
    .Does<BuildParameters>((parameters) =>
{
    Information("Cleaning directories..");

    CleanDirectories($"{MyProject.ProjectDir}bin/" + parameters.Configuration);
    CleanDirectories($"{MyProject.ProjectDir}obj");
    CleanDirectories(parameters.Paths.Directories.ToClean);
});