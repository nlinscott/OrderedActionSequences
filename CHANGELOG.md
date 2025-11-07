Created package 

Performance improvement - do not run a coroutine for each action sequence, only when we must account for a delay before an action sequence. Saves a coroutine.

Found issue during development where SequenceRunner did not work when an object begins in a disabled state. `Awake` is not called and components are not initialized causing exceptions.

1.0.2
 - Found bug where CompletionSource objects could have their links overwritten, leaking and forgetting previously CompletionSource objects causing processes to behave unexpectedly.
