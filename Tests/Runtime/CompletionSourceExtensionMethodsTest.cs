using NUnit.Framework;
using System.Collections.Generic;

namespace OrderedActionSequences.Tests
{
    class CompletionSourceExtensionMethodsTest
    {
        [Test]
        public void LinkAll_AllOriginalItemsAccountedFor()
        {
            CompletionSource parent1 = new CompletionSource();
            CompletionSource child1 = new CompletionSource();
            parent1.Link(child1);

            CompletionSource parent2 = new CompletionSource();
            CompletionSource child2 = new CompletionSource();
            parent2.Link(child2);

            CompletionSource root = new CompletionSource();

            root.LinkAll(parent1, parent2);

            List<ICompletionSource> sources = new List<ICompletionSource>
            {
                parent1, child1, parent2, child2
            };

            Assert.That(root.GetLinked(), Is.Not.Null);

            ICompletionSource source = root;
            int i = 0;
            while (source.GetLinked() != null)
            {
                Assert.That(sources[i].Equals(source.GetLinked()));

                source = source.GetLinked();

                ++i;
            }

            Assert.That(i == sources.Count);
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
