using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUtilities
{
    /// <summary>
    /// Represents a padding set with top, left, right and bottom padding
    /// </summary>
    public class Padding
    {
        /// <summary>Gets or sets the top padding</summary>
        public int Top { get; set; }
        /// <summary>Gets or sets the left padding</summary>
        public int Left { get; set; }
        /// <summary>Gets or sets the right padding</summary>
        public int Right { get; set; }
        /// <summary>Gets or sets the bottom padding</summary>
        public int Bottom { get; set; }

        /// <summary>
        /// Sets the padding with the default padding
        /// </summary>
        public Padding()
        {
            SetPadding(0, 4, 4, 0);
        }

        /// <summary>
        /// Creates a padding class with one value for both sets of padding
        /// </summary>
        /// <param name="all">The values for all padding</param>
        public Padding(int all)
        {
            SetPadding(all, all, all, all);
        }

        /// <summary>
        /// Creates a padding class with one value for both sets of padding
        /// </summary>
        /// <param name="topAndBottom">The values for the top and bottom padding</param>
        /// <param name="sides">The value for both sets of padding</param>
        public Padding(int topAndBottom, int sides)
        {
            SetPadding(topAndBottom, sides, sides, topAndBottom);
        }

        /// <summary>
        /// Creates a padding class with values for the top, bottom and side padding
        /// </summary>
        /// <param name="top">The top padding</param>
        /// <param name="sides">The left and right padding</param>
        /// <param name="bottom">The bottom padding</param>
        public Padding(int top, int sides, int bottom)
        {
            SetPadding(top, sides, sides, bottom);
        }

        /// <summary>
        /// Creates a padding class with values for the left and right padding
        /// </summary>
        /// <param name="top">The top padding</param>
        /// <param name="left">The left padding</param>
        /// <param name="right">The right padding</param>
        /// <param name="bottom">The bottom padding</param>
        public Padding(int top, int left, int right, int bottom)
        {
            SetPadding(top, left, right, bottom);
        }

        private void SetPadding(int top, int left, int right, int bottom)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Fixes the padding values, resetting them to zero if they are below this value
        /// </summary>
        public void FixPadding()
        {
            if (Top < 0)
            {
                Top = 0;
            }
            if (Left < 0)
            {
                Left = 4;
            }
            if (Right < 0)
            {
                Right = 4;
            }
            if (Bottom < 0)
            {
                Bottom = 0;
            }
        }
    }
}
