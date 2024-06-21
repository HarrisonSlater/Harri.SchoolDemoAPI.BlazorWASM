using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    public class StudentsPage : BasePage
    {
        public ILocator RowsDisplayed => _page.Locator(".mud-table-pagination-select");

        public ILocator Rows => _page.Locator(".mud-table-body tr.mud-table-row");

        public ILocator IdDataCells => _page.Locator("td[data-label=\"SId\"]");
        public ILocator NameDataCells => _page.Locator("td[data-label=\"Name\"]");
        public ILocator GPADataCells => _page.Locator("td[data-label=\"GPA\"]");

        public ILocator StudentSearch => _page.Locator("#student-search");
        public ILocator StudentEditButton => _page.Locator(".student-edit-button");
        public ILocator StudentSuccessAlert => _page.Locator("#student-success-alert");
        public ILocator StudentDeleteAlert => _page.Locator("#student-delete-alert");

        public PaginationActions Pagination { get; set; }

        public StudentsPage(IPage page, SchoolDemoBaseUrlSetting baseUrlSetting, PlaywrightConfiguration config) : base(page, baseUrlSetting, config)
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
            var id = await row.Locator("td[data-label=\"SId\"]").TextContentAsync();
            var name = await row.Locator("td[data-label=\"Name\"]").TextContentAsync();
            var gpa = await row.Locator("td[data-label=\"GPA\"]").TextContentAsync();

            if (gpa == string.Empty) gpa = null;

            return (id, name, gpa);
        }

        public async Task SearchForStudent(string? searchString)
        {
            if (searchString is null) throw new ArgumentException($"{nameof(searchString)} cannot be null");

            await StudentSearch.FillAsync(searchString);
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
        
        public async Task<string?> GetDeleteAlert()
        {
            await Assertions.Expect(StudentDeleteAlert).ToBeVisibleAsync();
            return await StudentDeleteAlert.TextContentAsync();
        }

        //TODO refactor
        public async Task<string> GetSuccessAlertId()
        {
            var successAlertText = await GetSuccessAlert();

            if (successAlertText is null) throw new ArgumentException($"{nameof(successAlertText)} cannot be null");

            var idMatch = Regex.Match(successAlertText, "'(\\d+)'");
            var id = idMatch.Groups[1].Value;
            id.Should().NotBeNull();

            return id;
        }
        
        public async Task<string> GetDeleteAlertId()
        {
            var deleteAlertText = await GetDeleteAlert();

            if (deleteAlertText is null) throw new ArgumentException($"{nameof(deleteAlertText)} cannot be null");

            var idMatch = Regex.Match(deleteAlertText, "'(\\d+)'");
            var id = idMatch.Groups[1].Value;
            id.Should().NotBeNull();

            return id;
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
            await Assertions.Expect(rows).ToHaveCountAsync(rowsDisplayed + 2); // + 2 includes header row and footer

            await AssertRowsAndGetCellData(sIds);

            return await AssertRowsAndGetCellData(names);
        }
    }
}
