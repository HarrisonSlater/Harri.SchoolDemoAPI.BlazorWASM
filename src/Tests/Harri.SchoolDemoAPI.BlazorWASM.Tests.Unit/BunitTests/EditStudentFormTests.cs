using AngleSharp.Dom;
using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Pages;
using Harri.SchoolDemoApi.Client;
using Harri.SchoolDemoAPI.Models.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using MudBlazor.Services;
using System.Threading.Tasks;
using Harri.SchoolDemoAPI.BlazorWASM.Components;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit.BunitTests
{
    /// <summary>
    /// These tests are written entirely in C#.
    /// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
    /// </summary>
    [TestFixture]
    public class EditStudentFormTests : BunitTestContext
    {
        private const string NameInputSelector = "#student-name";
        private const string GpaInputSelector = "#student-gpa";
        private const string SubmitButtonSelector = "#submit-button";

        private const string ErrorAlertSelector = "#student-error-alert";
        private const string ErrorInputsSelector = ".mud-input-control.mud-input-error";

        public const string DeleteButton = "#delete-student-button";
        public const string DeleteDialogCancelButton = "#dialog-cancel";
        public const string DeleteDialogButton = "#dialog-delete";

        public const string DeleteDialog = ".mud-dialog";

        private Mock<IStudentApiClient> _mockStudentApiClient = new Mock<IStudentApiClient>();

        [SetUp]
        public void SetUp()
        {
            _mockStudentApiClient = new Mock<IStudentApiClient>();
            Services.AddSingleton(_mockStudentApiClient.Object);
            Services.AddMudServices();

            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.initialize", _ => true);
        }

        private StudentDto SetUpMockExistingStudent()
        {
            var mockExistingStudent = new StudentDto()
            {
                SId = 1,
                Name = "Test Existing Student",
                GPA = 2.99m
            };
            _mockStudentApiClient.Setup(client => client.GetStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((StudentDto?)mockExistingStudent));

            return mockExistingStudent;
        }

        // New Student
        [Test]
        public async Task EditStudent_ForNewStudent_SubmitsSuccessfully()
        {
            var editStudentForm = RenderComponent<EditStudentForm>();

            var textField = editStudentForm.Find(NameInputSelector);
            textField.GetAttribute("value").Should().BeNullOrEmpty();

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().BeNullOrEmpty();

            textField.Change("Test Name");

            await editStudentForm.FindAndClickAsync(SubmitButtonSelector);

            _mockStudentApiClient.Verify(x => x.AddStudent(It.IsAny<NewStudentDto>()), Times.Once);
        }

        [TestCase("0.1")]
        [TestCase("0.99")]
        [TestCase("1")]
        [TestCase("3.55")]
        public async Task EditStudent_ForNewStudent_SubmitsSuccessfully_WithGPA(string gpa)
        {
            var editStudentForm = RenderComponent<EditStudentForm>();

            var textField = editStudentForm.Find(NameInputSelector);
            textField.GetAttribute("value").Should().BeNullOrEmpty();

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().BeNullOrEmpty();

            textField.Change("Test Name");
            gpaField.Change(gpa);

            await editStudentForm.FindAndClickAsync(SubmitButtonSelector);

            _mockStudentApiClient.Verify(x => x.AddStudent(It.IsAny<NewStudentDto>()), Times.Once);
        }

        [Test]
        public async Task EditStudent_ForNewStudent_ShowsErrorOnFail()
        {
            _mockStudentApiClient.Setup(client => client.AddStudent(It.IsAny<NewStudentDto>()))
                .Returns(Task.FromResult((int?)null));

            var editStudentForm = RenderComponent<EditStudentForm>();

            var textField = editStudentForm.Find(NameInputSelector);
            textField.GetAttribute("value").Should().BeNullOrEmpty();

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().BeNullOrEmpty();

            textField.Change("Test Name");

            await editStudentForm.FindAndClickAsync(SubmitButtonSelector);

            _mockStudentApiClient.Verify(x => x.AddStudent(It.IsAny<NewStudentDto>()), Times.Once);

            var errorAlert = editStudentForm.Find(ErrorAlertSelector);
            errorAlert.Should().NotBeNull();
        }
        
        [Test]
        public async Task EditStudent_ForNewStudent_ShowsValidationErrorForName()
        {
            var editStudentForm = RenderComponent<EditStudentForm>();

            var textField = editStudentForm.Find(NameInputSelector);
            textField.GetAttribute("value").Should().BeNullOrEmpty();

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().BeNullOrEmpty();

            //textField.Change("");
            await editStudentForm.FindAndClickAsync(SubmitButtonSelector);

            var errorInputContainer = editStudentForm.Find(ErrorInputsSelector);

            var errorText = errorInputContainer.LastChild?.TextContent;
            
            errorText.Should().NotBeNullOrWhiteSpace();

            _mockStudentApiClient.Verify(x => x.AddStudent(It.IsAny<NewStudentDto>()), Times.Never);
        }

        [TestCase("asdf")]
        [TestCase("0")]
        [TestCase("-1")]
        [TestCase("-3.55")]
        [TestCase("3.555")]
        public async Task EditStudent_ForNewStudent_ShowsValidationErrorForNameAndGPA(string gpa)
        {
            var editStudentForm = RenderComponent<EditStudentForm>();

            var textField = editStudentForm.Find(NameInputSelector);
            textField.GetAttribute("value").Should().BeNullOrEmpty();

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().BeNullOrEmpty();

            gpaField.Change(gpa);
            await editStudentForm.FindAndClickAsync(SubmitButtonSelector);

            var errorInputContainers = editStudentForm.FindAll(ErrorInputsSelector);

            errorInputContainers.Count.Should().Be(2);
            foreach (var errorInputContainer in errorInputContainers)
            {
                var errorText = errorInputContainer.LastChild?.TextContent;

                errorText.Should().NotBeNullOrWhiteSpace();
            }

            _mockStudentApiClient.Verify(x => x.AddStudent(It.IsAny<NewStudentDto>()), Times.Never);
        }

        // Existing Student
        [Test]
        public async Task EditStudent_ForExistingStudent_SubmitsSuccessfully()
        {
            // Arrange
            var mockExistingStudent = SetUpMockExistingStudent();
            _mockStudentApiClient.Setup(client => client.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>()))
                .Returns(Task.FromResult((bool?)true));

            var editStudentForm = RenderComponent<EditStudentForm>(parameters => parameters.Add(s => s.StudentId, 123));

            var textField = editStudentForm.Find(NameInputSelector);

            textField.GetAttribute("value").Should().Be(mockExistingStudent.Name);

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().Be(mockExistingStudent.GPA.ToString());

            // Act
            var updatedName = "Test Name Updated";
            textField.Change(updatedName);

            await editStudentForm.FindAndClickAsync(SubmitButtonSelector);

            _mockStudentApiClient.Verify(
                x => x.UpdateStudent(123, It.Is<UpdateStudentDto>(
                    dto => dto.Name == updatedName &&
                    dto.GPA == mockExistingStudent.GPA)
                ), Times.Once);
        }

        [TestCase(false)]
        [TestCase(null)]
        public async Task EditStudent_ForExistingStudent_ShowsErrorOnFailToUpdate(bool? updateStudentResponse)
        {
            // Arrange
            var mockExistingStudent = SetUpMockExistingStudent();

            _mockStudentApiClient.Setup(client => client.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>()))
                .Returns(Task.FromResult(updateStudentResponse));

            var editStudentForm = RenderComponent<EditStudentForm>(parameters => parameters.Add(s => s.StudentId, 123));

            var textField = editStudentForm.Find(NameInputSelector);

            textField.GetAttribute("value").Should().Be(mockExistingStudent.Name);

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().Be(mockExistingStudent.GPA.ToString());
            
            // Act
            var updatedName = "Test Name Updated";
            textField.Change(updatedName);

            await editStudentForm.FindAndClickAsync(SubmitButtonSelector);

            _mockStudentApiClient.Verify(
                x => x.UpdateStudent(123, It.Is<UpdateStudentDto>(
                    dto => dto.Name == updatedName &&
                    dto.GPA == mockExistingStudent.GPA)
                ), Times.Once);

            var errorAlert = editStudentForm.Find(ErrorAlertSelector);
            errorAlert.Should().NotBeNull();

        }

        [Test]
        public void EditStudent_ForExistingStudent_DoesNotSubmitWhenNoChangesMade()
        {
            // Arrange
            var mockExistingStudent = SetUpMockExistingStudent();

            var editStudentForm = RenderComponent<EditStudentForm>(parameters => parameters.Add(s => s.StudentId, 123));

            var textField = editStudentForm.Find(NameInputSelector);

            textField.GetAttribute("value").Should().Be(mockExistingStudent.Name);

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().Be(mockExistingStudent.GPA.ToString());

            // Act
            var button = editStudentForm.Find(SubmitButtonSelector);

            // Assert
            button.IsDisabled().Should().BeFalse();
            _mockStudentApiClient.Verify(x => x.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>()), Times.Never);
        }

        [Test]
        public void EditStudent_ForExistingStudent_RedirectsIfInvalidStudentId()
        {
            // Arrange
            _mockStudentApiClient.Setup(client => client.GetStudent(123))
                .Returns(Task.FromResult((StudentDto?)null));

            // Act
            var editStudentForm = RenderComponent<EditStudentForm>(parameters => parameters.Add(s => s.StudentId, 123));

            // Assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            navMan.Uri.Should().Be($"http://localhost/?{Constants.QueryString.InvalidStudentId}=123");
        }
    }
}
