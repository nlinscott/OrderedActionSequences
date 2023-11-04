namespace OrderedActionSequences
{
    interface IActionSequence
    {
        ICompletionSource OnStart();

        ICompletionSource OnEnd();
    }
}
