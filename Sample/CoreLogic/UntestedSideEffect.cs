namespace CoreLogic
{
    public class UntestedSideEffect
    {
        public static string Foo(bool b)
        {
            if (b)
            {
                PerformVitallyImportantBusinessFunction();
                return "OK";
            }

            return "FAIL";
        }

        private static void PerformVitallyImportantBusinessFunction() {}
    }
}
