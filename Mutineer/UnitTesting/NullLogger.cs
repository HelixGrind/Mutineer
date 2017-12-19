using Xunit;

namespace Mutineer.UnitTesting
{
    public class NullLogger : IRunnerLogger
    {
        public void LogMessage(StackFrameInfo stackFrame, string message) { }

        public void LogImportantMessage(StackFrameInfo stackFrame, string message) { }

        public void LogWarning(StackFrameInfo stackFrame, string message) { }

        public void LogError(StackFrameInfo stackFrame, string message) { }

        public object LockObject { get; } = new object();
    }
}
