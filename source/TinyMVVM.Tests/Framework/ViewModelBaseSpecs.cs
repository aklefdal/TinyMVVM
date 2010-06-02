﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Modules;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Conventions;
using TinyMVVM.Framework.Services;
using TinyMVVM.Tests.Framework.TestContext;
using System.ComponentModel;
using Moq;

namespace TinyMVVM.Tests.Framework
{
	class ViewModelBaseSpecs
	{
		[TestFixture]
		public class When_spawned : ViewModelBaseContext
		{
			[SetUp]
			public void Setup()
			{
				When(ClassThatImplements_ViewModelBase_is_spawned);
			}

			[Test]
			public void assure_it_isa_INotifyPropertyChanged()
			{
				Then(() =>
					(viewModel is INotifyPropertyChanged).ShouldBeTrue());
			}

			[Test]
			public void assure_it_has_a_PropertyChangeRecorder()
			{
				Then(() =>
					 viewModel.PropertyChangeRecorder.ShouldNotBeNull());
			}

			[Test]
			public void assure_it_has_a_CommandStateChangeRecorder()
			{
				Then(() =>
					 viewModel.CmdStateChangeRecorder.ShouldNotBeNull());
			}

		    [Test]
		    public void assure_it_has_Controllers()
		    {
                Then(() => 
    		        viewModel.Controllers.ShouldNotBeNull());
		    }
		}

		[TestFixture]
		public class When_Default_conventions_are_applied : ViewModelBaseContext
		{
			[SetUp]
			public void Setup()
			{
				Given(ClassThatImplments_ViewModelBase_is_created);

				When("default conventions are applied (in constructor)");
			}

			[Test]
			public void assure_BindCommandsDelegatesToMethods_convention_is_applied()
			{
				Then(() =>
					 viewModel.Conventions.Where(c => c is BindCommandsDelegatesToMethods).Count().ShouldBe(1));
			}

			[Test]
			public void assure_InvokeOnInitialize_convention_is_applied()
			{
				Then(() =>
				     viewModel.Conventions.Where(c => c is InvokeOnInitialize).Count().ShouldBe(1));
			}
		}

		[TestFixture]
		public class When_applying_convention : ViewModelBaseContext
		{
			[SetUp]
			public void Setup()
			{
				Given(ClassThatImplments_ViewModelBase_is_created);

				When("applying convention");
			}

			[Test]
			public void assure_convention_args_is_validated()
			{
				Then(() =>
					this.ShouldThrowException<ArgumentNullException>(() =>
						viewModel.ApplyConvention(null)));
			}
		}

		[TestFixture]
		public class When_convention_is_applied : ViewModelBaseContext
		{
			[SetUp]
			public void Setup()
			{
				Given(ClassThatImplments_ViewModelBase_is_created);
				And(ConventionMock_is_created);

				When("convention is applied", () =>
					viewModel.ApplyConvention(conventionMock.Object));
			}

			[Test]
			public void assure_convention_is_applied()
			{
				Then(() =>
					conventionMock.Verify(c =>
						c.ApplyTo(It.Is<ViewModelBase>((vm) => vm == viewModel)), Times.Once()));
			}
		}

	    [TestFixture]
	    public class When_register_Controller : ViewModelBaseContext
	    {
	        [SetUp]
	        public void Setup()
	        {
                Given(ClassThatImplments_ViewModelBase_is_created);

	            When("describing Controller to be created");
	        }

	        [Test]
	        public void assure_Exception_is_thrown_if_dependencies_are_not_configured()
	        {
                Then(() =>
                    this.ShouldThrowException<ViewModelException>(() =>
                        viewModel.RegisterController<TestController>(), ex =>
                        {
                            ex.Message.ShouldBe("Dependencies for Controller was not found. Add dependencies using the SharedNinjectModule static property. See inner Exception for more info");
                            ex.InnerException.ShouldBeInstanceOfType<ActivationException>();
                        }));
	        }
	    }

	    [TestFixture]
	    public class When_controllers_is_registerd : ViewModelBaseContext
	    {
	        [SetUp]
	        public void Setup()
	        {
	            Given(ClassThatImplments_ViewModelBase_is_created);
	            And("Shared Ninject module is specified", () =>
	            {
	                CustomViewModel.SharedNinjectModule = new SharedModule();
	            });

                When("controller is described", () =>
                {
                    viewModel.RegisterController<TestController>();
                    viewModel.RegisterController<AnotherController>();
                });
	        }

	        [Test]
	        public void assure_Controllers_is_created()
	        {
                Then(() =>
                {
                    viewModel.Controllers.ShouldHave(2);
                    viewModel.Controllers.First().GetType().ShouldBe(typeof(TestController));
                    viewModel.Controllers.Last().GetType().ShouldBe(typeof(AnotherController));
                });
	        }

