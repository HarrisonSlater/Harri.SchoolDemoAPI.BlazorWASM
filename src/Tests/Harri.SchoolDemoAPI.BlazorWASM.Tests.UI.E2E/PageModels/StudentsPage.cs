using FluentAssertions;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;
using System.Text.RegularExpressions;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    public class StudentsPage
    {
        private readonly IPage _page;
        public IPage Page => _page;
        private readonly string _baseUrl = SchoolDemoAdminUrl;
        private const string SchoolDemoAdminUrl = "https://localhost:7144";

        public ILocator RowsDisplayedLocator => _page.Locator(".mud-table-pagination-select");
        public ILocator IdDataCellsLocator => _page.Locator("td[data-label=\"SId\"]");
        public ILocator NameDataCellsLocator => _page.Locator("td[data-label=\"Name\"]");
        public ILocator GPADataCellsLocator => _page.Locator("td[data-label=\"GPA\"]");


        public PaginationActions Pagination { get; set; }
        //public StudentsPage(IPage page)
        //{
        //    _page = page;
        //    Pagination = new PaginationActions(_page);
        //}

        // Constructor for specflow
        public StudentsPage(BrowserDriver browserDriver)
        {
            var browserTask = browserDriver.Current;
            browserTask.Wait();
            var pageTask = browserTask.Result.NewPageAsync();
            pageTask.Wait();
            _page = pageTask.Result;
            Pagination = new PaginationActions(_page);
        }

        public async Task<int> GetRowsDisplayed()
        { 
            var rowsDisplayed = await RowsDisplayedLocator.TextContentAsync();

            rowsDisplayed.Should().NotBeNull();

            var rowsDisplayedInt = int.Parse(rowsDisplayed!);
            return rowsDisplayedInt;
        }

        public async Task<IReadOnlyList<string>> AssertRowsAndGetCellData(ILocator locator)
        {
            await Assertions.Expect(locator).ToHaveCountAsync(await GetRowsDisplayed());
            var pageCellData = await locator.AllTextContentsAsync();
            pageCellData.Should().AllSatisfy(x => x.Should().NotBeEmpty());

            return pageCellData;
        }

        public async Task<IReadOnlyList<string>> GetCellData(ILocator locator)
        {
            var pageCellData = await locator.AllTextContentsAsync();
            pageCellData.Should().AllSatisfy(x => x.Should().NotBeEmpty());

            return pageCellData;
        }

        public async Task GoToHome()
        {
            await _page.GotoAsync(_baseUrl);
        }

        public async Task GoTo()
        {
            await _page.GotoAsync(_baseUrl + "/students");
        }

        public async Task NavigateTo()
        {
            var studentsNavLink = _page.GetByRole(AriaRole.Link, new() { Name = "Students" });

            await studentsNavLink.ClickAsync();

            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*students/page/1"));
        }
    }

    public class PaginationActions
    {
        private readonly IPage _page;

        public PaginationActions(IPage page)
        {
            _page = page;
        }

        public ILocator PaginationActionsLocator => _page.Locator(".mud-table-pagination-actions");

        public ILocator GoToFirstPageButton => PaginationActionsLocator.Locator("button").Nth(0);
        public ILocator PreviousPageButton => PaginationActionsLocator.Locator("button").Nth(1);
        public ILocator NextPageButton => PaginationActionsLocator.Locator("button").Nth(2);
        public ILocator GoToLastPageButton => PaginationActionsLocator.Locator("button").Nth(3);
    }
}
