using OrderedActionSequences;
using NUnit.Framework;

namespace Assets.Tests
{
    class CompletionSourceTest
    {
        [Test]
        public void CompletedToken_DistinctInstance()
        {
            ICompletionSource c1 = CompletionSource.Completed;
            ICompletionSource c2 = CompletionSource.Completed;

            Assert.That(c1, Is.Not.EqualTo(c2));
        }

        [Test]
        public void GetLinked_ReturnsCorrectInstance()
        {
            ICompletionSource c1 = CompletionSource.Completed;
            ICompletionSource c2 = CompletionSource.Completed;
            c1.Link(c2);

            Assert.That(c1.GetLinked().Equals(c2));
        }

        [Test]
        public void CompletedToken_DistinctInstance_LinkSuccess()
        {
            ICompletionSource c1 = CompletionSource.Completed;
            ICompletionSource c2 = CompletionSource.Completed;

            c1.Link(c2);

            Assert.That(c1.IsComplete, "This assertion may fail if the token is not marked completed or if the two linked tokens are the same instance and a stack overflow exception occurs.");
        }
    }
}
