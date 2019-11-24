using System;
using LanguageExt;

namespace ThreadSpark.Core.Extensions
{
    public static class ConcurrentFunctionRunnerExtensions
    {
        /// <summary>
        /// Extension Function to allow a cleaner way to access task results through named tuples.
        /// </summary>
        /// <param name="runner">The Concurrent Function Runner.</param>
        /// <param name="first">The first function.</param>
        /// <param name="second">The second function</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns>Returns a tuple of value the same order as passed in.</returns>
        public static (Try<TResult> First, Try<TResult> Second)
            Run<TResult>(this ConcurrentFunctionRunner runner,
                Func<TResult> first, Func<TResult> second)
        {
            var result = runner.Run(new[] { first, second });
            return (result[0], result[1]);
        }
        
        /// <summary>
        /// Extension Function to allow a cleaner way to access task results through named tuples.
        /// </summary>
        /// <param name="runner">The Concurrent Function Runner.</param>
        /// <param name="first">The first function.</param>
        /// <param name="second">The second function</param>
        /// <param name="third">The third function</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns>Returns a tuple of value the same order as passed in.</returns>
        public static (Try<TResult> First, Try<TResult> Second, Try<TResult> Third)
            Run<TResult>(this ConcurrentFunctionRunner runner,
                Func<TResult> first, Func<TResult> second, Func<TResult> third)
        {
            var result = runner.Run(new[] 
                { first, second, third });

            return (result[0], result[1], result[2]);
        }
        
        /// <summary>
        /// Extension Function to allow a cleaner way to access task results through named tuples.
        /// </summary>
        /// <param name="runner">The Concurrent Function Runner.</param>
        /// <param name="first">The first function.</param>
        /// <param name="second">The second function</param>
        /// <param name="third">The third function</param>
        /// <param name="fourth">The fourth function</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns>Returns a tuple of value the same order as passed in.</returns>
        public static (Try<TResult> First, Try<TResult> Second, Try<TResult> Third, Try<TResult> Fourth)
            Run<TResult>(this ConcurrentFunctionRunner runner,
                Func<TResult> first, Func<TResult> second, Func<TResult> third, Func<TResult> fourth)
        {
            var result = runner.Run(new[]
                { first, second, third, fourth });

            return (result[0], result[1], result[2], result[3]);
        }
        
        /// <summary>
        /// Extension Function to allow a cleaner way to access task results through named tuples.
        /// </summary>
        /// <param name="runner">The Concurrent Function Runner.</param>
        /// <param name="first">The first function.</param>
        /// <param name="second">The second function</param>
        /// <param name="third">The third function</param>
        /// <param name="fourth">The fourth function</param>
        /// <param name="fifth">The fifth function</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns>Returns a tuple of value the same order as passed in.</returns>
        public static (Try<TResult> First, Try<TResult> Second, Try<TResult> Third, Try<TResult> Fourth, Try<TResult> Fifth)
            Run<TResult>(this ConcurrentFunctionRunner runner,
                Func<TResult> first, Func<TResult> second, Func<TResult> third, Func<TResult> fourth, Func<TResult> fifth)
        {
            var result = runner.Run(new[]
                { first, second, third, fourth, fifth });

            return (result[0], result[1], result[2], result[3], result[4]);
        }
        
        /// <summary>
        /// Extension Function to allow a cleaner way to access task results through named tuples.
        /// </summary>
        /// <param name="runner">The Concurrent Function Runner.</param>
        /// <param name="first">The first function.</param>
        /// <param name="second">The second function</param>
        /// <param name="third">The third function</param>
        /// <param name="fourth">The fourth function</param>
        /// <param name="fifth">The fifth function</param>
        /// <param name="sixth">The sixth function</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns>Returns a tuple of value the same order as passed in.</returns>
        public static (Try<TResult> First, Try<TResult> Second, Try<TResult> Third, Try<TResult> Fourth, Try<TResult> Fifth, Try<TResult> Sixth)
            Run<TResult>(this ConcurrentFunctionRunner runner,
                Func<TResult> first, Func<TResult> second, Func<TResult> third, Func<TResult> fourth, Func<TResult> fifth, Func<TResult> sixth)
        {
            var result = runner.Run(new[]
                { first, second, third, fourth, fifth, sixth });

            return (result[0], result[1], result[2], result[3], result[4], result[5]);
        }
        
        /// <summary>
        /// Extension Function to allow a cleaner way to access task results through named tuples.
        /// </summary>
        /// <param name="runner">The Concurrent Function Runner.</param>
        /// <param name="first">The first function.</param>
        /// <param name="second">The second function</param>
        /// <param name="third">The third function</param>
        /// <param name="fourth">The fourth function</param>
        /// <param name="fifth">The fifth function</param>
        /// <param name="sixth">The sixth function</param>
        /// <param name="seventh">The seventh function</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns>Returns a tuple of value the same order as passed in.</returns>
        public static (Try<TResult> First, Try<TResult> Second, Try<TResult> Third, Try<TResult> Fourth, Try<TResult> Fifth, Try<TResult> Sixth, Try<TResult> Seventh)
            Run<TResult>(this ConcurrentFunctionRunner runner,
                Func<TResult> first, Func<TResult> second, Func<TResult> third, Func<TResult> fourth, Func<TResult> fifth, Func<TResult> sixth, Func<TResult> seventh)
        {
            var result = runner.Run(new[]
                { first, second, third, fourth, fifth, sixth, seventh });

            return (result[0], result[1], result[2], result[3], result[4], result[5], result[6]);
        }
    }
}