using System;
using System.Collections.Generic;
using System.Linq;
using WindowsInput.Native;
using log4net;
using Personal_Keyboard_Mapper.Lib.Interfaces;

namespace Personal_Keyboard_Mapper.Lib.Extensions
{
    public static class CombinationsExtensions
    {
        /// <summary>
        /// Converts to virtualkeycodes.
        /// </summary>
        /// <param name="combinations">The combinations.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">combinations</exception>
        public static IEnumerable<VirtualKeyCode> ToVirtualKeyCodes(this IEnumerable<IKeyCombination> combinations)
        { 
            if (combinations == null)
            {
                throw new ArgumentNullException(nameof(combinations));
            }

            return combinations.SelectMany(x => x.Keys).Select(x => x.KeyCode);
        }

        /// <summary>
        /// Determines whether [is empty combination].
        /// </summary>
        /// <param name="combination">The combination.</param>
        /// <returns>
        ///   <c>true</c> if [is empty combination] [the specified combination]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">combination</exception>
        public static bool IsEmptyCombination(this IKeyCombination combination)
        {
            if (combination == null)
            {
                throw new ArgumentNullException(nameof(combination));
            }

            return combination.Keys.All(x => x == null);
        }

        /// <summary>
        /// Makes a copy of source combination.
        /// </summary>
        /// <param name="sourceCombination">The source combination.</param>
        /// <param name="destinationCombination">The destination combination.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// sourceCombination
        /// or
        /// destinationCombination
        /// </exception>
        public static IKeyCombination CopyTo(this IKeyCombination sourceCombination,
            IKeyCombination destinationCombination)
        {
            if (sourceCombination == null)
            {
                throw new ArgumentNullException(nameof(sourceCombination));
            }
            if (destinationCombination == null)
            {
                throw new ArgumentNullException(nameof(destinationCombination));
            }

            destinationCombination.Action = sourceCombination.Action;
            destinationCombination.Keys = sourceCombination.Keys;
            return destinationCombination;
        }

        /// <summary>
        /// Determines whether [is not empty action combination].
        /// </summary>
        /// <param name="combination">The combination.</param>
        /// <returns>
        ///   <c>true</c> if [is not empty action combination] [the specified combination]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// combination
        /// or
        /// Action
        /// </exception>
        public static bool IsNotEmptyActionCombination(this IKeyCombination combination, ILog logger)
        {
            if (combination == null)
            {
                logger.Warn("Combination is null");
                return false;
            }

            if (combination.Action == null)
            {
                logger.Warn("Action is null");
                return false;
            }

            return combination.Action.OutputVirtualKeys.Any();
        }

        /// <summary>
        /// Determines whether [is full combination].
        /// </summary>
        /// <param name="combination">The combination.</param>
        /// <returns>
        ///   <c>true</c> if [is full combination] [the specified combination]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// combination
        /// or
        /// Keys
        /// </exception>
        public static bool IsFullCombination(this IKeyCombination combination)
        {
            if (combination == null)
            {
                throw new ArgumentNullException(nameof(combination));
            }

            if (combination.Keys == null)
            {
                throw new ArgumentNullException(nameof(combination.Keys));
            }

            return combination.Keys.All(x => x != null);
        }

        public static string ToString(this IKeyCombination combination)
        {
            if (combination == null)
            {
                throw new ArgumentNullException(nameof(combination));
            }

            if (combination.Keys == null)
            {
                throw new ArgumentNullException(nameof(combination.Keys));
            }

            return string.Join(" ", combination.Keys.Select(x => x.KeyCode.ToString()));
        }

        public static IEnumerable<IKeyCombination> Copy(this IEnumerable<IKeyCombination> source)
        {
            var result = new List<IKeyCombination>();
            foreach (var combination in source)
            {
                result.Add((IKeyCombination)combination.Clone());
            }

            return result;
        }

        /// <summary>
        /// Determines whether the source combination has specified keys.
        /// </summary>
        /// <param name="sourceCombination">The source combination.</param>
        /// <param name="firstKey">string representation of first key.</param>
        /// <param name="secondKey">string representation of second key.</param>
        /// <param name="thirdKey">string representation of third key (optional).</param>
        /// <returns>
        ///   <c>true</c> if the source combination has specified keys; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasKeys(this IKeyCombination sourceCombination, string firstKey, string secondKey,
            string thirdKey = default)
        {
            if (thirdKey != default)
            {
                return sourceCombination.Keys[0].ToString() == firstKey
                       && sourceCombination.Keys[1].ToString() == secondKey
                       && sourceCombination.Keys[2].ToString() == thirdKey;
            }
            return sourceCombination.Keys[0].ToString() == firstKey
                   && sourceCombination.Keys[1].ToString() == secondKey;
        }
    }
}