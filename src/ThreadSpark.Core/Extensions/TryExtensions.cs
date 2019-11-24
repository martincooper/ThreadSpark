using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ThreadSpark.Core.Extensions
{
    public static class TryExtensions
    {
        /// <summary>
        /// Gets the value from a Try Success.
        /// User needs to check it's not a Failure before calling, else will throw an exception.
        /// </summary>
        /// <param name="item">The Try value.</param>
        /// <typeparam name="TType">The return type.</typeparam>
        /// <returns>Returns the unwrapped value.</returns>
        public static TType GetValue<TType>(this Try<TType> item)
        {
            return item.Match(
                Succ: val => val,
                Fail: _ => throw new ArgumentException("Error. Value not valid on a Try/Failure."));
        }

        /// <summary>
        /// Gets the Exception from a Try Failure.
        /// User needs to check it's a Failure before calling, else will throw an exception.
        /// </summary>
        /// <param name="item">The Try value.</param>
        /// <typeparam name="TType">The return type.</typeparam>
        /// <returns>Returns the unwrapped Exception.</returns>
        public static Exception GetException<TType>(this Try<TType> item)
        {
            return item.Match(
                Succ: _ => throw new ArgumentException("Error. Exception not valid on a Try/Success."),
                Fail: err => err);
        }
        
        /// <summary>
        /// Flattens a collection of Try values.
        /// If any are failed, then the first failure will be returned,
        /// else it'll return a collection of TType values.
        /// </summary>
        /// <param name="items">The collection of items to flatten.</param>
        /// <typeparam name="TType">The return type.</typeparam>
        /// <returns>Returns a collection of flattened values, or the first failure.</returns>
        public static Try<TType[]> AllOrFirstFail<TType>(this Try<TType>[] items)
        {
            var firstFail = items.FirstOrDefault(_ => _.IsFail());

            return firstFail != null
                ? Try<TType[]>(firstFail.GetException())
                : Try(items.Select(_ => _.GetValue()).ToArray());
        }
    }
}