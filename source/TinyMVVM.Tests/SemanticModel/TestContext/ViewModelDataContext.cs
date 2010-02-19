﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyBDD.Dsl.GivenWhenThen;
using TinyMVVM.SemanticModel;

namespace TinyMVVM.Tests.SemanticModel.TestContext
{
    public class ViewModelDataContext : NUnitScenarioClass
    {
        protected static ViewModelData viewModelData;

        protected When ViewModelData_is_spawned = () =>
        {
            viewModelData = new ViewModelData("Username", typeof(string));
        };
    }
}