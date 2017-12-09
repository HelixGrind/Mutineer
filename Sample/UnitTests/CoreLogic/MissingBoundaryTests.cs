using CoreLogic;
using Xunit;

namespace UnitTests.CoreLogic
{
    public sealed class MissingBoundaryTests
    {
        [Fact]
        public void ShouldReturnFooWhenGiven1()
        {
            Assert.Equal("foo", MissingBoundary.Foo(1));
        }

        [Fact]
        public void ShouldReturnBarWhenGivenMinus1()
        {
            Assert.Equal("bar", MissingBoundary.Foo(-1));
        }
    }
}
