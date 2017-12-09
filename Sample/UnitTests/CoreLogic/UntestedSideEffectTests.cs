using CoreLogic;
using Xunit;

namespace UnitTests.CoreLogic
{
    public class UntestedSideEffectTests
    {
        [Fact]
        public void ShouldFailWhenGivenFalse()
        {
            Assert.Equal("FAIL", UntestedSideEffect.Foo(false));
        }

        [Fact]
        public void ShouldBeOkWhenGivenTrue()
        {
            Assert.Equal("OK", UntestedSideEffect.Foo(true));
        }
    }
}
