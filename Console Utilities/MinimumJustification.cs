using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUtilities
{
    /// <summary>
    /// Represents a minimum justification length that can either be a percentage or absolute value
    /// </summary>
    public class MinimumJustification
    {
        /// <summary>Gets or sets whether the value is a percentage or absolute value</summary>
        public bool IsPercentage { get; set; }
        /// <summary>Gets or sets the total value</summary>
        public int Value { get; set; }

        /// <summary>
        /// Initialises an instance of the MinimumJustification type
        /// </summary>
        /// <param name="value">The value of the justification value</param>
        /// <param name="isPercentage">Whether the value given is a percentage</param>
        public MinimumJustification(int value, bool isPercentage)
        {
            IsPercentage = isPercentage;
            Value = value;
        }

        /// <summary>
        /// Gets the absolute value of the justification size depending
        /// </summary>
        /// <param name="availableWidth">The available width</param>
        /// <returns></returns>
        public int GetAbsoluteValue(int availableWidth)
        {
            if (IsPercentage)
            {
                return Value * availableWidth / 100;
            }
            return Value;
        }
    }
}
