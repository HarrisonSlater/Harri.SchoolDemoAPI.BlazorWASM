using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks;
using System.Security.Policy;
using System.Web;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    public class StudentsPage : BasePage
    {
        public ILocator RowsDisplayed => _page.Locator(".mud-table-pagination-select");

        public ILocator TableLoading => _page.Locator("td.mud-table-loading");
        public ILocator Rows => _page.Locator(".mud-table-body tr.mud-table-row");

        public ILocator IdDataCells => _page.Locator("td[data-label=\"Student ID\"]");
        public ILocator NameDataCells => _page.Locator("td[data-label=\"Name\"]");
        public ILocator GPADataCells => _page.Locator("td[data-label=\"GPA\"]");

        public ILocator StudentSIdSearch => _page.Locator(".filter-input-sid input");
        public ILocator StudentSIdSearchClear => _page.Locator(".filter-input-sid button[aria-label=\"Clear Filter\"]");
        public ILocator StudentNameSearch => _page.Locator(".filter-input-student-name input");
        public ILocator StudentNameSearchClear => _page.Locator(".filter-input-student-name button[aria-label=\"Clear Filter\"]");

        public ILocator StudentGPASearch => _page.Locator(".filter-input-gpa.filter-header-cell");
        public ILocator StudentGPASearchInput => StudentGPASearch.Locator("input");
        public ILocator StudentGPASearchFilterDropDownButton => StudentGPASearch.Locator(".mud-menu button.mud-icon-button");
        public ILocator StudentGPASearchFilterDropDownItems => _page.Locator(".mud-popover-open .mud-list .mud-menu-item");


        public ILocator StudentGPASearchClear => _page.Locator(".filter-input-gpa button[aria-label=\"Clear Filter\"]");

        public ILocator StudentEditButton => _page.Locator(".student-edit-button");
        public ILocator StudentSuccessAlert => _page.Locator("#student-success-alert");
        public ILocator StudentEditSuccessAlert => _page.Locator("#student-edit-success-alert");
        public ILocator StudentDeleteAlert => _page.Locator("#student-delete-alert");

        public PaginationActions Pagination { get; set; }
        public AlertActions Alerts { get; set; }

        public StudentsPage(IPage page, SchoolDemoBaseUrlSetting baseUrlSetting, PlaywrightConfiguration config) : base(page, baseUrlSetting, config)
        {
            Pagination = new PaginationActions(page);
            Alerts = new AlertActions(page);
        }

        public async Task<int> GetRowsDisplayed()
        {
            var rowsDisplayed = await RowsDisplayed.TextContentAsync();

            rowsDisplayed.Should().NotBeNull();

            var rowsDisplayedInt = int.Parse(rowsDisplayed!);
            return rowsDisplayedInt;
        }

        public async Task<IReadOnlyList<string>> GetCellData(ILocator locator)
        {
            var pageCellData = await locator.AllTextContentsAsync();
            pageCellData.Should().AllSatisfy(x => x.Should().NotBeEmpty());

            return pageCellData;
        }

        public async Task<List<(string?, string?, string?)>> GetAllRowData()
        {
            var allRowData = new List<(string?, string?, string?)>();
            
            var allRows = await Rows.AllAsync();

            foreach(var locator in allRows)
            {
                var rowData = await GetRowData(locator);
                allRowData.Add(rowData);
            }

            return allRowData;
        }

        private async Task<(string?, string?, string?)> GetRowData(ILocator row)
        {
            var id = await row.Locator("td[data-label=\"Student ID\"]").TextContentAsync();
            var name = await row.Locator("td[data-label=\"Name\"]").TextContentAsync();
            var gpa = await row.Locator("td[data-label=\"GPA\"]").TextContentAsync();

            if (gpa == string.Empty) gpa = null;

            return (id, name, gpa);
        }

        public async Task SearchForStudentBySId(string? searchString)
        {
            if (searchString is null) throw new ArgumentException($"{nameof(searchString)} cannot be null");

            //TODO check hostname, validate this response is from the api and not this app itself
            await Page.RunAndWaitForResponseAsync(() => StudentSIdSearch.FillAsync(searchString), r => r.Url.Contains($"students/?sId={Uri.EscapeDataString(searchString)}"));

            //TODO test this when the api is slow
            await Assertions.Expect(TableLoading).Not.ToBeAttachedAsync();
        }

        public async Task SearchForStudentByName(string? searchString)
        {
            if (searchString is null) throw new ArgumentException($"{nameof(searchString)} cannot be null");

            await Page.RunAndWaitForResponseAsync(() => StudentNameSearch.FillAsync(searchString), r => r.Url.Contains($"students/?name={Uri.EscapeDataString(searchString)}"));

            await Assertions.Expect(TableLoading).Not.ToBeAttachedAsync();
        }

        public async Task SearchForStudentByGPA(string? searchString)
        {
            if (searchString is null) throw new ArgumentException($"{nameof(searchString)} cannot be null");

            await Page.RunAndWaitForResponseAsync(() => StudentGPASearchInput.FillAsync(searchString), r => r.Url.Contains($"students/?GPA.eq={Uri.EscapeDataString(searchString)}"));

            await Assertions.Expect(TableLoading).Not.ToBeAttachedAsync();
        }

        public async Task SearchForStudentByGPAGreaterThan(string? searchString)
        {
            if (searchString is null) throw new ArgumentException($"{nameof(searchString)} cannot be null");

            await StudentGPASearchFilterDropDownButton.ClickAsync();
            await StudentGPASearchFilterDropDownItems.Nth(1).ClickAsync(); //TODO find operator by text not index

            await Page.RunAndWaitForResponseAsync(() => StudentGPASearchInput.FillAsync(searchString), r => r.Url.Contains($"students/?GPA.gt={Uri.EscapeDataString(searchString)}"));

            await Assertions.Expect(TableLoading).Not.ToBeAttachedAsync();
        }

        public async Task SearchForStudentByGPALessThan(string? searchString)
        {
            if (searchString is null) throw new ArgumentException($"{nameof(searchString)} cannot be null");

            await StudentGPASearchFilterDropDownButton.ClickAsync();
            await StudentGPASearchFilterDropDownItems.Nth(2).ClickAsync(); //TODO find operator by text not index

            await Page.RunAndWaitForResponseAsync(() => StudentGPASearchInput.FillAsync(searchString), r => r.Url.Contains($"students/?GPA.lt={Uri.EscapeDataString(searchString)}"));

            await Assertions.Expect(TableLoading).Not.ToBeAttachedAsync();
        }

        public async Task ClearIdSearch()
        {
            await Page.RunAndWaitForResponseAsync(() => StudentSIdSearchClear.ClickAsync(), r => r.Url.Contains($"students/"));

            await Assertions.Expect(TableLoading).Not.ToBeAttachedAsync();
        }

        public async Task ClearNameSearch()
        {
            await Page.RunAndWaitForResponseAsync(() => StudentNameSearchClear.ClickAsync(), r => r.Url.Contains($"students/"));

            await Assertions.Expect(TableLoading).Not.ToBeAttachedAsync();
        }

        public async Task ClearGPASearch()
        {
            await Page.RunAndWaitForResponseAsync(() => StudentGPASearchClear.ClickAsync(), r => r.Url.Contains($"students/"));

            await Assertions.Expect(TableLoading).Not.ToBeAttachedAsync();
        }

        public async Task ClickEditOnStudent(string? studentId)
        {
            if (studentId is null) throw new ArgumentException($"{nameof(studentId)} cannot be null");

            var rows = await GetAllRowData();
            var index = rows.FindIndex(0, x => x.Item1 == (studentId));

            await StudentEditButton.Nth(index).ClickAsync();
        }

        public async Task<(string?, string?, string?)> ClickEditOnTheFirstStudent()
        {
            var firstStudent = StudentEditButton.First;
            var firstRow = (await GetAllRowData()).First();

            await firstStudent.ClickAsync();

            return firstRow;
        }

        public async Task<string?> GetSuccessAlert()
        {
            await Assertions.Expect(StudentSuccessAlert).ToBeVisibleAsync();
            return await StudentSuccessAlert.TextContentAsync();
        }
        
        public async Task<string?> GetEditSuccessAlert()
        {
            await Assertions.Expect(StudentEditSuccessAlert).ToBeVisibleAsync();
            return await StudentEditSuccessAlert.TextContentAsync();
        }
        
        public async Task<string?> GetDeleteAlert()
        {
            await Assertions.Expect(StudentDeleteAlert).ToBeVisibleAsync();
            return await StudentDeleteAlert.TextContentAsync();
        }

        public async Task<string> GetSuccessAlertId()
        {
            return Alerts.ExtractIdFromAlert(await GetSuccessAlert());
        }

        public async Task<string> GetEditSuccessAlertId()
        {
            return Alerts.ExtractIdFromAlert(await GetEditSuccessAlert());
        }

        public async Task<string> GetDeleteAlertId()
        {
            return Alerts.ExtractIdFromAlert(await GetDeleteAlert());
        }

        // Assertions
        public async Task<IReadOnlyList<string>> AssertRowsAndGetCellData(ILocator locator)
        {
            await Assertions.Expect(locator).ToHaveCountAsync(await GetRowsDisplayed());
            var pageCellData = await locator.AllTextContentsAsync();
            pageCellData.Should().AllSatisfy(x => x.Should().NotBeEmpty());

            return pageCellData;
        }

        public async Task AssertNoStudentRowsExist()
        {
            await Assertions.Expect(IdDataCells).ToHaveCountAsync(0);
            await Assertions.Expect(NameDataCells).ToHaveCountAsync(0);
        }

        public async Task AssertAtLeastOneStudentRowExists()
        {
            var sIds = (await GetCellData(IdDataCells));
            sIds.Should().HaveCountGreaterThan(0);

            var names = (await GetCellData(NameDataCells));
            names.Should().HaveCountGreaterThan(0);
        }

        public async Task AssertStudentPageIsVisible()
        {
            await AssertFullTableAndGetNames();

            await Navigation.AssertStudentsPageUrlIsCorrect();
        }

        public async Task<IReadOnlyList<string>> AssertFullTableAndGetNames()
        {
            var names = NameDataCells;
            var sIds = IdDataCells;
            var rowsDisplayed = await GetRowsDisplayed();

            var rows = Page.GetByRole(AriaRole.Row);

            await Assertions.Expect(rows).ToHaveCountAsync(rowsDisplayed + 2 + 1); // + 2 includes header row and footer. + 1 is for the filtering rows

            await AssertRowsAndGetCellData(sIds);

            return await AssertRowsAndGetCellData(names);
        }
    }
}
