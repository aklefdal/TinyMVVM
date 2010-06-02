﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Modules;
using TinyBDD.Dsl.GivenWhenThen;
using TinyMVVM.Framework;
using Moq;
using TinyMVVM.Framework.Conventions;
using TinyMVVM.Framework.Services;

namespace TinyMVVM.Tests.Framework.TestContext
{
    public class ViewModelBaseContext : NUnitScenarioClass
    {
        protected static CustomViewModel viewModel;
    	protected static Mock<IViewModelConvention> conventionMock;

    	protected Context ClassThatImplments_ViewModelBase_is_created = () =>
    	{
    	    CustomViewModel.SharedNinjectModule = null;
    		viewModel = new CustomViewModel();
    	};

    	protected Context ConventionMock_is_created = () =>
    	{
    		conventionMock = new Mock<IViewModelConvention>();
    	};


        protected When ClassThatImplements_ViewModelBase_is_spawned = () =>
        {
            viewModel = new CustomViewModel();
        };

        public class CustomViewModel : ViewModelBase
        {
			public CustomViewModel()
			{
				ApplyDefaultConventions();
			}

        	public ReadOnlyCollection<IViewModelConvention> Conventions
        	{
				get { return AppliedConventions; }
        	}

            public void DescribeController(Type controllerType)
            {
                DescribeControllerToBeCreated(controllerType);
            }
        }

        protected class TestController
        {
            public ViewModelBase ViewModel { get; set; }
            public IBackgroundWorker BackgroundWorker { get; set; }

            public TestController(CustomViewModel customViewModel, IBackgroundWorker backgroundWorker)
            {
                ViewModel = customViewModel;
                BackgroundWorker = backgroundWorker;
            }
        }

        protected class AnotherController
        {
            public ViewModelBase ViewModel { get; set; }

            public AnotherController(CustomViewModel customViewModel)
            {
                ViewModel = customViewModel;
            }
        }

        protected class SharedModule : NinjectModule
        {
            public override void Load()
            {
                Kernel.Bind<IBackgroundWorker>().To<TinyMVVM.Framework.Services.Impl.BackgroundWorker>();
            }
        }
    }
}
