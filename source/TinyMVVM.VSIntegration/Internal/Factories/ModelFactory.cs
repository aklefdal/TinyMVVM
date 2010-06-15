﻿using System;
using TinyMVVM.TinyMVVM_VSIntegration.Internal.Model;

namespace TinyMVVM.TinyMVVM_VSIntegration.Internal.Factories
{
    public class ModelFactory : IModelFactory
    {
        public Solution NewSolution()
        {
            return new Solution();
        }

        public Project NewProject()
        {
            return new Project();
        }

        public Folder NewFolder()
        {
            return new Folder();
        }

        public File NewFile()
        {
            return new File();
        }
    }
}