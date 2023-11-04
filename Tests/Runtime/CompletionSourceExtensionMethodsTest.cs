using Moq;
using NUnit.Framework;
using OrderedActionSequences;


namespace OrderedActionSequences.Tests
{
    class CompletionSourceExtensionMethodsTest
    {

        [Test]
        public void LinkAll_EverythingLinked()
        {
            Mock<ICompletionSource> parentMock = new Mock<ICompletionSource>(MockBehavior.Strict);

            Mock<ICompletionSource> mock1 = new Mock<ICompletionSource>(MockBehavior.Strict);
            Mock<ICompletionSource> mock2 = new Mock<ICompletionSource>(MockBehavior.Strict);
            Mock<ICompletionSource> mock3 = new Mock<ICompletionSource>(MockBehavior.Strict);

            mock1.Setup(m => m.Link(mock2.Object)).Verifiable();
            mock2.Setup(m => m.Link(mock3.Object)).Verifiable();
            parentMock.Setup(m => m.Link(mock1.Object)).Verifiable();

            parentMock.Object.LinkAll(mock1.Object, mock2.Object, mock3.Object);

            mock1.Verify(m => m.Link(mock2.Object), Times.Once());
            mock2.Verify(m => m.Link(mock3.Object), Times.Once());
            parentMock.Verify(m => m.Link(mock1.Object), Times.Once());
        }


        [Test]
        public void AllLinked_EverythingLinkedCorrectly_CompletedOutOfOrder()
        {
            CompletionSource parent = new CompletionSource();
            CompletionSource c1 = new CompletionSource();
            CompletionSource c2 = new CompletionSource();
            CompletionSource c3 = new CompletionSource();

            Assert.False(parent.IsComplete);

            parent.LinkAll(c1, c2, c3);

            parent.MarkCompleted();

            Assert.False(parent.IsComplete);

            c3.MarkCompleted();
           
            Assert.False(parent.IsComplete);

            c1.MarkCompleted();
          
            Assert.False(parent.IsComplete);

            c2.MarkCompleted();

            Assert.True(parent.IsComplete);
        }
    }
}
