using CoreLogic;
using Xunit;

namespace UnitTests.CoreLogic
{
    public class AdditionTests
    {
        private const int Num1 = 13;
        private const int Num2 = 15;

        [Fact]
        public void Add_ReturnsSum()
        {
            Assert.Equal(Num1 + Num2, Addition.Add(Num1, Num2));
        }

        [Fact]
        public void Increment_ReturnsSum()
        {
            Assert.Equal(Num1 + 1, Addition.Increment(Num1));
        }
    }
}
