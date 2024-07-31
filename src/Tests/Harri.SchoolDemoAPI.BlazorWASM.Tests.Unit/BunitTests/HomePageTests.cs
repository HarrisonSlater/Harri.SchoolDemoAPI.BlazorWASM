using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Pages;
using Harri.SchoolDemoApi.Client;
using Harri.SchoolDemoAPI.Models.Dto;
using Moq;
using MudBlazor.Services;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit.BunitTests
{
    [TestFixture]
    public class HomePageTests : BunitTestContext
    {
        public const string ViewStudentsButton = "#home-shortcut-students";
        public const string CreateNewStudentButton = "#home-shortcut-students-new";
        public const string EditStudentIdInput = "#home-shortcut-edit-student-id";
        public const string EditStudentButton = "#home-shortcut-edit-student";

        public const string InvalidStudentErrorAlert = "#student-error-alert";

        private const string ErrorInputsSelector = ".mud-input-control.mud-input-error";

        public const string ErrorText = ".mud-input-helper-text";

        private Mock<IStudentApiClient> _mockStudentApiClient = new Mock<IStudentApiClient>();
        private StudentDto? _existingStudent;

        [SetUp]
        public void SetUp()
        {
            _mockStudentApiClient = new Mock<IStudentApiClient>();

            SetUpMockStudent();

            Services.AddSingleton(_mockStudentApiClient.Object);
            Services.AddMudServices();

            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.initialize", _ => true);
        }

        private void SetUpMockStudent()
        {
            _existingStudent = new StudentDto()
            {
                SId = 1,
                Name = "Test Existing Student 1",
                GPA = 1.11m
            };

            _mockStudentApiClient.Setup(client => client.GetStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((StudentDto?)_existingStudent));
        }

        [Test]
        public void HomePage_RendersSuccessfully()
        {
            // Arrange
            // Act
            var homePage = RenderComponent<Home>();

            // Assert
            homePage.WaitForElement(ViewStudentsButton);
            homePage.WaitForElement(CreateNewStudentButton);
            homePage.WaitForElement(EditStudentIdInput);
            homePage.WaitForElement(EditStudentButton);

            homePage.Invoking(x => x.Find(InvalidStudentErrorAlert)).Should().ThrowExactly<ElementNotFoundException>();
        }

        [TestCase("100")]
        [TestCase("1")]
        [TestCase("123456")]
        public async Task HomePage_EditStudentShortcutGoesToEditStudentPage(string studentId)
        {
            // Arrange
            _mockStudentApiClient.Setup(client => client.GetStudent(It.IsAny<int>()))
                .Returns(Task.FromResult(_existingStudent));
            var homePage = RenderComponent<Home>();

            // Act
            homePage.Find(EditStudentIdInput).Change(studentId);

            await homePage.FindAndClickAsync(EditStudentButton);

            // Assert
            homePage.Invoking(x => x.Find(ErrorInputsSelector)).Should().ThrowExactly<ElementNotFoundException>();
            homePage.Invoking(x => x.Find(InvalidStudentErrorAlert)).Should().ThrowExactly<ElementNotFoundException>();

            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.Uri.Should().EndWith($"/students/{studentId}");
        }


        [TestCase(null)]
        [TestCase("")]
        [TestCase("asdf")]
        [TestCase("-100")]
        [TestCase("0")]
        public async Task HomePage_ShowsErrorOnEditStudentIDInput(string? studentId)
        {
            // Arrange
            _mockStudentApiClient.Setup(client => client.GetStudent(It.IsAny<int>()))
                .Returns(Task.FromResult((StudentDto?)null));

            // Act
            var homePage = RenderComponent<Home>();

            if (studentId is not null)
            {
                homePage.Find(EditStudentIdInput).Change(studentId);
            }

            await homePage.FindAndClickAsync(EditStudentButton);

            // Assert
            var errorInputContainer = homePage.Find(ErrorInputsSelector);

            var errorText = errorInputContainer.LastChild?.TextContent;

            errorText.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void ViewStudents_RendersWithErrorAlert()
        {
            // Arrange
            var studentErrorId = 123;

            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.NavigateTo($"?{Constants.QueryString.InvalidStudentId}={studentErrorId}");

            // Act
            var homePage = RenderComponent<Home>();

            // Assert
            var errorAlert = homePage.Find(InvalidStudentErrorAlert);

            errorAlert.TextContent.Should().Contain(studentErrorId.ToString());
        }
    }
}
