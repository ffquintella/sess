using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using System.IO;

[CheckBuildProjectConfigurations]
class Build : NukeBuild
{

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath AppDirectory => RootDirectory / "artifacts/app";
    AbsolutePath DockerFile => RootDirectory / "Dockerfile";

    string ChangeLogFile => RootDirectory / "CHANGELOG.md";
    
    string[] Authors = { "Felipe F Quintella" };

    string Title = "SESS";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            //TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Logger.Info("Restoring Packages");
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            EnsureExistingDirectory(AppDirectory);
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetOutputDirectory(AppDirectory)
                .EnableNoRestore());
        });
    private Target Local_Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            Logger.Info("Publishing to artifacts...");
            EnsureExistingDirectory(AppDirectory);
            DotNetPublish(s => s
                .SetConfiguration(Configuration)
                .SetAuthors(Authors)
                .SetVersion(GitVersion.GetNormalizedFileVersion())
                .SetTitle(Title)
                .SetOutput(AppDirectory)
                .SetWorkingDirectory(RootDirectory)
                .SetProject(Solution)
            );
          
            //CopyFile(RootDirectory + "/sess/nLog.prod.config", AppDirectory + "/nlog.config", FileExistsPolicy.OverwriteIfNewer);

            string fileName = AppDirectory + "/version.txt";
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                sw.WriteLine(GitVersion.GetNormalizedFileVersion());
            }
            
        });
}
