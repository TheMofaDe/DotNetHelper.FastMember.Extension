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
