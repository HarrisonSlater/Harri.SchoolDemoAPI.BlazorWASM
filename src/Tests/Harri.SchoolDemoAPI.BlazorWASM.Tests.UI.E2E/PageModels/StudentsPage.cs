using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    public class StudentsPage : BasePage
    {
        public ILocator RowsDisplayed => _page.Locator(".mud-table-pagination-select");
        public ILocator IdDataCells => _page.Locator("td[data-label=\"SId\"]");
        public ILocator NameDataCells => _page.Locator("td[data-label=\"Name\"]");
        public ILocator GPADataCells => _page.Locator("td[data-label=\"GPA\"]");

        public PaginationActions Pagination { get; set; }

        public StudentsPage(IPage page) : base(page)
        {
            Pagination = new PaginationActions(_page);
        }

        public async Task<int> GetRowsDisplayed()
        { 
            var rowsDisplayed = await RowsDisplayed.TextContentAsync();

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
    }
}
