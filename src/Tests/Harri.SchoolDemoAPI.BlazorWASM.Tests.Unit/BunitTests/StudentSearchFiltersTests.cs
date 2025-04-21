using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Components;
using Harri.SchoolDemoAPI.BlazorWASM.Filters;
using Harri.SchoolDemoAPI.BlazorWASM.Layout;
using Harri.SchoolDemoAPI.Models.Dto;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit.BunitTests
{
    [TestFixture]
    public class StudentSearchFiltersTests : BunitTestContext
    {
        private StudentSearchFilters _studentSearchFilters;

        [SetUp]
        public void SetUp()
        {
            //_mockStudentApiClient = new Mock<IStudentApi>();

            //Services.AddSingleton(_mockStudentApiClient.Object);

            //Services.AddMudServices();
            //JSInterop.Mode = JSRuntimeMode.Loose; // Ignore mudblazor JS calls

            //TestContext!.RenderTree.Add<MainLayout>();
        }

        private PropertyColumn<T1, T2> RenderPropertyColumn<T1, T2>(Expression e)
        {
            var c = RenderComponent<PropertyColumn<T1, T2>>(parameters => parameters.Add(p => p.Property, e)).Instance;

            return c;
        }

        [Test]
        public void StudentSearchFilters_ShouldSetFiltersCorrectly()
        {
            // Arrange
            var filterDefinitions = (ICollection<IFilterDefinition<StudentDto>>)new List<IFilterDefinition<StudentDto>>() {
                new FilterDefinition<StudentDto>() { Column = RenderPropertyColumn<StudentDto, int?>((StudentDto x) => x.SId)},
                new FilterDefinition<StudentDto>() { Column = RenderPropertyColumn<StudentDto, string>((StudentDto x) => x.Name)},
                new FilterDefinition<StudentDto>() { Column = RenderPropertyColumn<StudentDto, decimal?>((StudentDto x) => x.GPA)},
            };

            //var c = RenderComponent<PropertyColumn<StudentDto, decimal?>>(parameters => parameters.Add(p => p.Property, x => x.SId)).Instance;


            // Act
            _studentSearchFilters = new StudentSearchFilters(filterDefinitions);

            // Assert

            _studentSearchFilters.NameFilter.Should().NotBeNull();
            _studentSearchFilters.SIdFilter.Should().NotBeNull();
            _studentSearchFilters.GPAFilter.Should().NotBeNull();

            _studentSearchFilters.NameFilter.Column.PropertyName.Should().Be("Name");
            _studentSearchFilters.SIdFilter.Column.PropertyName.Should().Be("SId");
            _studentSearchFilters.GPAFilter.Column.PropertyName.Should().Be("GPA");
        }
    }
}
