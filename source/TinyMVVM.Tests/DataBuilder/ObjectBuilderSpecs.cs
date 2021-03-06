﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using TinyMVVM.DataBuilder;
using TinyMVVM.SemanticModel.DataBuilder;
using TinyMVVM.Tests.DataBuilder.TestContext;

namespace TinyMVVM.Tests.DataBuilder
{
    public class ObjectBuilderSpecs
    {
        [TestFixture]
        public class When_build_a_list_that_contains_objects : DataBuilderTestContext
        {
            [SetUp]
            public void Setup()
            {
                Given(ObjectBuilder_is_created);

                And("part for list of objects is specified", () =>
                {
                    part = new Part(typeof(List<Customer>));
                	var customer1Part = new ValuePart(typeof (Customer));
                	customer1Part.AddPart(new PropertyPart("CEO", typeof (Employee)));
                	var employeesPart = new PropertyPart("Employees", typeof (ObservableCollection<Employee>));
                	var employeePart = new ValuePart(typeof (Employee));
					employeePart.AddPart(new PropertyPart("Boss", typeof(Employee)));

					employeesPart.AddPart(employeePart);
					customer1Part.AddPart(employeesPart);

					part.AddPart(customer1Part);
					part.AddPart(new ValuePart(typeof(Customer)));
                });

                When(build_object_graph);
            }

            [Test]
            public void assure_list_is_created()
            {
                Then(() => result.GetType().ShouldBe(typeof(List<Customer>)));
            }

            [Test]
            public void assure_values_are_added()
            {
                Then(() =>
                {
                    customers.Count.ShouldBe(2);
                });
            }

            [Test]
            public void assure_complex_object_values_are_built()
            {
                Then(() =>
                {
                    customers.First().CEO.ShouldNotBeNull();
                });
            }

        	[Test]
        	public void assure_complex_object_list_properties_are_built()
        	{
        		Then(() =>
        		{
					customers.First().Employees.ShouldNotBeNull();
        		});
        	}

        	[Test]
        	public void assure_complex_object_lists_values_are_built()
        	{
        		Then(() =>
        		{
        			var customer = customers.First();
					customer.Employees.ShouldNotBeNull();
					customer.Employees.First().Boss.ShouldNotBeNull();
        		});
        	}
        }

        [TestFixture]
        public class When_Build_complex_object : DataBuilderTestContext
        {
            [SetUp]
            public void Setup()
            {
                Given(ObjectBuilder_is_created);

				And("part for a complex object is specified", () =>
				{
					part = new Part(typeof(Customer));
					part.AddPart(new PropertyPart("CEO", typeof(Employee)));
					var employeesProperty = new PropertyPart("Employees", typeof (ObservableCollection<Employee>));
					part.AddPart(employeesProperty);

					employeesProperty.AddPart(new ValuePart(typeof(Employee)));
				});

				When(build_object_graph);
            }

            [Test]
            public void assure_complex_values_are_built()
            {
                Then(() =>
                {
                    result.GetType().ShouldBe(typeof (Customer));
                    var customer = result as Customer;
                    customer.CEO.ShouldNotBeNull();
                });
            }

        	[Test]
        	public void assure_list_properties_are_built()
        	{
        		Then(() =>
        		{
					customer.Employees.ShouldNotBeNull();
        		});
        	}

        	[Test]
        	public void assure_list_properties_values_are_built()
        	{
        		Then(() =>
        		{
					customer.Employees.ShouldHave(1);
        		});
        	}
        }
    }

	public class ObjectBuilder_usage_scenarios
	{
		[TestFixture]
		public class When_building_a_list_of_Customers_of_a_given_size : DataBuilderTestContext
		{
			[SetUp]
			public void Setup()
			{
				Given(ObjectBuilder_is_created);

				And("parts for lists of customers are specified", () =>
				{
					part = new Part(typeof(List<Customer>));
					var customerPart = new ValuePart(typeof (Customer));
					customerPart.Describe(m => m.Count = 10);
					customerPart.AddPart(new PropertyPart("Name", typeof(string))).
						Describe(m =>
						{
							m.Data.Add("HumanName", HumanNameOptions.Name);
						});

					var ceoPart = new PropertyPart("CEO", typeof (Employee));
					ceoPart.AddPart(new PropertyPart("Name", typeof (string))).
						Describe(metadata => 
							metadata.Data.Add("HumanName", HumanNameOptions.Name | HumanNameOptions.Surname));
					customerPart.AddPart(ceoPart);
					part.AddPart(customerPart);
				});

				When(build_object_graph);
			}

			[Test]
			public void assure_list_is_created()
			{
				Then(() =>
				{
					var listOfCustomers = result as IList;
					listOfCustomers.ShouldNotBeNull();
					listOfCustomers.ShouldHave(10);
				});
			}

			[Test]
			public void assure_Name_property_is_built_on_Customers_objects()
			{
				Then(() =>
				{
					foreach (var customer in customers)
					{
						customer.Name.ShouldNotBeNull();
						customer.Name.ShouldNotBe(string.Empty);
					}
				});
			}

			[Test]
			public void assure_atleast_80_percent_of_the_Customer_Names_are_unique()
			{
				Then(() =>
				{
					var distinctNames = customers.Select(c => c.Name).Distinct();
					var uniqueNames = ((double)distinctNames.Count() / customers.Count()) * 100;
					uniqueNames.ShouldBeGreaterThan(80);
				});
			}

			[Test]
			public void assure_CEO_property_is_built_on_Customers_objects()
			{
				Then(() =>
				{
					foreach (var customer in customers)
					{
						customer.CEO.ShouldNotBeNull();
					}
				});
			}

