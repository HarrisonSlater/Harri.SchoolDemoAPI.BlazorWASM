using Harri.SchoolDemoAPI.Models.Dto;
using MudBlazor;

namespace Harri.SchoolDemoAPI.BlazorWASM.Filters
{
    public class StudentSearchFilters
    {
        public IFilterDefinition<StudentDto> NameFilter { get; set; }
        public IFilterDefinition<StudentDto> SIdFilter { get; set; }
        public IFilterDefinition<StudentDto> GPAFilter { get; set; }

        public string? ParsedNameFilter { get; set; }
        public int? ParsedSIdFilter { get; set; }
        public decimal? ParsedGPAFilter { get; set; }

        public StudentSearchFilters(ICollection<IFilterDefinition<StudentDto>> filterDefinitions)
        {

            NameFilter = filterDefinitions.SingleOrDefault(x => x?.Column?.PropertyName == "Name");
            SIdFilter = filterDefinitions.SingleOrDefault(x => x?.Column?.PropertyName == "SId");
            GPAFilter = filterDefinitions.SingleOrDefault(x => x?.Column?.PropertyName == "GPA");

            ParsedNameFilter = (string?)NameFilter?.Value;
            ParsedSIdFilter = SIdFilter?.Value == null ? null : Convert.ToInt32((double?)SIdFilter?.Value); //All default MudBlazor filters treat any number as a double
            ParsedGPAFilter = GPAFilter?.Value == null ? null : Convert.ToDecimal((double?)GPAFilter?.Value); 
        }
    }
}
