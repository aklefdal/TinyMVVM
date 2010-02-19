﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TestGUI.Services;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Services.Impl;

namespace TestGUI.Tests.ViewModel.LoginViewModelSpecs
{
    [TestFixture]
    public class When_spawned : LoginViewModelContext
    {
        protected override void Context()
        {
            When_LoginViewModel_is_spawned();
        }

        [Test]
        public void assure_Login_Command_is_disabled()
        {
            viewModel.Login.CanExecute(null).ShouldBeFalse();
        }

        [Test]
        public void assure_Cancel_Command_is_disabled()
        {
            viewModel.Cancel.CanExecute(null).ShouldBeFalse();
        }

        [Test]
        public void assure_its_not_in_ReadOnly_state()
        {
            viewModel.ReadOnly.ShouldBeFalse();
        }
    }

    [TestFixture]
    public class When_Username_is_entered : LoginViewModelContext
    {
        protected override void  Context()
        {
            Given_LoginViewModel_is_created();

            When_Username_is_entered("goeran");
        }

        [Test]
        public void Then_assure_Cancel_Command_is_enabled()
        {
            viewModel.Cancel.CanExecute(null).ShouldBeTrue();
        }

        [Test]
        public void Then_assure_Login_Command_is_still_disabled()
        {
            viewModel.Login.CanExecute(null).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class When_Password_is_entered : LoginViewModelContext
    {
        protected override void Context()
        {
            Given_LoginViewModel_is_created();

            When_Password_is_entered("hansen");
        }

        [Test]
        public void Then_assure_Cancel_Command_is_enabled()
        {
            viewModel.Cancel.CanExecute(null).ShouldBeTrue();
        }

        [Test]
        public void Then_assure_Login_Command_is_still_disabled()
        {
            viewModel.Login.CanExecute(null).ShouldBeFalse();
        }
    }

    [TestFixture]
    public class When_Username_and_Password_is_entered : LoginViewModelContext
    {
        protected override void Context()
        {
            Given_LoginViewModel_is_created();
            And_Username_is_entered("goeran");

            When_Password_is_entered("hansen");
        }

        [Test]
        public void Then_assure_Login_Command_is_enabled()
        {
            viewModel.Login.CanExecute(null).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class When_Login : LoginViewModelContext
    {
        protected override void Context()
        {
            Given_LoginViewModel_is_created();
            And_Username_is_entered("goeran");
            And_Password_is_entered("hansen");

            When_execute_Login_Command();
        }

        [Test]
        public void Then_assure_AuthenticationService_is_called()
        {
            var authServiceFake = GetFakeFor<IAuthenticationService>();
            authServiceFake.Verify(s => 
                s.Authenticate(viewModel.Username, viewModel.Password), Times.Once());
        }

        [Test]
        public void Then_assure_Authentication_is_performed_on_a_Background_Thread()
        {
            var backgroundWorkerFake = GetFakeFor<IBackgroundWorker>();
            backgroundWorkerFake.Verify(s => 
                s.Invoke(It.IsAny<Action>()), Times.Once());
        }
    }

    [TestFixture]
    public class When_Login_with_invalid_credentials : LoginViewModelContext
    {
        protected override void Context()
        {
            var authFake = GetFakeFor<IAuthenticationService>();
            authFake.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).
                Returns(false);

            Given_LoginViewModel_is_created();
            And_Username_is_entered("goeran");
            And_Password_is_entered("notvalid");

            When_execute_Login_Command();
        }

        [Test]
        public void Then_assure_Status_is_set()
        {
            viewModel.Status.ShouldBe("Invalid credentials");
        }

        [Test]
        public void Then_assure_its_still_Visible()
        {
            viewModel.IsVisible.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class When_Login_with_valid_credentials : LoginViewModelContext
    {
        protected override void Context()
        {
            var authServiceFake = GetFakeFor<IAuthenticationService>();
            authServiceFake.Setup(s => 
                s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            Given_LoginViewModel_is_created();
            And_Username_is_entered("goeran");
            And_Password_is_entered("hansen");

            When_execute_Login_Command();
        }


        [Test]
        public void Then_assure_its_Invisible()
        {
            viewModel.IsVisible.ShouldBeFalse();
        }
    }

    [TestFixture]
    public class When_authenticating : LoginViewModelContext
    {
        protected override void Context()
        {
            Given_LoginViewModel_is_created();
            And_Username_is_entered("goeran");
            And_Password_is_entered("hansen");

            When_execute_Login_Command();
        }

        [Test]
        public void Then_assure_its_in_ReadOnly_state()
        {
            viewModel.ReadOnly.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class When_Cancel : LoginViewModelContext
    {
        protected override void Context()
        {
            Given_LoginViewModel_is_created();
            And_Username_is_entered("goeran");
            And_Password_is_entered("hansen");

            When_execute_Cancel_Command();
        }

        [Test]
        public void Then_assure_Username_is_cleared()
        {
            viewModel.Username.ShouldBeNull();
        }

        [Test]
        public void Then_assure_Password_is_cleared()
        {
            viewModel.Password.ShouldBeNull();
        }
    }
}