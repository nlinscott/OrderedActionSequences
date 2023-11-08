# Documentation

## Important Components and Concepts

1. CompletionSource
   - This class operates in a similar fashion to how the CancellationTokenSource works in .NET. You can create a new one, but instead of requesting cancellation, the instance will be used to report whether the operation has been completed.
   - Layers of abstraction make it possible to distinguish which code is responsible for calling `MarkComplete()` and which code must monitor for the completed state. Consider which layer is necessary in which part of your code. There are a two interfaces to consider in addition to the concrete implementation: `ICompletionSource`, `ICompletionToken`
   - **Not thread safe**, not intended to be used in mulithreaded code. Unity operates as a single threaded application with multiple synchronous calls to `Update()` and other behaviour lifecycle methods. This is intended to work with that paradigm.
   - Use `CompletionSource.Completed` to obtain a new instance that is marked as completed, for convienience.
> There is also a `CancelCompletionSource` that inherits from `CompletionSource`. This is more implementation specific and is not quite as ubiquitous, but can be used to determine if something was "cancelled" in constrast to being "complete" while using the same underlying mechanisms
2. OrderedActionSequence
   - This is the most basic identifier for a sequence item. The Order ID is a completely arbitrary ID that you define within a single sequence.
   - Use the optional `Start Delay Seconds` to wait some time before running the correspondng sequence. This is preferred over creating an IActionSequence implementation for the sole purpose of waiting.
3. IActionSequence
   - Implement this interface onto a MonoBehaviour to define what your sequence item will do
   - Add implementation as a component on the same GameObject as your `OrderedActionSequence`. Together, these make up a runnable item within your sequence.
   - Warnings will be issues if an implementation is not present, as its required in order to run.
4. WaitForCompletionSource
   - At the core of all of these features lies the backbone that makes all of this work. This class will wait for a CompletionSource to be marked complete. Use CompletionSource in your code as needed as well as this class to wait for completion.
5. Executors
   - **OrderedActionSequenceBehaviour**
      - Standerd sequence running capabilities using coroutines. Async by default. Can use all Sequence Item Types
   - **SyncOrderedActionSequenceBehaviour**
      - Less functional executor. Used specifically to run same-frame operations. Can only utilize default Sequence Item Types. See example below. 

## Sequence Item Types

There are a handful of different Sequence Item Types to facilitate your sequencing needs.

|Name| Explanation | Required Components
|--|--|--|
| Default | The most base identifier for a sequence item and has no special functions. | OrderedActionSequence
| Counted Repeating | Has properties to allow one action to run multiple times at an interval. | CountedRepeatingActionSequence, CountedRepeatingSequence
| Timed Repeating | Allows repetition at an interval for a duration. Constrain with minimum and maximum repetitions. | TimedRepeatingActionSequence, TimedRepeatingSequence
| Transactional | Calls `OnStart()`, then waits for a Target Order ID to be reached before calling `OnEnd()`| SyncedSequence, TransactionalActionSequence

> Transactional sequences are very powerful, allowing you to initiate some action, perform any number of actions, then end the initial action in the order specified. Great for enabling/disabling player control automaticaly or moving objects back and forth. Your IActionSequence implementations must support this behaviour by properly implementing both `OnStart()` and `OnEnd()` such that each call puts your objects in the desired state when called.

## Basics
In the editor, start by creating a new GameObject with an `OrderedActionSequenceBehaviour` component attached to it. 

> `Sequence Runner` is a required script and will be added automatically.

Next, create child GameObjects that will represent each step of your sequence. Assign an `OrderedActionSequence` component to each of the GameObjects. The parent components will only detect immediate children, so all steps of the sequence must be a direct child. 

Finally, create your `IActionSequence` implementation. Ideally, these implementations are small, reusable snippets of code such as enabling/disabling a GameObject, playing a VisualEffect, setting an Animation State, playing an animation or even running a nested sequence. Managing state machines is also possible depending on how you implement your classes.


### Running

To run a sequence, you need to obtain a reference to an `OrderedActionSequenceBehaviour` or `SynchronousSequenceBehavior` and call the appropriate method to run it. This behaviour class acts as an abstraction. Your controller classes don't necessarily need to know about the sequence itself, and you can configure as many as necessary to define and set state, for example.

> Sequnces run in the order you define Order IDs. In the editor, you assign these to arbitrary values as needed to ensure your objects run in the correct order. The order of the GameObject within the sequence is irrelevant. Its possible to have duplicated Order IDs. This means that two items will run in parallel. The next sequence item with a higher Order ID will not run until all actions for the previous Order ID are complete. 

## Architectural Notes

As mentioned previously, the `OrderedActionSequenceBehaviour` and `SynchronousSequenceBehavior` can act as abstractions. For example, lets say you need to create controllers for enemies. Rather than creating a separate controller and write code for a bug enemy and a rat enemy separately, you can define states in terms of ordered action sequences. This way, you can reuse a controller that will set certain states when you need them set, but those states are no longer implementation specific. You can have one controller class for multiple enemies and design enemy systems around these reusable components for a cleaner game architecture.

### Limitations of IActionSequence Implementations

By default, all items in a sequence are run using coroutines asynchronously. As such, actions such as disabling colliders or other frame-important operations will not work as expected. For example, if a player fires a projectile at an enemy and defeats it, allowing the player to rush through a battlefield. The collider of the enemy must be removed upon defeat in order for the player to move freely. If done using the asynchronous implementation, this collider **may not be** removed on the same frame the projectile registered collision. To solve this problem, you must used the `SyncOrderedActionSequenceBehaviour`. This works almost entirely the same, but it specificaly omits any async behaviour; meaning, it does not use coroutines. This will run an entire sequence in sync. This means only Default `OrderedActionSequence` components are allowed and each `IActionSequence` implementation must implement a definitive `OnStart()` and `OnEnd()`, as the system will call each method synchronously and will ignore the returned `CompletionSource`s.

## Sample Scene Overview

Open the Sample scene and take a look at all the sequences defined. The `Runner` is used to run a single sequence when you enter play mode. Assign the Sequence to whichever sequence you want to run. Some sequences involve waiting for the "player" to enter a certain area. There are no controls or UI elements in the sample scene, so in order to move the player, just enter the Scene view while in Play mode and drag the cube into the trigger zone. Simple particle systems play and sample logging is done in the console for you to understand when sequnce items are executed.
