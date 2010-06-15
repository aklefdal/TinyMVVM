﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using TinyMVVM.TinyMVVM_VSIntegration.Internal.Model;
using Project = TinyMVVM.TinyMVVM_VSIntegration.Internal.Model.Project;
using ProjectItem = EnvDTE.ProjectItem;
using Solution = TinyMVVM.TinyMVVM_VSIntegration.Internal.Model.Solution;

namespace TinyMVVM.TinyMVVM_VSIntegration.Internal.Factories
{
    internal class VsIntegrationModelFactory : IModelFactory
    {
        private DTE dte;

        public VsIntegrationModelFactory(EnvDTE.DTE dte)
        {
            this.dte = dte;
        }

        public Solution NewSolution()
        {
            var solution = new SolutionProxy();
            solution.VsSolution = dte.Solution;
            solution.Path = dte.Solution.FullName;

            for (int i = 1; i <= dte.Solution.Projects.Count; i++)
            {
                var vsProject = dte.Solution.Projects.Item(i);
                var project = NewProject();
                project.Path = vsProject.FullName;
                project.Name = vsProject.Name;
                solution.Projects.Add(project);

                for (int x = 1; x <= vsProject.ProjectItems.Count; x++)
                {
                    var item = vsProject.ProjectItems.Item(x);
                    if (item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
                    {
                        AddSubFolderInProject(project, item);
                    }
                }
            }

            return solution;
        }

        private void AddSubFolderInProject(Project project, ProjectItem vsItem)
        {
            var newFolder = NewFolder();
            newFolder.Name = vsItem.Name;
            
            project.Items.Add(newFolder);
                    
            if (vsItem.ProjectItems.Count > 0)
            {
                AddFilesAndFolders(newFolder, vsItem);
            }
        }

        private void AddFilesAndFolders(Folder folder, ProjectItem vsItem)
        {
            for (int x = 1; x <= vsItem.ProjectItems.Count; x++)
            {
                var item = vsItem.ProjectItems.Item(x);
                if (item.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFolder)
                {
                    var newFolder = NewFolder();
                    newFolder.Name = item.Name;
            
                    folder.Items.Add(newFolder);

                    if (item.ProjectItems.Count > 0)
                        AddFilesAndFolders(newFolder, item);
                }
            }
        }

        public Project NewProject()
        {
            return new ProjectProxy();
        }

        public Folder NewFolder()
        {
            return new FolderProxy();
        }

        public File NewFile()
        {
            return new FileProxy();
        }
    }

    public class SolutionProxy : Solution
    {
        public EnvDTE.Solution VsSolution { get; set; }
    }

    public class ProjectProxy : Project 
    {
        public EnvDTE.Project VsProject { get; set; }
    }

    public class FolderProxy : Folder
    {
        public EnvDTE.ProjectItem ProjectItem { get; set; }
    }

    public class FileProxy : File
    {
    }
}
