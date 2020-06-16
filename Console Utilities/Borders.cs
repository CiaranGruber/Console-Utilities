using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUtilities
{
    /// <summary>
    /// Represents a set of borders with specific 
    /// </summary>
    public class Borders
    {
        /// <summary>Gets or sets whether the top border is enabled</summary>
        public bool TopEnabled { get; set; } = true;
        /// <summary>Gets or sets whether the left border is enabled</summary>
        public bool LeftEnabled { get; set; } = true;
        /// <summary>Gets or sets whether the right border is enabled</summary>
        public bool RightEnabled { get; set; } = true;
        /// <summary>Gets or sets whether the bottom border is enabled</summary>
        public bool BottomEnabled { get; set; } = true;

        /// <summary>Gets or sets the character used for the top border</summary>
        public string TopStyle { get; set; } = "-";
        /// <summary>Gets or sets the character used for the left border</summary>
        public string LeftStyle { get; set; } = "|";
        /// <summary>Gets or sets the character used for the right border</summary>
        public string RightStyle { get; set; } = "|";
        /// <summary>Gets or sets the character used for the bottom border</summary>
        public string BottomStyle { get; set; } = "-";

        /// <summary>
        /// Creates the borders with the default values
        /// </summary>
        public Borders()
        {

        }

        /// <summary>
        /// Creates the border style with all enabled/disabled
        /// </summary>
        /// <param name="allEnabled">Whether the borders are all enabled (setting to false keeps them disabled)</param>
        public Borders(bool allEnabled)
        {
            TopEnabled = allEnabled;
            LeftEnabled = allEnabled;
            RightEnabled = allEnabled;
            BottomEnabled = allEnabled;
        }

        /// <summary>
        /// Returns if any borders are enabled
        /// </summary>
        /// <returns>Whether any borders are enabled</returns>
        public bool AnyEnabled()
        {
            return TopEnabled || LeftEnabled || RightEnabled || BottomEnabled;
        }
    }
}
