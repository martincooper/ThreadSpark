# ThreadSpark
C# Library for Functional Threading and Async Abstractions

## Overview

ThreadSpark is a lightweight library which simplifies concurrency and multi-threading, handling exceptions, converting into Try<T> and aggregating when requested.

### Features :
 * Can specify max threads to run with.
 * Optional early termination of threads if one fails.
 * Aggregates errors into a Single Try<T> if set to error on first fail.
 * Can specify Cancellation Token for early termination from external source.
 * Provides progress notification on each item complete.
 
## Examples

```csharp
// Build a collection of functions which each one will be on a seperate thread.
var funcs = Enumerable
	.Range(1, 100)
	.Select(idx => fun(() => idx * 100));

// Set to run with 10 threads.
var runner = new ConcurrentFunctionRunner(10);
var result = runner.Run(funcs);

Assert.IsTrue(result.IsSucc());
Assert.AreEqual(result.GetValue().Length, 100);
```


```csharp
// Using the extension method to inline the call.
// Runs concurrently and returns all the successful results, or the first one which failed.
var result = Enumerable
	.Range(1, 100)
	.Select(idx => fun(() => idx * 100))
	.RunConcurrentlyUntilError(10);

Assert.IsTrue(result.IsSucc());
Assert.AreEqual(result.GetValue().Length, 100);
```