			[Test]
			public void assure_Name_property_is_built_on_CEO_objects()
			{
				Then(() =>
				{
					foreach (var customer in customers)
					{
						customer.CEO.Name.ShouldNotBeNull();
						customer.CEO.Name.ShouldNotBe(string.Empty);
					}
				});
			}

		    [Test]
		    public void assure_atleast_80_percent_of_the_CEO_Names_are_unique()
		    {
		        Then(() =>
		        {
		            var distinctNames = customers.Select(c => c.CEO.Name).Distinct();
		        	var uniqueNames = ((double)distinctNames.Count()/customers.Count())*100;
		        	uniqueNames.ShouldBeGreaterThan(80);
		        });
		    }
		}

		[TestFixture]
		public class When_building_a_list_of_female_names : DataBuilderTestContext
		{
			[SetUp]
			public void Setup()
			{
				Given(ObjectBuilder_is_created);
				And("recipe is specified", () =>
				{
					part = new Part(typeof(List<string>));
					part.AddPart(new ValuePart(typeof (string))).Describe(metadata =>
					{
						metadata.Count = 10;
						metadata.Data.Add("HumanName", HumanNameOptions.FemaleName);
					});
				});

				When("build object graph", () =>
					names = objectBuilder.Build(part) as List<string>);
			}

			[Test]
			public void assure_list_is_created()
			{
				Then(() => 
					names.ShouldNotBeNull());
			}

			[Test]
			public void assure_list_contains_data()
			{
				Then(() =>
					names.Where(n => n == null || n == string.Empty).Count().ShouldBe(0));
			}

			[Test]
			public void assure_the_list_contains_female_names()
			{
				Then(() =>
				{
					names.ForEach(name => 
						femaleNames.Contains(name).ShouldBeTrue());
				});
			}
		}

		[TestFixture]
		public class When_building_a_list_of_male_names : DataBuilderTestContext
		{
			[SetUp]
			public void Setup()
			{
				Given(ObjectBuilder_is_created);
				And("recipe is specified", () =>
				{
					part = new Part(typeof(List<string>));
					part.AddPart(new ValuePart(typeof (string))).Describe(metadata =>
					{
						metadata.Count = 10;
						metadata.Data.Add("HumanName", HumanNameOptions.MaleName);
					});
				});

				When("build object graph", () =>
					names = objectBuilder.Build(part) as List<string>);
			}

			[Test]
			public void assure_list_contains_data()
			{
				Then(() => names.Count.ShouldNotBe(0));
			}

			[Test]
			public void assure_list_contains_male_names()
			{
				names.ForEach(name =>
					maleNames.Contains(name).ShouldBeTrue());
			}
		}

		[TestFixture]
		public class When_building_a_list_of_names : DataBuilderTestContext
		{
			[SetUp]
			public void Setup()
			{
				Given(ObjectBuilder_is_created);
				And("recipe is specified", () =>
				{
					part = new Part(typeof (List<string>));
					part.AddPart(new ValuePart(typeof (string))).Describe(metadata =>
					{
						metadata.Count = 10;
						metadata.Data.Add("HumanName", HumanNameOptions.Name | HumanNameOptions.Surname);
					});
				});

				When("build object graph", () =>
					names = objectBuilder.Build(part) as List<string>);
			}

			[Test]
			public void assure_list_contains_FirstName_and_Surname()
			{
				Then(() =>
				{
					var namesWithFirstAndSurname = names.Where(NameContainsFirstAndSurname());
					namesWithFirstAndSurname.Count().ShouldBe(names.Count);
				});
			}

			private Func<string, bool> NameContainsFirstAndSurname()
			{
				const char whitespace = ' ';

				return n => n.Split(whitespace).Count() == 2 &&
				            n != string.Empty;
			}
		}

		[TestFixture]
		public class When_building_a_list_of_empty_strings : DataBuilderTestContext
		{
			[SetUp]
			public void Setup()
			{
				Given(ObjectBuilder_is_created);
				And("recipe is specified", () =>
				{
					part = new Part(typeof (List<string>));
					part.AddPart(new ValuePart(typeof(string))).Describe(metadata =>
					{
						metadata.Count = 100;
					});
				});

				When(build_object_graph);
			}

			[Test]
			public void assure_list_is_built()
			{
				Then(() => result.ShouldNotBeNull());
			}

			[Test]
			public void assure_list_contains_empty_strings()
			{
				Then(() =>
				{
					var strings = result as List<string>;
					strings.ShouldHaveMoreThan(0);
					strings.ForEach(str => 
						str.ShouldBe(string.Empty));
				});
			}
		}

		[TestFixture]
		public class When_building_a_list_of_lists_of_names : DataBuilderTestContext
		{
			[SetUp]
			public void Setup()
			{
				Given(ObjectBuilder_is_created);
				And("recipe is specified", () =>
				{
					part = new Part(typeof (List<List<string>>));
					part.AddPart(new ValuePart(typeof(List<string>))).Describe(metadata =>
					{
						metadata.Count = 10;
						metadata.Part.AddPart(new ValuePart(typeof (string))).Describe(m =>
						{
							m.Count = 10;
							m.Data.Add("HumanName", HumanNameOptions.Name);
						});
					});
				});

				When(build_object_graph);
			}

			[Test]
			public void assure_list_is_created()
			{
				Then(() => result.ShouldNotBeNull());
			}

			[Test]
			public void assure_list_contains_list_of_names()
			{
				Then(() =>
				{
					var listOfLists = result as List<List<string>>;
					listOfLists.ShouldHave(10);

					listOfLists.ForEach(names =>
					{
						names.ShouldHave(10);
						var emptyNames = names.Where(n => n == null || n == string.Empty);
						emptyNames.Count().ShouldBe(0);
					});
				});
			}
		}
	}
}
