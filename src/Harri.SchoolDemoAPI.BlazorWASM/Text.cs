namespace Harri.SchoolDemoAPI.BlazorWASM
{
    public static class Text
    {
        public static class StudentsPage
        {
            public static string SuccessMessage(string id) => $"A new student with ID '{id}' was created successfully";

            public static string EditSuccessMessage(string id) => $"Student with ID '{id}' was updated successfully";

            public static string DeleteSuccessMessage(string id) => $"Student with ID '{id}' was deleted successfully";

            public static string InvalidStudentId(string id) => $"Student with ID '{id}' does not exist";

            public static string ErrorText => "Failed to retrieve students. Please refresh the page.";
            public static string SIdFilterErrorText => "Invalid SId. SId must be a number";

        }

        public static class EditStudentForm
        {
            public static string ErrorText(int? id) => id.HasValue ? "Failed to update student. Please try again" : "Failed to create new student. Please try again";

            public static string DisplayText(int? id) => id.HasValue ? "Edit Student" : "Create Student";

            public static string DeleteText(string? name, int? id) => $"Permanently delete student {name} with id \"{id}\"";

            public static string FailedToDelete = "Something went wrong. Failed to delete student. Please refresh the page.";
            public static string FailedToDeleteConflict = "Failed to delete student. This student has existing records. Please delete student applications first.";
        }
    }
}
