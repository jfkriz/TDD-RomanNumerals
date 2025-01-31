﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RomanNumerals.lib
{
    /// <summary>
    /// Convert Roman Numerals to Arabic, and Arabic to Roman Numerals.
    /// 
    /// Rules are well-defined on this page: http://sierra.nmsu.edu/morandi/coursematerials/RomanNumerals.html
    /// </summary>
    public class RomanNumeralsConverter
    {
        private static Dictionary<char, int> numerals = new Dictionary<char, int>
        {
            { 'I', 1 },
            { 'V', 5 },
            { 'X', 10 },
            { 'L', 50 },
            { 'C', 100 },
            { 'D', 500 },
            { 'M', 1000 },
        };

        public int ConvertRomanNumeralsToArabic(string romanNumerals)
        {
            var chars = ValidateInput(romanNumerals);

            var sum = 0;
            for (var i = 0; i < chars.Length; i++)
            {
                int currentValue = GetNumber(chars[i]);
                int? next = LookAhead(chars, i);
                // If the next value is larger than current, we'll subtract current, assuming it is valid to do so
                if (next.HasValue && next.Value > currentValue)
                {
                    // Based on the above link, looks like the rule is that the only valid subtractions are 5 - 1, 10 - 1, 50 - 10, 100 - 10, 500 - 100, and 1000 - 100
                    // So the "rule" is that to subtract, the result of dividing larger and smaller must be either 5 or 10, otherwise it's not a valid subtraction combination.
                    var quotient = next.Value / currentValue;
                    if (quotient != 10 && quotient != 5)
                    {
                        throw new ArgumentException($"Invalid subtraction '{chars[i]}{chars[i + 1]}' at position {i}");
                    }
                    sum -= currentValue;
                }
                else
                {
                    // .. otherwise just add the current value to the total
                    sum += currentValue;
                }
            }

            return sum;
        }

        private char[] ValidateInput(string romanNumerals)
        {
            if (romanNumerals.Length == 0)
            {
                throw new ArgumentException("Roman Numerals string must be at least one character");
            }

            var tooManyRepeats = new Regex(@"(.)\1{3,}");
            if (tooManyRepeats.IsMatch(romanNumerals))
            {
                throw new ArgumentException("Roman numerals cannot repeat more than 3 times");
            }

            var certainCharactersCannotRepeat = new Regex(@"([VLD])\1{1,}", RegexOptions.IgnoreCase);
            if (certainCharactersCannotRepeat.IsMatch(romanNumerals))
            {
                throw new ArgumentException("V, L, and D are not allowed to repeat");
            }

            return romanNumerals.ToUpper().ToCharArray();
        }

        private int? LookAhead(char[] chars, int current)
        {
            if (chars.Length > current + 1)
            {
                return GetNumber(chars[current + 1]);
            } else
            {
                return null;
            }
        }

        private int GetNumber(char numeral)
        {
            if (numerals.ContainsKey(numeral))
            {
                return numerals[numeral];
            }
            else
            {
                throw new ArgumentException($"{numeral} is not a valid Roman Numeral character");
            }
        }
    }
}
