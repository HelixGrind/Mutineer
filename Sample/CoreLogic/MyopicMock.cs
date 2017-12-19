namespace CoreLogic
{
    public static class MyopicMock
    {
        public static string Foo(ICollaborator c, bool b)
        {
            if (b)
            {
                return c.PerformAction();
            }

            return "FOO";
        }
    }

    public interface ICollaborator
    {
        string PerformAction();
    }
}
