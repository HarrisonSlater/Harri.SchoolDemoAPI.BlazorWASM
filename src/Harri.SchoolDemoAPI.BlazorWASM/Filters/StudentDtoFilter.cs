using Harri.SchoolDemoAPI.Models.Dto;

namespace Harri.SchoolDemoAPI.BlazorWASM.Filters
{
    public static class StudentDtoFilter
    {
        public static Func<StudentDto, bool> QuickFilterFor(string? searchString) => x =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;

            if (x.Name is not null && x.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.SId is not null && x.SId.Value.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.GPA is not null && x.GPA.Value.ToString().Contains(searchString))
                return true;

            return false;
        };
    }
}
