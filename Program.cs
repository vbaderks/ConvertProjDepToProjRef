using Microsoft.Build.Construction;
using System;
using System.IO;

namespace ConvertProjDepToProjRef
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: ConvertProjDepToProjRef solution-filename.sln");
                return 1;
            }

            try
            {
                var solutionPath = args[0];
                var solutionFile = SolutionFile.Parse(solutionPath);
                Console.WriteLine("Converting solution {0}", solutionPath);

                foreach (ProjectInSolution project in solutionFile.ProjectsInOrder)
                {
                    Console.WriteLine("Analyzing project {0} ({1})", project.ProjectName, project.RelativePath);
                    foreach (var dependencyGuid in project.Dependencies)
                    {
                        Console.WriteLine(" Found project dependency to {0}", dependencyGuid);
                        var dependedProject = solutionFile.ProjectsByGuid[dependencyGuid];

                        Console.WriteLine("  Name = {0}", dependedProject.ProjectName);
                        Console.WriteLine("  Absolute path = {0}", dependedProject.AbsolutePath);

                        AddProjectReference(project.AbsolutePath, dependedProject);
                    }
                }

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        static void AddProjectReference(string projectPath, ProjectInSolution referencedProject)
        {
            var project = ProjectRootElement.Open(projectPath);
            var basePath = Path.GetDirectoryName(projectPath);

            string projectRelativePath = Path.GetRelativePath(basePath!, referencedProject.AbsolutePath);

            if (HasProjectReference(project, projectRelativePath, basePath))
            {
                Console.WriteLine("  ProjectReference already exists, skipping");
                return;
            }

            project.AddItem("ProjectReference", projectRelativePath);
            project.Save();
            Console.WriteLine("  Added as ProjectReference");
        }

        static bool HasProjectReference(ProjectRootElement project, string projectRelativePath, string basePath)
        {
            foreach (var item in project.Items)
            {
                if (item.ItemType == "ProjectReference" && PathEquals(item.Include, projectRelativePath, basePath))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool PathEquals(string relativePath1, string relativePath2, string basePath)
        {
            return Path.GetFullPath(relativePath1, basePath)
                .Equals(Path.GetFullPath(relativePath2, basePath), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
