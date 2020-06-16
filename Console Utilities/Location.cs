using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUtilities
{
    /// <summary>
    /// Represents a location with X and Y co-ordinates
    /// </summary>
    public class Location
    {
        /// <summary>Gets or sets the X co-ordinate</summary>
        public int X { get; set; }

        /// <summary>Gets or sets the Y co-ordinate</summary>
        public int Y { get; set; }

        /// <summary>
        /// Sets the location with the cursor's co-ordinates
        /// </summary>
        public Location()
        {
            X = Console.CursorLeft;
            Y = Console.CursorTop;
        }

        /// <summary>Sets the location with co-ordinates</summary>
        /// <param name="x">The X co-ordinate</param>
        /// <param name="y">The Y co-ordinate</param>
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
