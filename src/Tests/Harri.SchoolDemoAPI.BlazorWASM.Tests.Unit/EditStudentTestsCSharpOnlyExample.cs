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

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit
{
    /// <summary>
    /// The same tests from EditStudentTests but written to test the model/c# code only and not assert on UI
    /// </summary>
    [TestFixture]
    public class EditStudentCSharpOnlyExampleTests : BunitTestContext
    {
        private Mock<IStudentApiClient> _mockStudentApiClient;

        [SetUp] 
        public void SetUp()
        {
            _mockStudentApiClient = new Mock<IStudentApiClient>();

            Services.AddSingleton(_mockStudentApiClient.Object);
            Services.AddMudServices();
            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
        }

        private StudentDto SetUpMockExistingStudent()
        {
            var mockExistingStudent = new StudentDto()
            {
                Name = "Test Existing Student",
                GPA = 2.99m
            };
            _mockStudentApiClient.Setup(client => client.GetStudent(123))
                .Returns(Task.FromResult((StudentDto?)mockExistingStudent));

            return mockExistingStudent;
        }

        // New Student
        [Test]
        public async Task EditStudent_ForNewStudent_SubmitsSuccessfully()
        {
            var editStudentPage = RenderComponent<EditStudent>().Instance;
            var formModel = editStudentPage.Student;

            formModel.Name.Should().BeNull();
            formModel.GPA.Should().BeNull();

            formModel.Name = "Test Student";

            await editStudentPage.HandleValidSubmit();

            _mockStudentApiClient.Verify(x => x.AddStudent(It.IsAny<NewStudentDto>()), Times.Once);
        }

        [Test]
        public async Task EditStudent_ForNewStudent_ShowsErrorOnFail()
        {
            _mockStudentApiClient.Setup(client => client.AddStudent(It.IsAny<NewStudentDto>()))
                .Returns(Task.FromResult((int?)null));

            var editStudentPage = RenderComponent<EditStudent>().Instance;
            var formModel = editStudentPage.Student;

            formModel.Name.Should().BeNull();
            formModel.GPA.Should().BeNull();

            formModel.Name = "Test Student";

            await editStudentPage.HandleValidSubmit();

            _mockStudentApiClient.Verify(x => x.AddStudent(It.IsAny<NewStudentDto>()), Times.Once);

            editStudentPage.ShowError.Should().BeTrue();
            editStudentPage.DisableSubmit.Should().BeFalse();
        }

        // Existing Student
        [Test]
        public async Task EditStudent_ForExistingStudent_SubmitsSuccessfully()
        {
            // Arrange
            var mockExistingStudent = SetUpMockExistingStudent();
            _mockStudentApiClient.Setup(client => client.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>()))
                .Returns(Task.FromResult((bool?)true));

            var editStudentPage = RenderComponent<EditStudent>(parameters => parameters.Add(s => s.StudentId, 123)).Instance;
            var formModel = editStudentPage.Student;

            formModel.Name.Should().Be(mockExistingStudent.Name);
            formModel.GPA.Should().Be(mockExistingStudent.GPA);

            // Act
            var updatedName = "Test Name Updated";
            formModel.Name = updatedName;

            await editStudentPage.HandleValidSubmit();

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

            var editStudentPage = RenderComponent<EditStudent>(parameters => parameters.Add(s => s.StudentId, 123)).Instance;
            var formModel = editStudentPage.Student;

            formModel.Name.Should().Be(mockExistingStudent.Name);
            formModel.GPA.Should().Be(mockExistingStudent.GPA);

            // Act
            var updatedName = "Test Name Updated";
            formModel.Name = updatedName;

            await editStudentPage.HandleValidSubmit();

            _mockStudentApiClient.Verify(
                x => x.UpdateStudent(123, It.Is<UpdateStudentDto>(
                    dto => dto.Name == updatedName &&
                    dto.GPA == mockExistingStudent.GPA)
                ), Times.Once);

            // Assert
            editStudentPage.ShowError.Should().BeTrue();
            editStudentPage.DisableSubmit.Should().BeFalse();

            editStudentPage.IsFormUnModified().Should().BeFalse();
        }

        [Test]
        public async Task EditStudent_ForExistingStudent_IsFormUnModified_ShouldBeTrue()
        {
            // Arrange
            var mockExistingStudent = SetUpMockExistingStudent();

            var editStudentPage = RenderComponent<EditStudent>(parameters => parameters.Add(s => s.StudentId, 123)).Instance;
            var formModel = editStudentPage.Student;

            formModel.Name.Should().Be(mockExistingStudent.Name);
            formModel.GPA.Should().Be(mockExistingStudent.GPA);

            // Act
            // Assert
            editStudentPage.IsFormUnModified().Should().BeTrue();
            _mockStudentApiClient.Verify(x => x.UpdateStudent(It.IsAny<int>(), It.IsAny<UpdateStudentDto>()), Times.Never);
        }

        [Test]
        public async Task EditStudent_ForExistingStudent_RedirectsIfInvalidStudentId()
        {
            // Arrange
            _mockStudentApiClient.Setup(client => client.GetStudent(123))
                .Returns(Task.FromResult((StudentDto?)null));

            // Act
            var editStudentPage = RenderComponent<EditStudent>(parameters => parameters.Add(s => s.StudentId, 123));

            // Assert
            var navMan = Services.GetRequiredService<FakeNavigationManager>();

            navMan.Uri.Should().EndWith("new");
        }
    }
}
