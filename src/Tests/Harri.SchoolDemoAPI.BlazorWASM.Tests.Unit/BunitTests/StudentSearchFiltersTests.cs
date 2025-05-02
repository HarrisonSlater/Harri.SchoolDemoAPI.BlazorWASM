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

        private PropertyColumn<T1, T2> RenderPropertyColumn<T1, T2>(Expression e)
        {
            return RenderComponent<PropertyColumn<T1, T2>>(parameters => parameters.Add(p => p.Property, e)).Instance;
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

        private static IEnumerable<TestCaseData> ShouldParseValuesTestCases()
        {
            // New student
            yield return new TestCaseData(null, null, null, null, null);
            yield return new TestCaseData("Test Student", null, null, null, null);
            yield return new TestCaseData("Test Student", 1d, 1, null, null);
            yield return new TestCaseData("Test Student", 123d, 123, null, null);
            yield return new TestCaseData("Test Student", 123d, 123, 3.89d, 3.89m);
        }

        [TestCaseSource(nameof(ShouldParseValuesTestCases))]
        public void StudentSearchFilters_ShouldParseValuesCorrectly(string? name, double? sid, int? expectedParsedSId, double? gpa, decimal? expectedParsedGPA)
        {
            // Arrange
            var filterDefinitions = (ICollection<IFilterDefinition<StudentDto>>)new List<IFilterDefinition<StudentDto>>() {
                new FilterDefinition<StudentDto>() { Value = name, Column = RenderPropertyColumn<StudentDto, string>((StudentDto x) => x.Name)},
                new FilterDefinition<StudentDto>() { Value = sid, Column = RenderPropertyColumn<StudentDto, int?>((StudentDto x) => x.SId)},
                new FilterDefinition<StudentDto>() { Value = gpa, Column = RenderPropertyColumn<StudentDto, decimal?>((StudentDto x) => x.GPA)},
            };

            // Act
            _studentSearchFilters = new StudentSearchFilters(filterDefinitions);

            // Assert

            _studentSearchFilters.NameFilter.Should().NotBeNull();
            _studentSearchFilters.SIdFilter.Should().NotBeNull();
            _studentSearchFilters.GPAFilter.Should().NotBeNull();

            _studentSearchFilters.ParsedNameFilter.Should().Be(name);
            _studentSearchFilters.ParsedSIdFilter.Should().Be(expectedParsedSId);
            _studentSearchFilters.ParsedGPAFilter.Should().Be(expectedParsedGPA);
        }
    }
}
