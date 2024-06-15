using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    /// <summary>
    /// Actions for interacting with the main nav sidebar and direct navigation via URL
    /// </summary>
    public class NavigationActions
    {
        private readonly string _baseUrl = SchoolDemoAdminUrl;
        public const string SchoolDemoAdminUrl = "https://localhost:7144/"; //TODO pull from settings

        private readonly IPage _page;

        private ILocator PageHeader => _page.Locator("header.mud-appbar .mud-toolbar");

        public NavigationActions(IPage page)
        {
            _page = page;
        }

        public async Task GoToHome()
        {
            await _page.GotoAsync(_baseUrl);
        }

        public async Task AssertPageLoaded()
        {
            await Assertions.Expect(PageHeader).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions() { Timeout = 60000}); //TODO Read app timeout from settings?
        }

        public async Task GoToStudentsPage()
        {
            await _page.GotoAsync(_baseUrl + "students");

            await AssertPageLoaded();

            await AssertStudentsPageUrlIsCorrect();
        }

        public async Task GoToCreateNewStudentPage()
        {
            await _page.GotoAsync(_baseUrl + "student/new");

            await AssertPageLoaded();

            await AssertCreateNewStudentPageUrlIsCorrect();
        }

        public async Task NavigateToStudentsPage()
        {
            var studentsNavLink = _page.GetByRole(AriaRole.Link, new() { Name = "Students" });

            await studentsNavLink.ClickAsync();

            await AssertStudentsPageUrlIsCorrect();
        }

        public async Task NavigateToCreateNewStudentPage()
        {
            var createNewStudentNavLink = _page.GetByRole(AriaRole.Link, new() { Name = "Create new student" }); //TODO not select by text

            await createNewStudentNavLink.ClickAsync();

            await AssertCreateNewStudentPageUrlIsCorrect();
        }

        public async Task AssertStudentsPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*students/page/1"), new PageAssertionsToHaveURLOptions() { Timeout= 10000 });
        }

        public async Task AssertCreateNewStudentPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*student/new"), new PageAssertionsToHaveURLOptions() { Timeout = 10000 });
        }

        public async Task AssertEditStudentPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*student/(\\d+)"), new PageAssertionsToHaveURLOptions() { Timeout = 10000 });
        }
    }
}
