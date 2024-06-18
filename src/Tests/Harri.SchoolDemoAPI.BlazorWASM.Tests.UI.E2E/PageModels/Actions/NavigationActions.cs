using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks;
using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    /// <summary>
    /// Actions for interacting with the main nav sidebar and direct navigation via URL
    /// </summary>
    public class NavigationActions
    {
        private readonly SchoolDemoBaseUrlSetting _baseUrlSetting;
        public string BaseUrl => _baseUrlSetting.Value; //TODO pull from settings

        private readonly IPage _page;

        private ILocator PageHeader => _page.Locator("header.mud-appbar .mud-toolbar");

        public NavigationActions(IPage page, SchoolDemoBaseUrlSetting baseUrlSetting)
        {
            _baseUrlSetting = baseUrlSetting;
            _page = page;
        }

        public async Task GoToHome()
        {
            await _page.GotoAsync(BaseUrl);
        }

        public async Task AssertPageLoaded()
        {
            await Assertions.Expect(PageHeader).ToBeVisibleAsync();
        }

        public async Task GoToStudentsPage()
        {
            await _page.GotoAsync(BaseUrl + "students");

            await AssertPageLoaded();

            await AssertStudentsPageUrlIsCorrect();
        }

        public async Task GoToStudentsPage(int pageNumber)
        {
            await _page.GotoAsync(BaseUrl + $"students/page/{pageNumber}");

            await AssertPageLoaded();

            await AssertStudentsPageUrlIsCorrect();
        }

        public async Task GoToCreateNewStudentPage()
        {
            await _page.GotoAsync(BaseUrl + "students/new");

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
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*students/page/(\\d+)"), new PageAssertionsToHaveURLOptions()); // { Timeout= 10000 }
        }

        public async Task AssertCreateNewStudentPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*students/new"), new PageAssertionsToHaveURLOptions());
        }

        public async Task AssertEditStudentPageUrlIsCorrect()
        {
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*students/(\\d+)"), new PageAssertionsToHaveURLOptions());
        }
    }
}
