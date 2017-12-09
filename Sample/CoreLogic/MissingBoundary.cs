namespace CoreLogic
{
    public static class MissingBoundary
    {
        public static string Foo(int i)
        {
            if (i >= 0)
            {
                return "foo";
            }

            return "bar";
        }
    }
}
