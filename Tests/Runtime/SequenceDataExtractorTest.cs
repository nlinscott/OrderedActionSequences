using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using OrderedActionSequences;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Tests
{
    class SequenceDataExtractorTest
    {
        private GameObject Root
        {
            get;
            set;
        }

        [UnitySetUp]
        public IEnumerator SetupTests()
        {
            TestSceneLoader loader = new TestSceneLoader();
            yield return loader.LoadTestScene();

            Root = loader.GetTestContextByName(TestSceneLoader.RootObjects.SequenceDataExtractorTest);
        }

        [UnityTest]
        public IEnumerator DistinctIDsGrouped()
        {
            Transform transform = Root.transform.Find(nameof(SequenceDataExtractorTest.DistinctIDsGrouped));
            Assert.That(transform, Is.Not.Null);

            SequenceDataExtractor extractor = new SequenceDataExtractor(transform);

            IOrderedSequence[] sequences = transform.GetComponentsInChildren<IOrderedSequence>();

            Assert.That(sequences, Is.Not.Null);
            Assert.That(sequences.Length, Is.Not.Zero);

            IEnumerable<long> allOrderIDs = sequences.Select(s => s.OrderID);

            HashSet<long> set = new HashSet<long>(allOrderIDs);

            CollectionAssert.AreEqual(set, extractor.DistinctSequences);

            yield return null;
        }

        [UnityTest]
        public IEnumerator AllGroupedCorrectly()
        {
            Transform transform = Root.transform.Find(nameof(SequenceDataExtractorTest.AllGroupedCorrectly));
            Assert.That(transform, Is.Not.Null);

            SequenceDataExtractor extractor = new SequenceDataExtractor(transform);

            Assert.That(extractor.ActionSequences.Count, Is.EqualTo(2));
            Assert.That(extractor.RepeatingSequences.Count, Is.EqualTo(1));
            Assert.That(extractor.SyncedSequences.Count, Is.EqualTo(1));

            yield return null;
        }

        [UnityTest]
        public IEnumerator NoRepeating()
        {
            Transform transform = Root.transform.Find(nameof(SequenceDataExtractorTest.NoRepeating));
            Assert.That(transform, Is.Not.Null);

            SequenceDataExtractor extractor = new SequenceDataExtractor(transform);

            IEnumerable<SequenceItem> seqItems = extractor.ActionSequences.Select(g => g).SelectMany(g => g);
            IEnumerable<SequenceItem> repSeqItems = extractor.RepeatingSequences.Select(g => g).SelectMany(g => g);
            IEnumerable<SequenceItem> syncedItems = extractor.SyncedSequences.Select(g => g).SelectMany(g => g);

            Assert.That(syncedItems.Count(), Is.Zero);
            Assert.That(seqItems.Count(), Is.EqualTo(2));
            Assert.That(repSeqItems.Count(), Is.Zero);


            yield return null;
        }

        [UnityTest]
        public IEnumerator OnlyRepeating()
        {
            Transform transform = Root.transform.Find(nameof(SequenceDataExtractorTest.OnlyRepeating));
            Assert.That(transform, Is.Not.Null);

            SequenceDataExtractor extractor = new SequenceDataExtractor(transform);

            IEnumerable<SequenceItem> seqItems = extractor.ActionSequences.Select(g => g).SelectMany(g => g);
            IEnumerable<SequenceItem> repSeqItems = extractor.RepeatingSequences.Select(g => g).SelectMany(g => g);
            IEnumerable<SequenceItem> syncedItems = extractor.SyncedSequences.Select(g => g).SelectMany(g => g);

            Assert.That(syncedItems.Count(), Is.Zero);

            Assert.That(seqItems.Count(), Is.Zero);
            Assert.That(repSeqItems.Count(), Is.EqualTo(2));

            yield return null;
        }

        [UnityTest]
        public IEnumerator OneSyncedSequence()
        {
            Transform transform = Root.transform.Find(nameof(SequenceDataExtractorTest.OneSyncedSequence));
            Assert.That(transform, Is.Not.Null);

            SequenceDataExtractor extractor = new SequenceDataExtractor(transform);

            IEnumerable<SequenceItem> syncedItems = extractor.SyncedSequences.Select(g => g).SelectMany(g => g);

            Assert.That(syncedItems.Count(), Is.EqualTo(1));

            yield return null;
        }
    }

}
