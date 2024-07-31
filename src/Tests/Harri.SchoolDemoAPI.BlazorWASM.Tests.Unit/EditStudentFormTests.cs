using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Components;
using Harri.SchoolDemoAPI.Models.Dto;
using System.Collections.Generic;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit
{
    [TestFixture]
    public class EditStudentFormTests
    {
        private EditStudentForm _editStudentForm;

        [SetUp]
        public void SetUp()
        {
            _editStudentForm = new EditStudentForm();
        }

        private static IEnumerable<TestCaseData> IsFormChangedTestCases()
        {
            // New student
            yield return new TestCaseData(new NewStudentDto(), null, true);
            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name" }, null, true);
            yield return new TestCaseData(new NewStudentDto() { GPA = 1.1m }, null, true);
            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name", GPA = 1.1m }, null, true);

            // Form Unchanged
            yield return new TestCaseData(new NewStudentDto(), new StudentDto(), false);
            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name" }, new StudentDto() { Name = "Test Name" }, false);
            yield return new TestCaseData(new NewStudentDto() { GPA = 1.1m }, new StudentDto() { GPA = 1.1m }, false);
            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name", GPA = 1.1m }, new StudentDto() { Name = "Test Name", GPA = 1.1m }, false);

            // Form Changed
            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name" }, new StudentDto() { Name = "Test Name Changed" }, true);
            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name Changed" }, new StudentDto() { Name = "Test Name" }, true);
            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name" }, new StudentDto() { }, true);
            yield return new TestCaseData(new NewStudentDto() { }, new StudentDto() { Name = "Test Name" }, true);

            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name", GPA = 1.1m }, new StudentDto() { Name = "Test Name Changed", GPA = 1.1m }, true);
            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name Changed", GPA = 1.1m }, new StudentDto() { Name = "Test Name", GPA = 1.1m }, true);


            yield return new TestCaseData(new NewStudentDto() { GPA = 1.1m }, new StudentDto() { GPA = 1.25m }, true);
            yield return new TestCaseData(new NewStudentDto() { GPA = 1.25m }, new StudentDto() { GPA = 1.1m }, true);
            yield return new TestCaseData(new NewStudentDto() { GPA = 1.1m }, new StudentDto() {  }, true);
            yield return new TestCaseData(new NewStudentDto() {  }, new StudentDto() { GPA = 1.1m }, true);

            yield return new TestCaseData(new NewStudentDto() { GPA = 1.1m, Name = "Test Name" }, new StudentDto() { GPA = 1.25m, Name = "Test Name" }, true);
            yield return new TestCaseData(new NewStudentDto() { GPA = 1.25m, Name = "Test Name" }, new StudentDto() { GPA = 1.1m, Name = "Test Name" }, true);


            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name", GPA = 1.1m }, new StudentDto() { Name = "Test Name Changed", GPA = 1.25m }, true);
            yield return new TestCaseData(new NewStudentDto() { Name = "Test Name Changed", GPA = 1.25m }, new StudentDto() { Name = "Test Name", GPA = 1.1m }, true);

            yield return new TestCaseData(new NewStudentDto() { }, new StudentDto() { Name = "Test Name Changed", GPA = 1.25m }, true);
        }

        [TestCaseSource(nameof(IsFormChangedTestCases))]
        public void IsFormChanged_ShouldReturnCorrectValue(NewStudentDto studentDto, StudentDto? existingStudentDto, bool expectedResult)
        {
            // Arrange
            _editStudentForm.Student = studentDto;
            _editStudentForm.ExistingStudent = existingStudentDto;

            // Act
            var isFormChanged = _editStudentForm.IsFormChanged();

            // Assert
            isFormChanged.Should().Be(expectedResult);
        }
    }
}
