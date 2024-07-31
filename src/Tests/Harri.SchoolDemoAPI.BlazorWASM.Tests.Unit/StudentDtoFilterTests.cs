using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Filters;
using Harri.SchoolDemoAPI.Models.Dto;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit
{
    [TestFixture]
    public class StudentDtoFilterTests
    {
        [TestCase("Student", true)]
        [TestCase("Test Student", true)]
        [TestCase("Test ", true)]
        [TestCase("1", true)]
        [TestCase("101", true)]
        [TestCase("10110", true)]
        [TestCase("110", true)]
        [TestCase("4", true)]
        [TestCase("4.", true)]
        [TestCase("4.4", true)]
        [TestCase("4.42", true)]
        [TestCase(".42", true)]
        [TestCase("42", true)]
        [TestCase("", true)]
        [TestCase("", true)]
        [TestCase("Test Student 2", false)]
        [TestCase("1010", false)]
        [TestCase("10111", false)]
        [TestCase("4.43", false)]
        public void QuickFilterFor_ShouldFilterAsExpected(string searchString, bool expectedResult)
        {
            // Arrange
            var studentToFilter = new StudentDto()
            {
                Name = "Test Student",
                SId = 10110,
                GPA = 4.42m
            };

            // Act
            var filter = StudentDtoFilter.QuickFilterFor(searchString);

            var result = filter.Invoke(studentToFilter);
            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
