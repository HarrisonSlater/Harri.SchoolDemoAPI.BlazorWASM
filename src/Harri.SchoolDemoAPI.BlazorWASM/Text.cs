namespace Harri.SchoolDemoAPI.BlazorWASM
{
    public static class Text
    {
        public static class StudentsPage
        {
            public static string SuccessMessage(string id) => $"A new student with ID '{id}' was created successfully";

            public static string EditSuccessMessage(string id) => $"Student with ID '{id}' was updated successfully";

            public static string ErrorText => "Failed to retrieve students. Please refresh the page.";
        }

        public static class EditStudentForm
        {
            public static string ErrorText(int? id) => id.HasValue ? "Failed to update student. Please try again" : "Failed to create new student. Please try again";

            public static string DisplayText(int? id) => id.HasValue ? "Edit Student" : "Create Student";
        }
    }
}
