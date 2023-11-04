using Moq;
using NUnit.Framework;
using OrderedActionSequences;
using OrderedActionSequences.Executors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TestTools;

namespace Assets.Tests
{
    class OrderedActionSequenceExecutorTest
    {
        private class TestSequenceRunner : ISequenceRunner
        {
            public ICompletionSource Run(SequenceItem item)
            {
                return item.Action.OnStart();
            }
        }

        private static Mock<IActionSequence> SetupAction()
        {
            Mock<IActionSequence> sequence = new Mock<IActionSequence>(MockBehavior.Strict);
            sequence.Setup(s => s.OnStart()).Returns(CompletionSource.Completed).Verifiable();
            sequence.Setup(s => s.OnEnd()).Returns(CompletionSource.Completed).Verifiable();

            return sequence;
        }

        [UnityTest]
        public IEnumerator Execute()
        {
            Mock<ISequenceDataExtractor> extractor = new Mock<ISequenceDataExtractor>(MockBehavior.Strict);

            long orderID = 123;
            IOrderedSequence data = Mock.Of<IOrderedSequence>(o => o.OrderID == orderID);

            Mock<IActionSequence> sequence = SetupAction();

            SequenceItem item = new SequenceItem(data, sequence.Object);

            extractor.SetupGet(e => e.ActionSequences).Returns(() =>
            {
                return item.AsEnumerable().ToLookup(s => s.Data.OrderID);
            });

            extractor.SetupGet(e => e.RepeatingSequences).Returns(() =>
            {
                //empty intentionally for this test
                return Enumerable.Empty<SequenceItem>().ToLookup(r => r.Data.OrderID);
            });

            extractor.SetupGet(e => e.SyncedSequences).Returns(() =>
            {
                //empty intentionally for this test
                return Enumerable.Empty<SyncedSequenceItem>().ToLookup(r => r.Data.OrderID);
            });

            extractor.Setup(e => e.DistinctSequences).Returns(() =>
            {
                return new List<long>() { orderID };
            });

            OrderedActionSequenceExecutor exec = new OrderedActionSequenceExecutor(extractor.Object, new TestSequenceRunner());

            yield return exec.Execute();

            sequence.Verify(s => s.OnStart(), Times.Once(), "OnStart was not called correctly");
            sequence.Verify(s => s.OnEnd(), Times.Once(), "OnEnd was not called correctly");
        }

        [UnityTest]
        public IEnumerator Execute_WithRepeating()
        {
            Mock<ISequenceDataExtractor> extractor = new Mock<ISequenceDataExtractor>(MockBehavior.Strict);

            long orderID = 123;

            Mock<IActionSequence> sequence = SetupAction();
            IOrderedSequence data = Mock.Of<IOrderedSequence>(o => o.OrderID == orderID);
            SequenceItem item = new SequenceItem(data, sequence.Object);

            extractor.SetupGet(e => e.ActionSequences).Returns(() =>
            {
                return item.AsEnumerable().ToLookup(s => s.Data.OrderID);
            });

            extractor.SetupGet(e => e.SyncedSequences).Returns(() =>
            {
                //empty intentionally for this test
                return Enumerable.Empty<SyncedSequenceItem>().ToLookup(r => r.Data.OrderID);
            });

            Mock<IActionSequence> repeating = SetupAction();
            IOrderedSequence repData = Mock.Of<IOrderedSequence>(o => o.OrderID == orderID);
            SequenceItem repItem = new SequenceItem(repData, repeating.Object);

            extractor.SetupGet(e => e.RepeatingSequences).Returns(() =>
            {
                // return Enumerable.Empty<SequenceItem>().ToLookup(r => r.Data.OrderID);
                return repItem.AsEnumerable().ToLookup(r => r.Data.OrderID);
            });

            extractor.Setup(e => e.DistinctSequences).Returns(() =>
            {
                return new List<long>() { orderID };
            });

            OrderedActionSequenceExecutor exec = new OrderedActionSequenceExecutor(extractor.Object, new TestSequenceRunner());

            yield return exec.Execute();

            sequence.Verify(s => s.OnStart(), Times.Once(), "OnStart was not called correctly");
            sequence.Verify(s => s.OnEnd(), Times.Once(), "OnEnd was not called correctly");

            repeating.Verify(s => s.OnStart(), Times.Once(), "Repeating OnStart was not called correctly");
            repeating.Verify(s => s.OnEnd(), Times.Once(), "Repeating OnEnd was not called correctly");
        }

