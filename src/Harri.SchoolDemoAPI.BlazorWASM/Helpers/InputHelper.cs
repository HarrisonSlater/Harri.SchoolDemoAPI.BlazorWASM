namespace Harri.SchoolDemoAPI.BlazorWASM.Helpers
{
    public static class InputHelper
    {
        /// <returns>Null when unable to parse</returns>
        public static int? ParseInt(string str)
        {
            int i;
            if (int.TryParse(str, out i))
            {
                return i;
            }
            else
            {
                return null;
            }
        }
    }
}
