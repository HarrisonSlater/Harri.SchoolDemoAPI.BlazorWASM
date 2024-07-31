using AngleSharp.Dom;
using FluentAssertions;
using Harri.SchoolDemoApi.Client;
using Harri.SchoolDemoAPI.Models.Dto;
using Moq;
using MudBlazor.Services;
using System.Threading.Tasks;
using Harri.SchoolDemoAPI.BlazorWASM.Components;
using System.Linq;

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

            textField.Input("Test Name");

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

            textField.Input("Test Name");
            gpaField.Input(gpa);

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

            textField.Input("Test Name");

            await editStudentForm.FindAndClickAsync(SubmitButtonSelector);

            _mockStudentApiClient.Verify(x => x.AddStudent(It.IsAny<NewStudentDto>()), Times.Once);

            var errorAlert = editStudentForm.Find(ErrorAlertSelector);
            errorAlert.Should().NotBeNull();
        }
        
        [Test]
        public async Task EditStudent_ForNewStudent_ShowsValidationErrorForName()
        {
            // Arrange
            var editStudentForm = RenderComponent<EditStudentForm>();

            var textField = editStudentForm.Find(NameInputSelector);
            textField.GetAttribute("value").Should().BeNullOrEmpty();

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().BeNullOrEmpty();

            // Act
            await editStudentForm.FindAndClickAsync(SubmitButtonSelector);

            // Assert
            editStudentForm.Find(SubmitButtonSelector).IsDisabled().Should().Be(true);

            var errorInputContainer = editStudentForm.Find(ErrorInputsSelector);

            var errorText = errorInputContainer.LastChild?.TextContent;
            
            errorText.Should().NotBeNullOrWhiteSpace();

            _mockStudentApiClient.Verify(x => x.AddStudent(It.IsAny<NewStudentDto>()), Times.Never);
        }

        [TestCase("0")]
        [TestCase("-1")]
        [TestCase("-3.55")]
        [TestCase("3.555")]
        public void EditStudent_ForNewStudent_ShowsValidationErrorForGPAAndDisablesButton(string gpa)
        {
            var editStudentForm = RenderComponent<EditStudentForm>();

            var textField = editStudentForm.Find(NameInputSelector);
            textField.GetAttribute("value").Should().BeNullOrEmpty();

            var gpaField = editStudentForm.Find(GpaInputSelector);
            gpaField.GetAttribute("value").Should().BeNullOrEmpty();
            gpaField.Input(gpa);

            var button = editStudentForm.Find(SubmitButtonSelector);
            button.IsDisabled().Should().BeTrue();

            var errorInputContainers = editStudentForm.FindAll(ErrorInputsSelector);

            errorInputContainers.Count.Should().Be(1);
            var errorText = errorInputContainers.Single().LastChild?.TextContent;
            errorText.Should().NotBeNullOrWhiteSpace();

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
            textField.Input(updatedName);

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
            textField.Input(updatedName);


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
            button.IsDisabled().Should().BeTrue();
            gpaField.KeyDown(Key.Enter);

            // Assert
            button.IsDisabled().Should().BeTrue();
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
