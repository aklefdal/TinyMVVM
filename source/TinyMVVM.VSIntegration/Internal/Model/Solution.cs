﻿using System.Collections.Generic;

namespace TinyMVVM.TinyMVVM_VSIntegration.Internal.Model
{
    /// <summary>
    /// Aggregated root. Represents a Visual Studio solution.
    /// </summary>
    public class Solution
    {
        internal Solution()
        {
            Projects = new List<Project>();
        }

        public virtual string Name { get; set; }
        public virtual List<Project> Projects { get; private set; }
        public virtual string Path { get; set; }

    }
}
