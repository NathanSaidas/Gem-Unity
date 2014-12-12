using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added utiltieis class.
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// This class contains a bunch of helper methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// This is the shorth and way of destroy an object with DestroyImmediate or Destroy depending on the state of Application.isPlaying
        /// </summary>
        /// <param name="aObject">The object being destroyed</param>
        public static void Destroy(UnityEngine.Object aObject)
        {
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(aObject);
            }
            else
            {
                UnityEngine.Object.Destroy(aObject);
            }
        }
        /// <summary>
        /// This is the shorth and way of destroy an object with DestroyImmediate or Destroy depending on the state of Application.isPlaying
        /// </summary>
        /// <param name="aObject">The object being destroyed</param>
        public static void DestroyImmediate(UnityEngine.Object aObject)
        {
            if (Application.isEditor)
            {
                UnityEngine.Object.DestroyImmediate(aObject);
            }
            else
            {
                UnityEngine.Object.Destroy(aObject);
            }
        }

        /// <summary>
        /// Parses a string for all of its words, uses a space character as a separator.
        /// </summary>
        /// <param name="aContext">The string to parse</param>
        /// <returns>A list of words from the context. </returns>
        public static List<string> ParseToWords(string aContext, bool aToLower)
        {
            List<string> words = new List<string>();
            if (aContext.Length == 0)
            {
                return words;
            }
            if (aToLower == true)
            {
                aContext = aContext.ToLower();
            }
            if (aContext.Length == 1)
            {
                words.Add(aContext);
                return words;
            }
            int startIndex = -1;
            int endIndex = 0;
            int spaceIndex = 0;

            while (aContext.Length > 0)
            {
                startIndex = -1;
                endIndex = 0;
                spaceIndex = 0;
                if (aContext.Length == 1)
                {
                    words.Add(aContext);
                    break;
                }

                for (int i = 0; i < aContext.Length; i++)
                {
                    if (startIndex == -1 && aContext[i] != ' ')
                    {
                        startIndex = i;
                        continue;
                    }
                    if (startIndex != -1 && aContext[i] == ' ')
                    {
                        spaceIndex = i;
                        break;
                    }
                    if (i == aContext.Length - 1)
                    {
                        spaceIndex = aContext.Length;
                        break;
                    }
                }

                endIndex = spaceIndex - 1;
                int wordLength = Mathf.Clamp(endIndex - startIndex + 1, 0, aContext.Length);
                int removeLength = Mathf.Clamp(spaceIndex - startIndex + 1, 0, aContext.Length);
                string word = aContext.Substring(startIndex, wordLength);
                aContext = aContext.Remove(startIndex, removeLength);
                words.Add(word);
            }
            return words;
        }

        /// <summary>
        /// Parses a string for all of its words using the specified separtor.
        /// </summary>
        /// <param name="aContext">The string to parse</param>
        /// <param name="aSeparator">The value which determines a space in the sentence</param>
        /// <returns>A list of words from the context. </returns>
        public static List<string> ParseToWords(string aContext, char aSeparator, bool aToLower)
        {
            List<string> words = new List<string>();
            if (aContext.Length == 0)
            {
                return words;
            }
            if (aToLower == true)
            {
                aContext = aContext.ToLower();
            }
            if (aContext.Length == 1)
            {
                words.Add(aContext);
                return words;
            }
            int startIndex = -1;
            int endIndex = 0;
            int spaceIndex = 0;

            while (aContext.Length > 0)
            {
                startIndex = -1;
                endIndex = 0;
                spaceIndex = 0;
                if (aContext.Length == 1)
                {
                    words.Add(aContext);
                    break;
                }

                for (int i = 0; i < aContext.Length; i++)
                {
                    if (startIndex == -1 && aContext[i] != aSeparator)
                    {
                        startIndex = i;
                        continue;
                    }
                    if (startIndex != -1 && aContext[i] == aSeparator)
                    {
                        spaceIndex = i;
                        break;
                    }
                    if (i == aContext.Length - 1)
                    {
                        spaceIndex = aContext.Length;
                        break;
                    }
                }

                endIndex = spaceIndex - 1;
                int wordLength = Mathf.Clamp(endIndex - startIndex + 1, 0, aContext.Length);
                int removeLength = Mathf.Clamp(spaceIndex - startIndex + 1, 0, aContext.Length);
                string word = aContext.Substring(startIndex, wordLength);
                aContext = aContext.Remove(startIndex, removeLength);
                words.Add(word);
            }
            return words;
        }
    }
}