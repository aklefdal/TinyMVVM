﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using TinyMVVM.SemanticModel.MVVM;
using TinyMVVM.Tests.SemanticModel.MVVM.TestContext;

namespace TinyMVVM.Tests.SemanticModel.MVVM.ViewModelSpecs
{
    [TestFixture]
    public class When_spawning : ViewModelContext
    {
        [SetUp]
        public void Setup()
        {
            When("spawning");
        }

        [Test]
        public void assure_Name_arg_is_validated()
        {
            Then(() =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new ViewModel(null)));
        }
    }
        
    [TestFixture]
    public class When_spawned : ViewModelContext
    {
        [SetUp]
        public void Setup()
        {
            When(ViewModel_is_spawned);
        }

        [Test]
        public void assure_it_has_a_Name()
        {
            Then(() =>
                 viewModel.Name.ShouldBe("LoginViewModel"));
        }   

        [Test]
        public void assure_it_Data()
        {
            Then(() =>
                 viewModel.Properties.ShouldNotBeNull());
        }

        [Test]
        public void assure_it_has_Commands()
        {
            Then(() =>
                 viewModel.Commands.ShouldNotBeNull());
        }

        [Test]
        public void assure_it_has_a_Parent()
        {
            Then(() =>
                 viewModel.Parent.ShouldBe("TinyMVVM.Framework.ViewModelBase"));
        }

    }

    [TestFixture]
    public class When_adding_ViewModelData : ViewModelContext
    {
        [SetUp]
        public void Setup()
        {
            Given(ViewModel_is_created);
        }

        [Test]
        public void assure_Data_arg_is_validated()
        {
            When("adding ViewModelData");

            Then(() =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    viewModel.AddProperty(null)));
        }

        [Test]
        public void assure_its_added()
        {
            var viewModelData = new ViewModelProperty("Username", typeof(string).Name, false);

            When("adding ViewModelData", () =>
                viewModel.AddProperty(viewModelData));

            Then(() =>
                 viewModel.Properties.ShouldContain(viewModelData));
        }
    }

    [TestFixture]
    public class When_adding_a_ViewModelCommand : ViewModelContext
    {
        [SetUp]
        public void Setup()
        {
            Given(ViewModel_is_created);
        }

        [Test]
        public void assure_Command_arg_is_validated()
        {
            When("adding a ViewModelCommand");

            Then(() =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    viewModel.AddCommand(null)));
        }

        [Test]
        public void assure_its_added()
        {
            var command = new ViewModelCommand("Login");

            When("adding a ViewModelCommand", () =>
                viewModel.AddCommand(command));

            Then(() =>
                 viewModel.Commands.ShouldContain(command));
        }
    }

    [TestFixture]
    public class When_setting_Parent : ViewModelContext
    {
        [SetUp]
        public void Setup()
        {
            Given(ViewModel_is_created);

            When("setting Parent");
        }

        [Test]
        public void assure_Value_is_validated()
        {
            Then(() =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    viewModel.Parent = null));
        }

    }

}
