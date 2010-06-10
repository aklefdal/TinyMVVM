﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Tests;
using TinyMVVM.TinyMVVM_VSIntegration.Internal.Model;

namespace TinyMVVM.VSIntegration.Tests.Internal.Model
{
    public class FolderSpecs
    {
        [TestFixture]
        public class When_not_added_to_a_Project : FolderTestScenario
        {
            [SetUp]
            public void Setup()
            {
                Given(Folder_is_created);
            }

            [Test]
            public void assure_Project_is_not_found()
            {
                When("Get Project");

                Then(() => folder.Project.ShouldBeNull());
            }
        }

        [TestFixture]
        public class When_check_if_SubFolder_exists : FolderTestScenario
        {
            [SetUp]
            public void Setup()
            {
                Given(Folder_is_created);
                And("it has a sub folder", () =>
                    folder.NewFolder("Views"));

                When("check if same sub Folder exists", () =>
                    result = folder.HasFolder("Views"));
            }

            [Test]
            public void assure_result_is_true()
            {
                Then(() => result.ShouldBeTrue());
            }
        }

        [TestFixture]
        public class When_get_sub_Folder_by_name : FolderTestScenario
        {
            [SetUp]
            public void Setup()
            {
                Given(Folder_is_created);
                And("it has a sub folder", () =>
                    folder.NewFolder("Views"));
            }

            [Test]
            public void assure_sub_Folder_is_returned()
            {
                When("get sub folder by name", () =>
                    subFolder = folder.GetSubFolder("Views"));

                Then(() => subFolder.ShouldNotBeNull());
            }

            [Test]
            public void assure_Exception_is_thrown_if_sub_folder_doesnt_exist()
            {
                When("get sub folder by name");

                Then(() =>
                    this.ShouldThrowException<ArgumentException>(() =>
                        folder.GetSubFolder("foobar"), ex =>
                            ex.Message.ShouldBe("Folder doesn't exists")));
            }
        }

        [TestFixture]
        public class When_find_Project_from_a_SubFolder : FolderTestScenario
        {
            [SetUp]
            public void Setup()
            {
                Given(Project_is_created);
                And("it has a sub folder", () =>
                    subSubFolder = project.NewFolder("Views").NewFolder("Login"));

                When("find Project from a SubFolder");
            }

            [Test]
            public void assure_Project_is_found()
            {
                Then(() => subSubFolder.Project.ShouldBe(project));
            }
        }

        public class FolderTestScenario : NUnitScenarioClass
        {
            protected static Folder folder;
            protected static bool result;
            protected static Folder subFolder;
            protected static Folder subSubFolder;
            protected static Project project;

            protected Context Project_is_created = () =>
            {
                project = new Project();
            };

            protected Context Folder_is_created = () =>
            {
                folder = new Folder();
            };
        }
    }
}