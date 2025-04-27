using Harri.SchoolDemoAPI.BlazorWASM.Filters;
using MudBlazor;

namespace Harri.SchoolDemoAPI.BlazorWASM
{
    public static class Constants
    {
        public static class QueryString
        {
            public const string CreateSuccessId = "successId";
            public const string EditSuccessId = "editSuccessId";
            public const string DeleteStudentId = "deleteId";

            public const string InvalidStudentId = "invalidStudentId";
        }

        public static class SearchFilters
        {
            public static class Students
            {
                public static readonly HashSet<string> GPAFilterOperators = new HashSet<string>()
                {
                    FilterOperator.Number.GreaterThan,
                    FilterOperator.Number.Equal,
                    FilterOperator.Number.LessThan,
                    FilterOperator.Number.Empty
                };

            }
        }

    }
}
