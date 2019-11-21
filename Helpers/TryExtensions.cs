using System;
using LanguageExt;

namespace ThreadStrike.Helpers
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
        
        public static Exception GetException<TType>(this Try<TType> item)
        {
            return item.Match(
                Succ: _ => throw new ArgumentException("Error. Exception not valid on a Try/Success."),
                Fail: err => err);
        }
    }
}