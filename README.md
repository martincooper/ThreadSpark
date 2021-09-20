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

Builds a collection of functions which will all be run on a seperate thread.
It returns a collection of Try<T> values, each one can be individually cheked for Success / Failure.
```csharp
var funcs = Enumerable
	.Range(1, 100)
	.Select(idx => fun(() => idx * 100));

// Set to run with 10 threads.
var runner = new ConcurrentFunctionRunner(10);
var result = runner.Run(funcs);

Assert.IsTrue(result.IsSucc());
Assert.AreEqual(result.GetValue().Length, 100);
```

Builds a collection of functions which will all be run on a seperate thread.
Using "Run Until Error" will return the aggregated results if ALL successful, or the first failure which occurred.
```csharp
// Using the "RunConcurrentlyUntilError" extension method to inline the call.
var result = Enumerable
	.Range(1, 100)
	.Select(idx => fun(() => idx * 100))
	.RunConcurrentlyUntilError(10);

Assert.IsTrue(result.IsSucc());
Assert.AreEqual(result.GetValue().Length, 100);
```