        [UnityTest]
        public IEnumerator Execute_RepeatingNotDoneYet_OnEndCalled()
        {
            Mock<ISequenceDataExtractor> extractor = new Mock<ISequenceDataExtractor>(MockBehavior.Strict);

            long orderID = 123;

            Mock<ICompletionSource> repOnStart = new Mock<ICompletionSource>(MockBehavior.Strict);
            repOnStart.SetupGet(c => c.IsComplete).Returns(false);

            IOrderedSequence data = Mock.Of<IOrderedSequence>(o => o.OrderID == orderID);
            Mock<IActionSequence> firstSequence = SetupAction();

            firstSequence.Setup(s => s.OnEnd()).Returns(CompletionSource.Completed).Callback(() =>
            {
                repOnStart.SetupGet(c => c.IsComplete).Returns(true);
            });

            extractor.SetupGet(e => e.SyncedSequences).Returns(() =>
            {
                //empty intentionally for this test
                return Enumerable.Empty<SyncedSequenceItem>().ToLookup(r => r.Data.OrderID);
            });

            SequenceItem first = new SequenceItem(data, firstSequence.Object);
            extractor.SetupGet(e => e.ActionSequences).Returns(() =>
            {
                return first.AsEnumerable().ToLookup(s => s.Data.OrderID);
            });

            IOrderedSequence repData = Mock.Of<IOrderedSequence>(o => o.OrderID == orderID);
            Mock<IActionSequence> repSequence = SetupAction();

            repSequence.Setup(e => e.OnStart()).Returns(repOnStart.Object);

            SequenceItem rep = new SequenceItem(repData, repSequence.Object);
            extractor.SetupGet(e => e.RepeatingSequences).Returns(() =>
            {
                return rep.AsEnumerable().ToLookup(s => s.Data.OrderID);
            });

            extractor.Setup(e => e.DistinctSequences).Returns(() =>
            {
                return new List<long>() { orderID };
            });

            OrderedActionSequenceExecutor exec = new OrderedActionSequenceExecutor(extractor.Object, new TestSequenceRunner());

            yield return exec.Execute();

            firstSequence.Verify(s => s.OnStart(), Times.Once(), "first OnStart was not called correctly");
            firstSequence.Verify(s => s.OnEnd(), Times.Once(), "first OnEnd was not called correctly");

            repSequence.Verify(s => s.OnStart(), Times.Once(), "rep OnStart was not called correctly");
            repSequence.Verify(s => s.OnEnd(), Times.Once(), "rep OnEnd was not called correctly");
        }

        [UnityTest]
        public IEnumerator Execute_TransactionIsStartedAndEndedInOrder()
        {
            Mock<ISequenceDataExtractor> extractor = new Mock<ISequenceDataExtractor>();

            long txOrderID = 1;
            long orderID = 2;

            IOrderedSequence txData = Mock.Of<IOrderedSequence>(o => o.OrderID == txOrderID);

            Mock<IActionSequence> targetAction = SetupAction();

            Mock<IActionSequence> txAction = SetupAction();
            txAction.Setup(t => t.OnStart()).Returns(CompletionSource.Completed).Callback(() =>
            {
                Assert.That(targetAction.Invocations.Count, Is.Zero);
            }).Verifiable();

            txAction.Setup(t => t.OnEnd()).Returns(CompletionSource.Completed).Callback(() =>
            {
                Assert.That(targetAction.Invocations.Count, Is.EqualTo(2));
                Assert.That(targetAction.Invocations.FirstOrDefault().Method, Is.EqualTo(typeof(IActionSequence).GetMethod(nameof(IActionSequence.OnStart))));
                Assert.That(targetAction.Invocations.LastOrDefault().Method, Is.EqualTo(typeof(IActionSequence).GetMethod(nameof(IActionSequence.OnEnd))));

            }).Verifiable();

            SyncedSequenceItem txItem = new SyncedSequenceItem(txData, txAction.Object, orderID);

            IOrderedSequence data = Mock.Of<IOrderedSequence>(o => o.OrderID == orderID);

            SequenceItem item = new SequenceItem(data, targetAction.Object);

            extractor.SetupGet(e => e.ActionSequences).Returns(() =>
            {
                return item.AsEnumerable().ToLookup(s => s.Data.OrderID);
            });

            extractor.SetupGet(e => e.RepeatingSequences).Returns(() =>
            {
                //empty intentionally for this test
                return Enumerable.Empty<SequenceItem>().ToLookup(r => r.Data.OrderID);
            });

            extractor.SetupGet(e => e.SyncedSequences).Returns(() =>
            {
                return txItem.AsEnumerable().ToLookup(r => r.Data.OrderID);
            });

            extractor.Setup(e => e.DistinctSequences).Returns(() =>
            {
                return new List<long>() { txOrderID, orderID };
            });

            OrderedActionSequenceExecutor exec = new OrderedActionSequenceExecutor(extractor.Object, new TestSequenceRunner());

            yield return exec.Execute();

            targetAction.Verify(s => s.OnStart(), Times.Once(), "OnStart was not called correctly on target");
            targetAction.Verify(s => s.OnEnd(), Times.Once(), "OnEnd was not called correctly on target");

            txAction.Verify(s => s.OnStart(), Times.Once(), "OnStart was not called correctly on transactional action");
            txAction.Verify(s => s.OnEnd(), Times.Once(), "OnEnd was not called correctly on transactional action");
        }

