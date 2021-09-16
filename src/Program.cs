using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static System.Console;

[assembly: CLSCompliant(true)]

const int Success = 0;
const int Failure = 1;
const string ProjectReferenceItemType = "ProjectReference";

if (args.Length < 1)
{
    WriteLine("Usage: ConvertProjDepToProjRef solution-filename.sln");
    return Failure;
}

try
{
    var solutionPath = args[0];
    if (!Path.IsPathRooted(solutionPath))
    {
        solutionPath = Path.GetFullPath(solutionPath, Directory.GetCurrentDirectory());
    }

    var solutionFile = SolutionFile.Parse(solutionPath);
    WriteLine("Converting solution {0}", solutionPath);

    foreach (var project in solutionFile.ProjectsInOrder)
    {
        WriteLine("Analyzing project {0} ({1})", project.ProjectName, project.RelativePath);
        foreach (var dependencyGuid in project.Dependencies)
        {
            WriteLine(" Found project dependency to {0}", dependencyGuid);
            var dependedProject = solutionFile.ProjectsByGuid[dependencyGuid];

            WriteLine("  Name = {0}", dependedProject.ProjectName);
            WriteLine("  Absolute path = {0}", dependedProject.AbsolutePath);

            AddProjectReference(project.AbsolutePath, dependedProject);
        }
    }

    return Success;
}
catch (Exception e)
{
    WriteLine("Error: " + e.Message);
    return Failure;
}


void AddProjectReference(string projectPath, ProjectInSolution referencedProject)
{
    var project = ProjectRootElement.Open(projectPath);
    var basePath = Path.GetDirectoryName(projectPath);

    string projectRelativePath = Path.GetRelativePath(basePath!, referencedProject.AbsolutePath);

    if (HasProjectReference(project, projectRelativePath, basePath!))
    {
        WriteLine("  Project reference already exists, skipping");
        return;
    }

    List<KeyValuePair<string, string>> metadata = new();

    if (string.IsNullOrEmpty(project.Sdk))
    {
        WriteLine("  Classic project format (Sdk property not used): Project GUID needed");
        metadata.Add(new KeyValuePair<string, string>("Project", referencedProject.ProjectGuid.ToLower(CultureInfo.InvariantCulture)));

        if (Path.GetExtension(projectPath).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
        {
            metadata.Add(new KeyValuePair<string, string>("Name", referencedProject.ProjectName));
        }
    }
    if (IsReferenceOutputAssemblyNeeded(projectPath, referencedProject.AbsolutePath))
    {
        WriteLine("  Reference to mixed project types: <ReferenceOutputAssembly> needed");
        metadata.Add(new KeyValuePair<string, string>("ReferenceOutputAssembly", "false"));
    }
    _ = project.AddItem(ProjectReferenceItemType, projectRelativePath, metadata);

    project.Save();
    WriteLine("  Added as ProjectReference");
}

bool HasProjectReference(ProjectRootElement project, string projectRelativePath, string basePath)
{
    return project.Items.Any(item => item.ItemType == ProjectReferenceItemType && PathEquals(item.Include, projectRelativePath, basePath));
}

static bool PathEquals(string relativePath1, string relativePath2, string basePath)
{
    return Path.GetFullPath(relativePath1, basePath)
        .Equals(Path.GetFullPath(relativePath2, basePath), StringComparison.OrdinalIgnoreCase);
}

static bool IsReferenceOutputAssemblyNeeded(string path1, string path2)
{
    return Path.GetExtension(path1) != Path.GetExtension(path2);
}
