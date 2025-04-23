Created package 

Performance improvement - do not run a coroutine for each action sequence, only when we must account for a delay before an action sequence. Saves a coroutine.

Found issue during development where SequenceRunner did not work when an object begins in a disabled state. `Awake` is not called and components are not initialized causing exceptions.