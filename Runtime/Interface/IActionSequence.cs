namespace OrderedActionSequences
{
    public interface IActionSequence
    {
        ICompletionSource OnStart();

        ICompletionSource OnEnd();
    }
}
