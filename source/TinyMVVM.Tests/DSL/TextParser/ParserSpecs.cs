﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using TinyMVVM.DSL.TextParser;
using TinyMVVM.Tests.DSL.TextParser.TestContext;
using Moq;
using System.IO;

namespace TinyMVVM.Tests.DSL.TextParser.ParserSpecs
{
    [TestFixture]
    public class When_spawning_with_specific_LexicalAnalyzer : ParserContext
    {
        [SetUp]
        public void Setup()
        {
            When("spawning with specific LexicalAnalyzer");
        }

        [Test]
        public void assure_scanner_arg_is_validated()
        {
            Then(() =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new Parser(null)));
        }
    }

    [TestFixture]
    public class When_Parse : ParserContext
    {
        [SetUp]
        public void Setup()
        {
            Given("a simple viewmodel is described with the MVVM dsl", () =>
            {
                code = "viewmodel LoginViewModel:\n" +
                       "\tproperty Username as string\n\r" +
                       "\tproperty Password as string\n" +
                       "\t\tcommand Login\n" +
                       "\tcommand Cancel\n" +
                       "" +
                       "viewmodel Search:\n" +
                       "\tcommand Search" +
                       "\toproperty Query as string\n";
            });
            And(Parser_is_created);

            When("parse", () =>
                semanticModel = parser.Parse(new InlineCode(code)));
        }

        [Test]
        public void assure_ViewModels_are_parsed()
        {
            Then(() =>
            {
                semanticModel.ViewModels.ShouldHave(2);
            });
        }

        [Test]
        public void assure_ViewModel_Name_is_parsed()
        {
            Then(() =>
            {
                semanticModel.ViewModels[0].Name.ShouldBe("LoginViewModel");
                semanticModel.ViewModels[1].Name.ShouldBe("Search");
            });
        }

        [Test]
        public void assure_ViewModel_Properties_is_parsed()
        {
            Then(() =>
            {
                var vm = semanticModel.ViewModels.First();
                vm.Properties[0].Name.ShouldBe("Username");
                vm.Properties[0].Type.ShouldBe(typeof(string));
                vm.Properties[1].Name.ShouldBe("Password");
                vm.Properties[1].Type.ShouldBe(typeof(string));

                var vmSearch = semanticModel.ViewModels[1];
                vmSearch.Properties[0].Name.ShouldBe("Query");
            });
        }

        [Test]
        public void assure_ViewModelCommand_is_parsed()
        {
            Then(() =>
            {
                var vm = semanticModel.ViewModels.First();
                vm.Commands[0].Name.ShouldBe("Login");
                vm.Commands[1].Name.ShouldBe("Cancel");

                var vmSearch = semanticModel.ViewModels[1];
                vmSearch.Commands[0].Name.ShouldBe("Search");
            });
        }
    }

    [TestFixture]
    public class When_Parse_and_checking_grammar : ParserContext
    {
        [SetUp]
        public void Setup()
        {
            Given(Parser_is_created);
        }

        [Test]
        public void assure_it_checks_if_Type_is_specified_for_property()
        {
            And("dsl code is described", () =>
                code = "viewmodel Login:\n" +
                    "\tproperty Username");

            When("parse");

            Then(() =>
                this.ShouldThrowException<InvalidSyntaxException>(() =>
                    parser.Parse(new InlineCode(code)), ex =>
                        ex.Message.ShouldBe("Type must be specified for Property")));
        }

        [Test]
        public void assure_it_checks_if_Name_is_specified_for_property()
        {
            And("dsl code is described", () =>
                code = "viewmodel Login:\n" +
                    "\tproperty as string");

            When("parse");

            Then(() =>
                this.ShouldThrowException<InvalidSyntaxException>(() =>
                    parser.Parse(new InlineCode(code)), ex =>
                        ex.Message.ShouldBe("Name must be specified for Property")));
        }

        [Test]
        public void assure_it_checks_if_Name_is_specified_for_ViewModel()
        {
            And("dsl code is described", () =>
                code = "viewmodel:\n" +
                    "\tproperty as string");

            When("parse");

            Then(() =>
                this.ShouldThrowException<InvalidSyntaxException>(() =>
                    parser.Parse(new InlineCode(code)), ex =>
                        ex.Message.ShouldBe("Name must be specified for ViewModel")));
        }
    }

    [TestFixture]
    public class When_loading_Code : ParserContext
    {
        [SetUp]
        public void Setup()
        {
            Given(Parser_is_created);
        }

        [Test]
        public void assure_LoadingStrategy_arg_is_validated()
        {
            When("loading Code");

            Then(() =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    parser.Parse(null)));
        }

    }
}
