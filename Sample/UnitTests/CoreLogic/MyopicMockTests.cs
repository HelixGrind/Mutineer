using CoreLogic;
using Moq;
using Xunit;

namespace UnitTests.CoreLogic
{
    public sealed class MyopicMockTests
    {
        private readonly Mock<ICollaborator> _mockCollaborator;

        public MyopicMockTests()
        {
            _mockCollaborator = new Mock<ICollaborator>();
            _mockCollaborator.Setup(x => x.PerformAction()).Returns("BAR");
        }

        [Fact]
        public void ShouldPerformActionWhenGivenTrue()
        {
            MyopicMock.Foo(_mockCollaborator.Object, true);
            _mockCollaborator.Verify(x => x.PerformAction(), Times.Exactly(1));
        }

        [Fact]
        public void ShouldNotPerformActionWhenGivenFalse()
        {
            MyopicMock.Foo(_mockCollaborator.Object, false);
            _mockCollaborator.Verify(x => x.PerformAction(), Times.Never);
        }
    }
}
