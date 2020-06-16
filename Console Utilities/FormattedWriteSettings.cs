using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUtilities
{
    /// <summary>
    /// Represents a set of settings for the FormattedWrite method in ConsoleUtils
    /// </summary>
    public class FormattedWriteSettings
    {
        /// <summary>
        /// <para>Gets or sets whether a new line will be created (cursor will move to next line at the end)</para>
        /// <para>Default: true</para>
        /// </summary>
        public bool NewLine { get; set; } = true;

        /// <summary>
        /// <para>Gets or sets the padding for the text</para>
        /// <para>Default: new Padding(0, 4)</para>
        /// </summary>
        public Padding InnerPadding { get; set; } = new Padding();

        /// <summary>
        /// <para>Gets or sets whether to wrap the words at all</para>
        /// <para>Default: true</para>
        /// </summary>
        public bool WrapWords { get; set; } = true;

        /// <summary>
        /// <para>Gets or sets how the text is aligned</para>
        /// <para>Default: Alignment.Left</para>
        /// </summary>
        public Alignment TextAlignment { get; set; } = Alignment.Left;

        /// <summary>
        /// <para>Gets or sets whether the text is justified</para>
        /// <para>Default: false</para>
        /// </summary>
        public bool Justified { get; set; } = false;

        /// <summary>
        /// <para>Gets or sets the minimum amount to justify by</para>
        /// <para>Default: new MinimumJustification(70, true)</para>
        /// </summary>
        public MinimumJustification MinToJustify { get; set; } = new MinimumJustification(70, true);

        /// <summary>
        /// <para>Gets or sets the minimum width of the text. If it is justified, it will always justify to this size.
        /// Otherwise it will merely always overwrite to this size</para>
        /// <para>Default: 0</para>
        /// </summary>
        public int MinimumWidth { get; set; } = 0;

        /// <summary>
        /// <para>Gets or sets the maximum width that the text and padding can take up</para>
        /// <para>Default: 0</para>
        /// </summary>
        public int MaximumWidth { get; set; } = 0;

        /// <summary>
        /// <para>Gets or sets the maximum height that the text and padding can take up. </para>
        /// <para>Default: int.MaxValue</para>
        /// <paramref name="WARNING"/>: Text will be cut if it becomes greater than this value (an ellipsis will be used at the end of the last line)
        /// </summary>
        public int MaximumHeight { get; set; } = int.MaxValue;

        /// <summary>
        /// <para>Gets or sets whether to write the text character by character</para>
        /// <para>Default: false</para>
        /// </summary>
        public bool SlowWrite { get; set; } = false;

        /// <summary>
        /// <para>Gets or sets the amount of time between each letter being created if SlowWrite is enabled</para>
        /// <para>Default: 50</para>
        /// </summary>
        public int ThreadGap { get; set; } = 50;

        /// <summary>
        /// <para>Gets or sets whether the left position of the cursor becomes the default position for the paragraph 
        /// rather than text moving to the next line (good for looks similar to a GUI)</para>
        /// <para>Default: true</para>
        /// </summary>
        public bool KeepIndent { get; set; } = true;

        /// <summary>
        /// <para>Gets or sets the starting write location of the text</para>
        /// <para>Default: new Location(Console.CursorLeft, Console.CursorTop)</para>
        /// </summary>
        public Location Location { get; set; } = new Location();

        /// <summary>
        /// <para>Gets or sets whether to reset the cursor position to the original location after writing</para>
        /// <para>Default: false</para>
        /// </summary>
        public bool ResetCursorPosition { get; set; } = false;

        /// <summary>
        /// <para>Gets or sets whether to show the cursor as the text is being written (more for if SlowWrite is enabled)</para>
        /// <para>Default: false</para>
        /// </summary>
        public bool ShowCursor { get; set; } = false;

        /// <summary>
        /// <para>Gets or sets the borders that surround the area taken by the text</para>
        /// <para>Default: new Borders(false)</para>
        /// </summary>
        public Borders AreaBorderSettings { get; set; } = new Borders(false);

        /// <summary>
        /// <para>Gets or sets the borders surrounding the text (no padding)</para>
        /// <para>Default: new Borders(false)</para>
        /// </summary>
        public Borders TextBorderSettings { get; set; } = new Borders(false);

        /// <summary>
        /// Initialises a <c>FormattedWriteSettings</c> instance with the default values
        /// </summary>
        public FormattedWriteSettings()
        {

        }

        /// <summary>
        /// Initialises a <c>FormattedWriteSettings</c> instance with the parent values
        /// </summary>
        /// <param name="parentSettings">The parent write settings to extend from</param>
        public FormattedWriteSettings(FormattedWriteSettings parentSettings)
        {
            NewLine = parentSettings.NewLine;

            InnerPadding = new Padding
            {
                Top = parentSettings.InnerPadding.Top,
                Left = parentSettings.InnerPadding.Left,
                Right = parentSettings.InnerPadding.Right,
                Bottom = parentSettings.InnerPadding.Bottom
            };

            WrapWords = parentSettings.WrapWords;

            TextAlignment = parentSettings.TextAlignment;
            Justified = parentSettings.Justified;
            MinToJustify = new MinimumJustification(parentSettings.MinToJustify.Value, parentSettings.MinToJustify.IsPercentage);

            MinimumWidth = parentSettings.MinimumWidth;
            MaximumWidth = parentSettings.MaximumWidth;
            MaximumHeight = parentSettings.MaximumHeight;

            SlowWrite = parentSettings.SlowWrite;
            ThreadGap = parentSettings.ThreadGap;

            KeepIndent = parentSettings.KeepIndent;
            Location = new Location(parentSettings.Location.X, parentSettings.Location.Y);

            ResetCursorPosition = parentSettings.ResetCursorPosition;
            ShowCursor = parentSettings.ShowCursor;

            AreaBorderSettings = new Borders
            {
                TopEnabled = parentSettings.AreaBorderSettings.TopEnabled,
                LeftEnabled = parentSettings.AreaBorderSettings.LeftEnabled,
                RightEnabled = parentSettings.AreaBorderSettings.RightEnabled,
                BottomEnabled = parentSettings.AreaBorderSettings.BottomEnabled,

                TopStyle = parentSettings.AreaBorderSettings.TopStyle,
                LeftStyle = parentSettings.AreaBorderSettings.LeftStyle,
                RightStyle = parentSettings.AreaBorderSettings.RightStyle,
                BottomStyle = parentSettings.AreaBorderSettings.BottomStyle,
            };
        }

        /// <summary>
        /// Resets the location to the current cursor position
        /// </summary>
        public void ResetLocation()
        {
            Location = new Location();
        }
    }
}