        [UnityTest]
        public IEnumerator Execute_NestedTransactions()
        {
            Mock<ISequenceDataExtractor> extractor = new Mock<ISequenceDataExtractor>();

            long txOrderID = 1;
            long txInnerOrderID = 2;
            long orderID = 3;

            IOrderedSequence txData = Mock.Of<IOrderedSequence>(o => o.OrderID == txOrderID);
            IOrderedSequence txInnerData = Mock.Of<IOrderedSequence>(o => o.OrderID == txInnerOrderID);
            IOrderedSequence data = Mock.Of<IOrderedSequence>(o => o.OrderID == orderID);

            Mock<IActionSequence> txAction = SetupAction();
            Mock<IActionSequence> txInnerAction = SetupAction();
            Mock<IActionSequence> targetAction = SetupAction();

            txAction.Setup(t => t.OnStart()).Returns(CompletionSource.Completed).Callback(() =>
            {
                Assert.That(txInnerAction.Invocations.Count, Is.EqualTo(0));
                Assert.That(targetAction.Invocations.Count, Is.Zero);
            }).Verifiable();

            txAction.Setup(t => t.OnEnd()).Returns(CompletionSource.Completed).Callback(() =>
            {
                Assert.That(txInnerAction.Invocations.Count, Is.EqualTo(2));
                Assert.That(txInnerAction.Invocations.FirstOrDefault().Method, Is.EqualTo(typeof(IActionSequence).GetMethod(nameof(IActionSequence.OnStart))));
                Assert.That(txInnerAction.Invocations.LastOrDefault().Method, Is.EqualTo(typeof(IActionSequence).GetMethod(nameof(IActionSequence.OnEnd))));

                Assert.That(targetAction.Invocations.Count, Is.EqualTo(2));
                Assert.That(targetAction.Invocations.FirstOrDefault().Method, Is.EqualTo(typeof(IActionSequence).GetMethod(nameof(IActionSequence.OnStart))));
                Assert.That(targetAction.Invocations.LastOrDefault().Method, Is.EqualTo(typeof(IActionSequence).GetMethod(nameof(IActionSequence.OnEnd))));

            }).Verifiable();

            txInnerAction.Setup(t => t.OnStart()).Returns(CompletionSource.Completed).Callback(() =>
            {
                Assert.That(targetAction.Invocations.Count, Is.Zero);
            }).Verifiable();

            txInnerAction.Setup(t => t.OnEnd()).Returns(CompletionSource.Completed).Callback(() =>
            {
                Assert.That(targetAction.Invocations.Count, Is.EqualTo(2));
                Assert.That(targetAction.Invocations.FirstOrDefault().Method, Is.EqualTo(typeof(IActionSequence).GetMethod(nameof(IActionSequence.OnStart))));
                Assert.That(targetAction.Invocations.LastOrDefault().Method, Is.EqualTo(typeof(IActionSequence).GetMethod(nameof(IActionSequence.OnEnd))));

            }).Verifiable();

            SyncedSequenceItem txItem = new SyncedSequenceItem(txData, txAction.Object, txInnerOrderID);
            SyncedSequenceItem txInnerItem = new SyncedSequenceItem(txInnerData, txInnerAction.Object, orderID);

            SequenceItem item = new SequenceItem(data, targetAction.Object);

            extractor.SetupGet(e => e.ActionSequences).Returns(() =>
            {
                return item.AsEnumerable().ToLookup(s => s.Data.OrderID);
            });

            extractor.SetupGet(e => e.RepeatingSequences).Returns(() =>
            {
                //empty intentionally for this test
                return Enumerable.Empty<SequenceItem>().ToLookup(r => r.Data.OrderID);
            });

            extractor.SetupGet(e => e.SyncedSequences).Returns(() =>
            {
                return new List<SyncedSequenceItem>() { txItem, txInnerItem }.ToLookup(r => r.Data.OrderID);
            });

            extractor.Setup(e => e.DistinctSequences).Returns(() =>
            {
                return new List<long>() { txOrderID, txInnerOrderID, orderID };
            });

            OrderedActionSequenceExecutor exec = new OrderedActionSequenceExecutor(extractor.Object, new TestSequenceRunner());

            yield return exec.Execute();

            targetAction.Verify(s => s.OnStart(), Times.Once(), "OnStart was not called correctly on target");
            targetAction.Verify(s => s.OnEnd(), Times.Once(), "OnEnd was not called correctly on target");

            txAction.Verify(s => s.OnStart(), Times.Once(), "OnStart was not called correctly on transactional action");
            txAction.Verify(s => s.OnEnd(), Times.Once(), "OnEnd was not called correctly on transactional action");

            txInnerAction.Verify(s => s.OnStart(), Times.Once(), "OnStart was not called correctly on inner transactional action");
            txInnerAction.Verify(s => s.OnEnd(), Times.Once(), "OnEnd was not called correctly on inner transactional action");
        }
    }
}
