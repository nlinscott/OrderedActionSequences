﻿using OrderedActionSequences.Model;

namespace OrderedActionSequences.ActionSequences.Decorator.Base
{
    public abstract class SyncedSequenceBase : SequenceBase
    {
        protected SyncedSequence SequenceData
        {
            get;
            private set;
        }

        protected override void Awake()
        {
            base.Awake();

            SequenceData = GetComponent<SyncedSequence>();
        }
    }
}