	        [Test]
	        public void assure_ViewModel_instance_is_injected_into_the_Controllers_constructor()
	        {
	            Then(() =>
	            {
	                var testController = viewModel.Controllers.First() as TestController;
	                var anotherController = viewModel.Controllers.Last() as AnotherController;
 
                    testController.ViewModel.ShouldBe(viewModel);
                    anotherController.ViewModel.ShouldBe(viewModel);
	            });
	        }

	        [Test]
	        public void assure_Services_defined_in_SharedNinjectModule_is_injected_to_the_Controllers_constructor()
	        {
                Then(() =>
                {
                    var testController = viewModel.Controllers.First() as TestController;
                    testController.BackgroundWorker.ShouldNotBeNull();
                });
	        }
	    }

	    [TestFixture]
	    public class When_ConfigureDependencies : ViewModelBaseContext
	    {
	        [SetUp]
	        public void Setup()
	        {
                Given(ClassThatImplments_ViewModelBase_is_created);

                When("configure dependencies", () =>
                {
                    viewModel.ConfigureDependencies(config =>
                    {
                        config.Bind<IBackgroundWorker>().To<CustomBackgroundWorker>();
                    });
                });
	        }

	        [Test]
	        public void assure_Dependencies_are_configured()
	        {
                //TODO: bad test - can't assert on anything
	            Then(() => { });
	        }
	    }

	    [TestFixture]
	    public class When_dependencies_are_configured : ViewModelBaseContext 
	    {
	        [SetUp]
	        public void Setup()
	        {
                Given(ClassThatImplments_ViewModelBase_is_created);
                And("dependencies are configured", () =>
                {
                    viewModel.ConfigureDependencies(config =>
                    {
                        config.Bind<IBackgroundWorker>().To<CustomBackgroundWorker>();
                    });
                });

	            When("register Controllers", () =>
	            {
	                viewModel.RegisterController<TestController>();
                    viewModel.RegisterController<AnotherController>();
                    viewModel.RegisterController<YetAnotherController>();
	            });
	        }

            [Test]
            public void assure_dependencies_are_injected_into_Controller()
            {
                Then(() =>
                    GetTestControllerInViewModel().BackgroundWorker.
                        ShouldBeInstanceOfType<CustomBackgroundWorker>());
            }

	        [Test]
	        public void assure_dependencies_are_not_shared_between_controllers_by_default()
	        {
	            Then(() =>
	            {
	                var testController = GetTestControllerInViewModel();
	                var yetAnotherController = GetYetAnotherControllerInViewModel();

                    testController.BackgroundWorker.ShouldNotBe(yetAnotherController.BackgroundWorker);
                });
	        }

	        [Test]
	        public void assure_dependencies_are_shared_between_controllers()
	        {
	            //TODO: add support
	        }

	    }

	    [TestFixture]
	    public class When_global_dependencies_are_configured : ViewModelBaseContext
	    {
	        [SetUp]
	        public void Setup()
	        {
	            Given(ClassThatImplments_ViewModelBase_is_created);
                And("another instnace of the same ViewModel is created", () =>
                    viewModel2 = new CustomViewModel());
	            And(yet_another_ViewModel_is_created);
                And("dependencies are configured", () =>
                {
                    CustomViewModel.ConfigureGlobalDependencies(config =>
                    {
                        config.Bind<IBackgroundWorker>().To<CustomBackgroundWorker>();
                    });
                    AnotherCustomViewModel.ConfigureGlobalDependencies(config =>
                    {
                        config.Bind<IBackgroundWorker>().To<CustomBackgroundWorker>();                                                                               
                    });
                });

	            When("register the same type Controllers on both of the ViewModels", () =>
	            {
	                viewModel.RegisterController<TestController>();
                    viewModel.RegisterController<AnotherController>();
                    viewModel.RegisterController<YetAnotherController>();

                    viewModel2.RegisterController<TestController>();
                    viewModel2.RegisterController<AnotherController>();
                    viewModel2.RegisterController<YetAnotherController>();

                    anotherViewModel.RegisterController<TestController>();
                });
	        }

	        [Test]
	        public void assure_dependencies_are_shared_between_viewModel_instances()
	        {
                Then(() =>
                {
                    var testControllerOnViewModel = viewModel.Controllers.First() as TestController;
                    var testControllerOnViewModel2 = viewModel2.Controllers.First() as TestController;

                    testControllerOnViewModel.BackgroundWorker.ShouldBe(testControllerOnViewModel2.BackgroundWorker);
                });
	        }

	        [Test]
            [Ignore("Will impl this functionality at a later stage")]
	        public void assure_dependencies_are_only_shared_between_viewmodels_of_same_type()
	        {
                Then(() =>
                {
                    var testControllerOnViewModel = viewModel.Controllers.First() as TestController;
                    var testControllerOnAnotherViewModel = anotherViewModel.Controllers.First() as TestController;

                    testControllerOnViewModel.BackgroundWorker.ShouldNotBe(testControllerOnAnotherViewModel.BackgroundWorker);
                });
	        }
	    }

	}
}
