using Moq;
using OrderedActionSequences.Executors;
using OrderedActionSequences;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TestTools;
using System.Collections;
using NUnit.Framework;

class SynchronousSequenceExecutorTest
{
    private static Mock<IActionSequence> SetupAction()
    {
        Mock<IActionSequence> sequence = new Mock<IActionSequence>(MockBehavior.Strict);
        sequence.Setup(s => s.OnStart()).Returns(CompletionSource.Completed).Verifiable();
        sequence.Setup(s => s.OnEnd()).Returns(CompletionSource.Completed).Verifiable();

        return sequence;
    }

    [Test]
    public void Execute()
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

        SynchronousSequenceExecutor exec = new SynchronousSequenceExecutor(extractor.Object);

        exec.Execute();

        sequence.Verify(s => s.OnStart(), Times.Once(), "OnStart was not called correctly");
        sequence.Verify(s => s.OnEnd(), Times.Once(), "OnEnd was not called correctly");
    }

    [Test]
    public void Execute_StopsWhenRepeatingSequencesFound()
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
            return item.AsEnumerable().ToLookup(s => s.Data.OrderID);
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

        SynchronousSequenceExecutor exec = new SynchronousSequenceExecutor(extractor.Object);

        exec.Execute();

        sequence.Verify(s => s.OnStart(), Times.Never(), "OnStart should not be not called");
        sequence.Verify(s => s.OnEnd(), Times.Never(), "OnEnd should not be not called");
    }

    [Test]
    public void Execute_StopsWhenSyncedSequencesFound()
    {
        Mock<ISequenceDataExtractor> extractor = new Mock<ISequenceDataExtractor>(MockBehavior.Strict);

        long orderID = 123;
        IOrderedSequence data = Mock.Of<IOrderedSequence>(o => o.OrderID == orderID);

        Mock<IActionSequence> sequence = SetupAction();

        SyncedSequenceItem item = new SyncedSequenceItem(data, sequence.Object, 1);

        extractor.SetupGet(e => e.SyncedSequences).Returns(() =>
        {
            //empty intentionally for this test
            return item.AsEnumerable().ToLookup(s => s.Data.OrderID);
        });

        extractor.Setup(e => e.DistinctSequences).Returns(() =>
        {
            return new List<long>() { orderID };
        });

        SynchronousSequenceExecutor exec = new SynchronousSequenceExecutor(extractor.Object);

        exec.Execute();

        sequence.Verify(s => s.OnStart(), Times.Never(), "OnStart should not be not called");
        sequence.Verify(s => s.OnEnd(), Times.Never(), "OnEnd should not be not called");
    }

    [Test]
    public void Execute_StopsWhenNoSequencesFound()
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

        extractor.Setup(e => e.DistinctSequences).Returns(() =>
        {
            return Enumerable.Empty<long>();
        });

        SynchronousSequenceExecutor exec = new SynchronousSequenceExecutor(extractor.Object);

        exec.Execute();

        sequence.Verify(s => s.OnStart(), Times.Never(), "OnStart should not be not called");
        sequence.Verify(s => s.OnEnd(), Times.Never(), "OnEnd should not be not called");
    }
}