// Install modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.3.0

// Install addins.
#addin "nuget:?package=Cake.Gitter&version=0.11.0"
#addin "nuget:?package=Cake.Docker&version=0.10.1"
#addin "nuget:?package=Cake.Npm&version=0.17.0"
#addin "nuget:?package=Cake.Incubator&version=5.1.0"
#addin "nuget:?package=Cake.Json&version=4.0.0"
#addin "nuget:?package=Cake.Tfx&version=0.9.0"
#addin "nuget:?package=Cake.Gem&version=0.8.0"
#addin "nuget:?package=Cake.Codecov&version=0.7.0"
#addin "nuget:?package=Cake.DocFx&version=0.13.0"
#addin "nuget:?package=Newtonsoft.Json&version=9.0.1"
#addin "nuget:?package=xunit.assert&version=2.4.1"



// Install tools.
#tool "nuget:?package=NUnit.ConsoleRunner&version=3.10.0"
#tool "nuget:?package=GitReleaseNotes&version=0.7.1"
#tool "nuget:?package=ILRepack&version=2.0.16"
#tool "nuget:?package=Codecov&version=1.7.0"
#tool "nuget:?package=nuget.commandline&version=4.9.2"
#tool "nuget:?package=GitVersion.CommandLine&version=5.0.2-beta1.2"
#tool "nuget:?package=docfx.console&version=2.44.0"
#tool "nuget:?package=WiX.Toolset.UnofficialFork&version=3.11.1"
#tool "nuget:?package=OpenCover&version=4.7.922"
#tool nuget:?package=ReportGenerator&version=4.2.15

// Install .NET Core Global tools.
#tool "dotnet:?package=GitReleaseManager.Tool&version=0.8.0"
#tool "dotnet:?package=dotnet-format&version=3.1.37601"

// Load other scripts.
#load "./build/parameters.cake"
#load "./build/utils.cake"

// Load build tasks
#load "./build/tasks/clean.cake"
#load "./build/tasks/build.cake"
#load "./build/tasks/test.cake"
#load "./build/tasks/generatedocs.cake"
#load "./build/tasks/packagemsi.cake"
#load "./build/tasks/copyfiles.cake"
#load "./build/tasks/packagegem.cake"
#load "./build/tasks/packagenuget.cake"
#load "./build/tasks/packagechocolatey.cake"
#load "./build/tasks/zipfiles.cake"
#load "./build/tasks/dockerbuild.cake"
#load "./build/tasks/dockertest.cake"
#load "./build/tasks/pack.cake"
#load "./build/tasks/releasenotes.cake"
#load "./build/tasks/publishcoverage.cake"
#load "./build/tasks/publishappveyor.cake"
#load "./build/tasks/publishazuredevops.cake"
#load "./build/tasks/publishdockerhub.cake"
#load "./build/tasks/publishnuget.cake"
#load "./build/tasks/publishchocolatey.cake"
#load "./build/tasks/publish.cake"
#load "./build/tasks/default.cake"
#load "./build/tasks/formatcode.cake"



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
// EXECUTION
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
RunTarget(target);