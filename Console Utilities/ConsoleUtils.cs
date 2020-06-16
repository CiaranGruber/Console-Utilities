using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleUtilities
{
    /// <summary>
    /// Contains code for classes specifically for modifying the console. Paired with <c>GeneralUtils</c>
    /// </summary>
    public static class ConsoleUtils
    {
        /// <summary>
        /// Writes a blank line to the console with the default settings
        /// </summary>
        /// <returns>The most right and down location of the text</returns>
        public static Location FormattedWrite()
        {
            return FormattedWrite("", new FormattedWriteSettings());
        }

        /// <summary>
        /// Writes to the console with the default settings
        /// </summary>
        /// <param name="text">The text that is to be written</param>
        /// <returns>The most right and down location of the text</returns>
        public static Location FormattedWrite(object text)
        {
            return FormattedWrite(text, new FormattedWriteSettings());
        }

        /// <summary>
        /// Writes to the console with various settings
        /// </summary>
        /// <param name="text">The text that is to be written</param>
        /// <param name="settings">The settings included for the FormattedWrite</param>
        /// <returns>The most right and down location of the text</returns>
        public static Location FormattedWrite(object text, FormattedWriteSettings settings)
        {
            // Sets variables
            int availableWidth;
            int availableHeight = settings.MaximumHeight;
            int spacesToAdd;
            int lineCount = 1;
            int word = 0;
            int widestLine = 0;
            bool cursorShownOriginally = Console.CursorVisible;
            int terminalWidth = Console.WindowWidth;
            string currentLine;
            string nextWord;
            string rawText = Convert.ToString(text);
            List<string> printingList = new List<string>();
            Console.CursorVisible = settings.ShowCursor;

            // Checks and sets cursor positioning
            Location returnPosition = new Location(0, 0);
            int[] startingCursorPosition = new int[] { Console.CursorLeft, Console.CursorTop };

            if (settings.Location.X + settings.InnerPadding.Left >= Console.BufferWidth ||
                settings.Location.Y + settings.InnerPadding.Top >= Console.BufferHeight)
            {
                return settings.Location;
            }
            Console.SetCursorPosition(settings.Location.X, settings.Location.Y);

            Location cursorPosition = new Location(Console.CursorLeft, Console.CursorTop);


            // Checks if text is null
            if (rawText == null)
            {
                rawText = "";
            }

            // Fixes padding
            settings.InnerPadding.FixPadding();

            // Sets minimum and maximum width
            if (settings.MaximumWidth < settings.MinimumWidth)
            {
                settings.MaximumWidth = settings.MinimumWidth;
            }

            // Sets the available width to write
            if (settings.MinimumWidth > 0 && settings.MinimumWidth > settings.InnerPadding.Left + settings.InnerPadding.Right && settings.MinimumWidth > terminalWidth)
            {
                availableWidth = settings.MinimumWidth;
            }
            else if (settings.MaximumWidth > 0 && settings.MaximumWidth > settings.InnerPadding.Left + settings.InnerPadding.Right)
            {
                availableWidth = settings.MaximumWidth - settings.InnerPadding.Left - settings.InnerPadding.Right;
            }
            else if (settings.MaximumWidth > 0 && terminalWidth - settings.InnerPadding.Left - settings.InnerPadding.Right > settings.MaximumWidth)
            {
                availableWidth = 1;
            }
            else
            {
                availableWidth = terminalWidth - settings.InnerPadding.Left - settings.InnerPadding.Right;
            }

            // Fixes padding if available width is too small
            if (availableWidth < 1)
            {
                availableWidth = 1;
                settings.InnerPadding.Left = (terminalWidth - 1) / 2;
                settings.InnerPadding.Right = settings.InnerPadding.Left;
            }

            // Gets the list to print
            string[] rawTextWords = rawText.Split(' ');
            nextWord = rawTextWords[word];
            while ((word < rawTextWords.Length || nextWord != "") && lineCount <= availableHeight)
            {
                lineCount++;
                currentLine = "";

                // If word is bigger than the available width
                if (nextWord.Length > availableWidth)
                {
                    int letterIndex = 0;
                    while (letterIndex < nextWord.Length)
                    {
                        currentLine = "";
                        while (letterIndex < nextWord.Length && currentLine.Length < availableWidth)
                        {
                            currentLine += nextWord[letterIndex];
                            letterIndex++;
                        }
                        printingList.Add(currentLine);
                    }
                    word++;
                    if (word < rawTextWords.Length)
                    {
                        nextWord = rawTextWords[word];
                    }
                    else
                    {
                        nextWord = "";
                    }
                }
                else
                {
                    // Add words until no space left
                    while (word < rawTextWords.Length && currentLine.Length + nextWord.Length <= availableWidth)
                    {
                        currentLine += nextWord + ' ';
                        word++;
                        if (word < rawTextWords.Length)
                        {
                            nextWord = rawTextWords[word];
                        }

                        else
                        {
                            nextWord = "";
                        }
                    }

                    // Adds as much of the next word as possible if wrapWords is disabled
                    if (!settings.WrapWords && currentLine.Length + nextWord.Length > availableWidth)
                    {
                        string decreasedWord = "";
                        foreach (char character in nextWord)
                        {
                            if (currentLine.Length > availableWidth)
                            {
                                decreasedWord += character;
                            }
                            else
                            {
                                currentLine += character;
                            }
                        }
                        nextWord = decreasedWord;
                        currentLine += ' ';
                    }

                    // Remove trailing space
                    if (currentLine != "")
                    {
                        currentLine = currentLine.Remove(currentLine.Length - 1);

                        // Adds spaces if it has the justified alignment
                        if (settings.Justified && currentLine.Length != availableWidth && (currentLine.Length > settings.MinToJustify.GetAbsoluteValue(availableWidth) || currentLine.Length < settings.MinimumWidth))
                        {
                            int numberOfWords = currentLine.Split(' ').Length - 1;
                            int letterIndex = 0;
                            if (currentLine.Length > settings.MinToJustify.GetAbsoluteValue(availableWidth))
                            {
                                spacesToAdd = availableWidth - currentLine.Length;
                            }
                            else
                            {
                                spacesToAdd = settings.MinimumWidth - currentLine.Length;
                            }
                            List<char> justificationList = currentLine.ToCharArray().ToList();

                            // Adds spaces
                            while (spacesToAdd > 0)
                            {
                                while (justificationList[letterIndex] != ' ') letterIndex++;
                                for (int i = 0; i < (int)Math.Ceiling((double)spacesToAdd / numberOfWords); i++)
                                {
                                    justificationList.Insert(letterIndex, ' ');
                                    letterIndex++;
                                }
                                spacesToAdd -= (int)Math.Ceiling((double)spacesToAdd / numberOfWords);
                                numberOfWords--;
                                letterIndex++;
                            }

                            // Sets currentLine
                            currentLine = new string(justificationList.ToArray());
                        }

                        // Fixes any minimum width problems
                        if (currentLine.Length < settings.MinimumWidth)
                        {
                            while (currentLine.Length < settings.MinimumWidth)
                            {
                                currentLine += ' ';
                            }
                        }

                        // Adds the line and splits it if any \n (newline) chars are found 
                        if (currentLine.Contains('\n'))
                        {
                            printingList.AddRange(currentLine.Split('\n'));
                        }
                        else
                        {
                            printingList.Add(currentLine);
                        }
                    }
                }
            }

            // Gets the widest line
            foreach (string printingLine in printingList)
            {
                if (printingLine.Length > widestLine)
                {
                    widestLine = printingLine.Length;
                }
            }

            // If border and keepIndent is enabled, create the border
            if (settings.AreaBorderSettings.AnyEnabled() && settings.KeepIndent)
            {
                if (settings.TextAlignment == Alignment.Left)
                {
                    returnPosition = new Location(cursorPosition.X + widestLine + settings.InnerPadding.Left + settings.InnerPadding.Right, cursorPosition.Y + printingList.Count + settings.InnerPadding.Top + settings.InnerPadding.Bottom + 1);
                }
                else
                {
                    returnPosition = new Location(cursorPosition.X + availableWidth + settings.InnerPadding.Left + settings.InnerPadding.Right, cursorPosition.Y + printingList.Count + settings.InnerPadding.Top + settings.InnerPadding.Bottom + 1);
                }

                if (settings.AreaBorderSettings.LeftEnabled) returnPosition.X += settings.AreaBorderSettings.LeftStyle.Length;
                if (settings.AreaBorderSettings.RightEnabled) returnPosition.X += settings.AreaBorderSettings.RightStyle.Length;
                if (settings.AreaBorderSettings.TopEnabled) returnPosition.Y += settings.AreaBorderSettings.TopStyle.Length;
                if (settings.AreaBorderSettings.BottomEnabled) returnPosition.Y += settings.AreaBorderSettings.BottomStyle.Length;

                if (settings.TextBorderSettings.LeftEnabled) returnPosition.X += settings.TextBorderSettings.LeftStyle.Length;
                if (settings.TextBorderSettings.RightEnabled) returnPosition.X += settings.TextBorderSettings.RightStyle.Length;
                if (settings.TextBorderSettings.TopEnabled) returnPosition.Y += settings.TextBorderSettings.TopStyle.Length;
                if (settings.TextBorderSettings.BottomEnabled) returnPosition.Y += settings.TextBorderSettings.BottomStyle.Length;

                CreateBox(cursorPosition, returnPosition, settings.AreaBorderSettings);
            }
            cursorPosition.Y += settings.InnerPadding.Top;
            if (settings.AreaBorderSettings.TopEnabled) cursorPosition.Y += settings.AreaBorderSettings.TopStyle.Length;
            if (settings.TextBorderSettings.TopEnabled) cursorPosition.Y += settings.TextBorderSettings.TopStyle.Length;

            // Creates borders around the text specifically
            if (settings.TextBorderSettings.AnyEnabled() && settings.KeepIndent)
            {
                Location startingPosition = new Location(cursorPosition.X, cursorPosition.Y);
                if (settings.AreaBorderSettings.LeftEnabled)
                {
                    startingPosition.X += settings.AreaBorderSettings.LeftStyle.Length;
                }
                if (settings.TextBorderSettings.LeftEnabled)
                {
                    startingPosition.X += settings.TextBorderSettings.LeftStyle.Length;
                }

                if (settings.TextAlignment == Alignment.Centre)
                {
                    startingPosition.X += (availableWidth - widestLine) / 2;
                }
                else if (settings.TextAlignment == Alignment.Right)
                {
                    startingPosition.X += availableWidth - widestLine;
                }
                startingPosition.X += settings.InnerPadding.Left;

                CreateBorder(startingPosition, new Location(startingPosition.X + widestLine, startingPosition.Y + printingList.Count - 1), new Padding(0), settings.TextBorderSettings);
            }

            Console.SetCursorPosition(cursorPosition.X, cursorPosition.Y);


            // Print the printing list
            for (int x = 0; x < printingList.Count; x++)
            {
                // Add padding
                Console.SetCursorPosition(Console.CursorLeft + settings.InnerPadding.Left, Console.CursorTop);
                if (settings.AreaBorderSettings.LeftEnabled)
                {
                    Console.CursorLeft += settings.AreaBorderSettings.LeftStyle.Length;
                }
                if (settings.TextBorderSettings.LeftEnabled)
                {
                    Console.CursorLeft += settings.TextBorderSettings.LeftStyle.Length;
                }

                // Moves the cursor to the correct position for alignment
                if (settings.TextAlignment == Alignment.Right)
                {
                    Console.SetCursorPosition(Console.CursorLeft + availableWidth - printingList[x].Length, Console.CursorTop);
                }
                else if (settings.TextAlignment == Alignment.Centre)
                {
                    Console.SetCursorPosition(Console.CursorLeft + (availableWidth - printingList[x].Length) / 2, Console.CursorTop);
                }

                // Print text
                if (settings.SlowWrite)
                {
                    for (int i = 0; i < printingList[x].Length; i++)
                    {
                        Console.Write(printingList[x][i]);
                        if (Console.KeyAvailable)
                        {
                            settings.SlowWrite = false;
                        }
                        if (settings.SlowWrite)
                        {
                            Thread.Sleep(settings.ThreadGap);
                        }
                    }
                    while (Console.KeyAvailable) Console.ReadKey(true);
                }
                else
                {
                    Console.Write(printingList[x]);
                }

                // Sets widest line for the return location
                if (Console.CursorLeft > returnPosition.X)
                {
                    returnPosition.X = Console.CursorLeft;
                }
                if (Console.CursorTop > returnPosition.Y)
                {
                    returnPosition.Y = Console.CursorTop;
                }

                // Moves the cursor if it is not the last line or if newLine is enabled
                if (settings.NewLine || x < printingList.Count - 1)
                {
                    if (settings.KeepIndent)
                    {
                        cursorPosition.Y++;
                        Console.SetCursorPosition(cursorPosition.X, cursorPosition.Y);
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
            }

            // Add bottom padding if newLine is enabled
            if (settings.NewLine)
            {
                for (int x = 0; x < settings.InnerPadding.Bottom; x++)
                {
                    if (settings.KeepIndent)
                    {
                        cursorPosition.Y++;
                        Console.SetCursorPosition(cursorPosition.X, cursorPosition.Y);
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
            }

            if (settings.ResetCursorPosition)
            {
                Console.SetCursorPosition(startingCursorPosition[0], startingCursorPosition[1]);
            }
            Console.CursorVisible = cursorShownOriginally;
            return returnPosition;
        }

        /// <summary>
        /// Preps the screen by clearing the console and adding a header
        /// </summary>
        /// <param name="title">Title for the header</param>
        /// <param name="lineIncluded">Whether the line for the header is included</param>
        public static void PrepNewScreen(string title = "", bool lineIncluded = true)
        {
            Console.Clear();
            FormattedWrite(title, new FormattedWriteSettings() { TextAlignment = Alignment.Centre, InnerPadding = new Padding(1, 0, 0) });

            // Adds the line if it is included
            if (lineIncluded)
            {
                FormattedWrite("".PadLeft(Console.WindowWidth, '_'), new FormattedWriteSettings() { InnerPadding = new Padding(0, 0, 2) });
            }
        }

        /// <summary>
        /// Creates a menu with various menu options and default title
        /// </summary>
        /// <param name="menuOptions">The options presented by the menu</param>
        public static void CreateMenu(ICollection menuOptions)
        {
            CreateMenu(menuOptions, "Menu:", new FormattedWriteSettings(), new FormattedWriteSettings());
        }

        /// <summary>
        /// Creates a menu with various menu options and a custom title
        /// </summary>
        /// <param name="menuOptions">The options presented by the menu</param>
        /// <param name="menuTitle">The menu title</param>
        public static void CreateMenu(ICollection menuOptions, string menuTitle)
        {
            CreateMenu(menuOptions, menuTitle, new FormattedWriteSettings(), new FormattedWriteSettings());
        }

        /// <summary>
        /// Creates a menu with various menu options, custom title and write settings
        /// </summary>
        /// <param name="menuOptions">The options presented by the menu</param>
        /// <param name="menuTitle">The menu title</param>
        /// <param name="writeSettings">Any extra write settings for the title and options</param>
        public static void CreateMenu(ICollection menuOptions, string menuTitle, FormattedWriteSettings writeSettings)
        {
            CreateMenu(menuOptions, menuTitle, writeSettings, writeSettings);
        }

        /// <summary>
        /// Creates a menu with various menu options, custom titles and separate write settings for the title and menu options
        /// </summary>
        /// <param name="menuOptions">The options presented by the menu</param>
        /// <param name="menuTitle">The menu title</param>
        /// <param name="menuWriteSettings">Any extra write settings for the title</param>
        /// <param name="optionsWriteSettings">Any extra write settings for the options</param>
        public static void CreateMenu(ICollection menuOptions, string menuTitle, FormattedWriteSettings menuWriteSettings, FormattedWriteSettings optionsWriteSettings)
        {
            int menuIndex = 1;

            // Writes the title
            FormattedWrite(menuTitle, menuWriteSettings);
            menuWriteSettings.Location.Y += 1;

            // Writes each menu option
            foreach (object menuOption in menuOptions)
            {
                FormattedWrite(menuIndex + ". " + menuOption.ToString(), menuWriteSettings);
                menuWriteSettings.Location.Y += 1;
                menuIndex++;
            }
        }

        /// <summary>
        /// Clears a line completely
        /// </summary>
        /// <param name="line">The line where it is cleared</param>
        public static void ClearLine(int line)
        {
            ClearLine(line, 0, Console.WindowWidth);
        }

        /// <summary>
        /// Clears a line from a specific digit
        /// </summary>
        /// <param name="line">The line where it is cleared</param>
        /// <param name="startingDigit">The digit to start clearing from</param>
        public static void ClearLine(int line, int startingDigit)
        {
            ClearLine(line, startingDigit, Console.WindowWidth - startingDigit);
        }

        /// <summary>
        /// Clears an area between certain values
        /// </summary>
        /// <param name="line">The line where it is cleared</param>
        /// <param name="startingDigit">The digit to start clearing from</param>
        /// <param name="count">The digit to finish upon</param>
        public static void ClearLine(int line, int startingDigit, int count)
        {
            int currentLine = Console.CursorTop;
            int currentDigit = Console.CursorLeft;

            // Clears line between limits if there is anything to clear
            Console.CursorVisible = false;
            Console.SetCursorPosition(startingDigit, line);
            Console.Write("".PadLeft(count));
            Console.SetCursorPosition(currentDigit, currentLine);
            Console.CursorVisible = true;
        }

        /// <summary>
        /// Clears an area between certain values
        /// </summary>
        /// <param name="startingDigit">The digit to start clearing from</param>
        /// <param name="count">The digit to finish upon</param>
        /// <param name="line">The line where it is cleared</param>
        /// <param name="finishingLine">The line to finish clearing on (inclusive)</param>
        public static void ClearArea(Location startingLocation, Location finishingLocation)
        {
            int currentLine = Console.CursorTop;
            int currentDigit = Console.CursorLeft;

            // Prevent memory overflow by maximising PadLeft size
            if (finishingLocation.X > Console.WindowWidth)
            {
                finishingLocation.X = Console.WindowWidth;
            }

            // Clears line between limits if there is anything to clear
            Console.CursorVisible = false;
            if (finishingLocation.Y >= startingLocation.Y)
            {
                for (int i = 0; i <= finishingLocation.Y - startingLocation.Y; i++)
                {
                    Console.SetCursorPosition(startingLocation.X, startingLocation.Y + i);
                    Console.Write("".PadLeft(finishingLocation.X - startingLocation.X));
                }
            }
            Console.SetCursorPosition(currentDigit, currentLine);
            Console.CursorVisible = true;
        }

        /// <summary>
        /// Creates a box using the default border styles
        /// </summary>
        /// <param name="topleft">The top left location of the box</param>
        /// <param name="bottomRight">The bottom right location of the box (exclusive bounds)</param>
        public static void CreateBox(Location topleft, Location bottomRight)
        {
            CreateBox(topleft, bottomRight, new Borders());
        }

        /// <summary>
        /// Creates a box using the default border styles
        /// </summary>
        /// <param name="topLeft">The top left location of the box</param>
        /// <param name="bottomRight">The bottom right location of the box (exclusive bounds)</param>
        /// <param name="borders">The border style</param>
        public static void CreateBox(Location topLeft, Location bottomRight, Borders borders)
        {
            Location originalCursorPosition = new Location();
            Location customPosition = new Location(topLeft.X, topLeft.Y);

            // Create top border
            if (borders.TopEnabled)
            {
                for (int i = 0; i < borders.TopStyle.Length; i++)
                {
                    Console.SetCursorPosition(topLeft.X, customPosition.Y);
                    Console.Write("".PadLeft(bottomRight.X - topLeft.X, borders.TopStyle[i]));
                    customPosition.Y++;
                }
            }

            // Create middle borders
            if (topLeft.X != bottomRight.X)
            {
                // Gets times to repeat, removing repeats depending on top/bottom borders
                int repeats = bottomRight.Y - topLeft.Y - 1;
                if (borders.TopEnabled)
                {
                    repeats -= borders.TopStyle.Length;
                }
                if (borders.BottomEnabled)
                {
                    repeats -= borders.BottomStyle.Length;
                }

                // Create borders
                for (int i = 0; i < repeats; i++)
                {
                    if (borders.LeftEnabled)
                    {
                        Console.SetCursorPosition(topLeft.X, customPosition.Y);
                        Console.Write(borders.LeftStyle);
                    }
                    if (borders.RightEnabled)
                    {
                        Console.SetCursorPosition(bottomRight.X - borders.RightStyle.Length, customPosition.Y);
                        Console.Write(borders.RightStyle);
                    }
                    customPosition.Y++;
                }
            }

            // Create bottom border
            if (borders.BottomEnabled)
            {
                for (int i = 0; i < borders.BottomStyle.Length; i++)
                {
                    Console.SetCursorPosition(topLeft.X, customPosition.Y);
                    Console.Write("".PadLeft(bottomRight.X - topLeft.X, borders.BottomStyle[i]));
                    customPosition.Y++;
                }
            }

            // Reset cursor position
            Console.SetCursorPosition(originalCursorPosition.X, originalCursorPosition.Y);
        }

        /// <summary>
        /// Creates a border around a location with 1-gap padding using the default border styles
        /// </summary>
        /// <param name="topleft">The top left location of the box</param>
        /// <param name="bottomRight">The bottom right location of the box (exclusive bounds)</param>
        public static void CreateBorder(Location topleft, Location bottomRight)
        {
            CreateBorder(topleft, bottomRight, new Padding(1), new Borders());
        }

        /// <summary>
        /// Creates a border around a location with padding using the default border styles
        /// </summary>
        /// <param name="topleft">The top left location of the box</param>
        /// <param name="bottomRight">The bottom right location of the box (exclusive bounds)</param>
        /// <param name="padding">Any padding around the area</param>
        public static void CreateBorder(Location topleft, Location bottomRight, Padding padding)
        {
            CreateBorder(topleft, bottomRight, padding, new Borders());
        }

        /// <summary>
        /// Creates a border around a location with padding using the default border styles
        /// </summary>
        /// <param name="topLeft">The top left location of the box</param>
        /// <param name="bottomRight">The bottom right location of the box (exclusive bounds)</param>
        /// <param name="padding">Any padding around the area</param>
        /// <param name="borders">The border style</param>
        public static void CreateBorder(Location topLeft, Location bottomRight, Padding padding, Borders borders)
        {
            Location originalCursorPosition = new Location();

            // Gets the required horizontal width depending on original width, padding and border sizes
            int horizontalWidth = bottomRight.X - topLeft.X + padding.Left + padding.Right;
            if (borders.LeftEnabled)
            {
                horizontalWidth += borders.LeftStyle.Length;
            }
            if (borders.RightEnabled)
            {
                horizontalWidth += borders.RightStyle.Length;
            }

            // Create top border
            if (borders.TopEnabled)
            {
                for (int i = 0; i < borders.TopStyle.Length; i++)
                {
                    if (borders.LeftEnabled)
                    {
                        Console.SetCursorPosition(topLeft.X - padding.Left - borders.LeftStyle.Length, topLeft.Y - borders.TopStyle.Length + i - padding.Top);
                    }
                    else
                    {
                        Console.SetCursorPosition(topLeft.X - padding.Left, topLeft.Y - borders.TopStyle.Length + i - padding.Top);
                    }
                    Console.Write("".PadLeft(horizontalWidth, borders.TopStyle[i]));
                }
            }

            // Create middle borders
            if (topLeft.X != bottomRight.X)
            {
                for (int i = 0; i < bottomRight.Y - topLeft.Y + padding.Top + padding.Bottom + 1; i++)
                {
                    if (borders.LeftEnabled)
                    {
                        Console.SetCursorPosition(topLeft.X - padding.Left - borders.LeftStyle.Length, topLeft.Y + i - padding.Top);
                        Console.Write(borders.LeftStyle);
                    }
                    if (borders.RightEnabled)
                    {
                        Console.SetCursorPosition(bottomRight.X + padding.Right, topLeft.Y + i - padding.Top);
                        Console.Write(borders.RightStyle);
                    }
                }
            }

            // Create bottom border
            if (borders.BottomEnabled)
            {
                for (int i = 0; i < borders.BottomStyle.Length; i++)
                {
                    if (borders.LeftEnabled)
                    {
                        Console.SetCursorPosition(topLeft.X - padding.Left - borders.LeftStyle.Length, bottomRight.Y + padding.Bottom + i + 1);
                    }
                    else
                    {
                        Console.SetCursorPosition(topLeft.X - padding.Left, bottomRight.Y + padding.Bottom + i + 1);
                    }
                    Console.Write("".PadLeft(horizontalWidth, borders.BottomStyle[i]));
                }
            }

            // Reset cursor position
            Console.SetCursorPosition(originalCursorPosition.X, originalCursorPosition.Y);
        }

        /// <summary>
        /// Gets an unchecked input with the option to clear input afterwards and the default prompt "Input: "
        /// </summary>
        /// <param name="clearInput">Whether to clear the input after it has been entered</param>
        /// <returns>The user-inputted input</returns>
        public static string FancyGetInput(bool clearInput = false)
        {
            return FancyGetInput("Input: ", int.MaxValue, 1, new FormattedWriteSettings() { NewLine = false }, clearInput);
        }

        /// <summary>
        /// Gets an unchecked input with the option to clear input afterwards
        /// </summary>
        /// <param name="prompt">The prompt that will be given</param>
        /// <param name="clearInput">Whether to clear the input after it has been entered</param>
        /// <returns>The user-inputted input</returns>
        public static string FancyGetInput(string prompt, bool clearInput = false)
        {
            return FancyGetInput(prompt, int.MaxValue, 1, new FormattedWriteSettings() { NewLine = false }, clearInput);
        }

        /// <summary>
        /// Gets an unchecked input with basic user limits and the option to clear input afterwards
        /// </summary>
        /// <param name="prompt">The prompt that will be given</param>
        /// <param name="maxLength">The maximum length of the user input</param>
        /// <param name="clearInput">Whether to clear the input after it has been entered</param>
        /// <returns>The user-inputted input</returns>
        public static string FancyGetInput(string prompt, int maxLength, bool clearInput = false)
        {
            return FancyGetInput(prompt, maxLength, int.MaxValue, new FormattedWriteSettings() { NewLine = false }, clearInput);
        }

        /// <summary>
        /// Gets an unchecked input with user limits and the option to clear input afterwards
        /// </summary>
        /// <param name="prompt">The prompt that will be given</param>
        /// <param name="maxLength">The maximum length of the user input</param>
        /// <param name="maxHeight">The maximum amount of lines the user can use</param>
        /// <param name="clearInput">Whether to clear the input after it has been entered</param>
        /// <returns>The user-inputted input</returns>
        public static string FancyGetInput(string prompt, int maxLength, int maxHeight, bool clearInput = false)
        {
            return FancyGetInput(prompt, maxLength, maxHeight, new FormattedWriteSettings() { NewLine = false }, clearInput);
        }

        /// <summary>
        /// Gets an unchecked input with user limits, write settings and the option to clear input afterwards
        /// </summary>
        /// <param name="prompt">The prompt that will be given</param>
        /// <param name="maxLength">The maximum length of the user input from the start of the prompt position</param>
        /// <param name="maxHeight">The maximum amount of lines for the user input and prompt</param>
        /// <param name="writeSettings">Any extra write settings</param>
        /// <param name="clearInput">Whether to clear the input after it has been entered</param>
        /// <returns>The user-inputted input</returns>
        public static string FancyGetInput(string prompt, int maxLength, int maxHeight, FormattedWriteSettings writeSettings, bool clearInput = false)
        {
            string input;
            Location inputPosition;
            Location startingPosition = new Location();

            // Writes the question and gets the input position
            FormattedWrite(prompt, writeSettings);
            inputPosition = new Location();

            // Reduce maxLength depending on size of prompt
            maxLength -= inputPosition.X - startingPosition.X;

            // Get the input
            input = GetLimitedSizeInput(maxLength, maxHeight);

            // Clears the input (if set to) and sets the cursor to the next line
            if (clearInput)
            {
                ClearArea(inputPosition, new Location(inputPosition.X + maxLength, Console.CursorTop - 1));
                Console.SetCursorPosition(startingPosition.X, inputPosition.Y + 1);
            }
            else
            {
                Console.SetCursorPosition(startingPosition.X, Console.CursorTop);
            }

            return input;
        }

        /// <summary>
        /// Gets an integer with limits from the user
        /// </summary>
        /// <param name="prompt">The prompt to get the integer</param>
        /// <param name="errorPrompt">The error prompt that is given if input is not an integer</param>
        /// <param name="limitsPrompt">The limits prompt if integer is outside limits</param>
        /// <param name="minValue">The minimum value (inclusive)</param>
        /// <param name="maxValue">The maximum value (inclusive)</param>
        /// <param name="writeOptions">Any extra write options for the input text (and error prompts if these are not given)</param>
        /// <param name="errorWriteOptions">The write options for the errors</param>
        /// <param name="maxCharLength">The maximum length the the user input can take</param>
        /// <param name="maxCharHeight">The maximum height that the user input can take</param>
        /// <returns>The user-inputted integer</returns>
        public static int FancyGetInteger(string prompt = "Input: ", string errorPrompt = "Input must be an integer", string limitsPrompt = "Default", int minValue = int.MinValue, int maxValue = int.MaxValue, FormattedWriteSettings writeOptions = null, FormattedWriteSettings errorWriteOptions = null, int maxCharLength = int.MaxValue, int maxCharHeight = 1)
        {
            int integer = -1;
            bool invalidInput = true;
            bool hadError = false;
            Location finalCursorPosition = new Location(0, 0);
            Location promptFinalPos = new Location(0, 0);
            FormattedWriteSettings changedWriteOptions;

            // Sets the limits prompt if it is the default and get largest prompt (to get the deletion)
            if (limitsPrompt.ToLower() == "default")
            {
                limitsPrompt = "Integer must be " + minValue + " or greater and " + maxValue + " or less";
            }

            // Get prompt settings
            if (writeOptions == null)
            {
                writeOptions = new FormattedWriteSettings();
                writeOptions.NewLine = false;
            }
            if (errorWriteOptions == null)
            {
                changedWriteOptions = new FormattedWriteSettings(writeOptions);
            }
            else
            {
                changedWriteOptions = errorWriteOptions;
            }
            promptFinalPos.Y = writeOptions.Location.Y;

            // Create initial prompt
            Console.CursorTop = writeOptions.Location.Y;
            promptFinalPos = FormattedWrite(prompt, writeOptions);
            Location inputCursorPos = new Location();

            // Set error prompt location settings
            if (changedWriteOptions.Location.X == -1)
            {
                changedWriteOptions.Location.X = 0;
            }
            if (errorWriteOptions == null || changedWriteOptions.Location.Y == -1)
            {
                changedWriteOptions.InnerPadding.Top = 0;
                changedWriteOptions.Location.Y = promptFinalPos.Y + 2;
            }
            changedWriteOptions.ResetCursorPosition = true;

            // Gets the integer
            while (invalidInput)
            {
                try
                {
                    // Get the integer
                    Console.SetCursorPosition(inputCursorPos.X, inputCursorPos.Y);
                    integer = Convert.ToInt32(GetLimitedSizeInput(maxCharLength, maxCharHeight));

                    // Check if integer is within values
                    if (integer < minValue || integer > maxValue)
                    {
                        // Clear the input
                        ClearArea(inputCursorPos, new Location(inputCursorPos.X + maxCharLength, Console.CursorTop));

                        // Clear the space taken by the previous error
                        if (hadError)
                        {
                            ClearArea(changedWriteOptions.Location, finalCursorPosition);
                        }

                        // Write the error
                        finalCursorPosition = FormattedWrite(limitsPrompt, changedWriteOptions);
                        hadError = true;
                    }
                    else
                    {
                        invalidInput = false;
                    }
                }
                catch
                {
                    // Clear the input
                    ClearArea(inputCursorPos, new Location(inputCursorPos.X + maxCharLength, Console.CursorTop - 1));

                    // Clear the space taken by the previous error
                    if (hadError)
                    {
                        ClearArea(changedWriteOptions.Location, finalCursorPosition);
                    }

                    // Write the error
                    finalCursorPosition = FormattedWrite(errorPrompt, changedWriteOptions);
                    hadError = true;
                }
            }
            // Remove any errors that were created
            if (hadError)
            {
                ClearArea(changedWriteOptions.Location, finalCursorPosition);
            }

            // Reset cursor position
            Console.CursorLeft = writeOptions.Location.X;

            return integer;
        }

        /// <summary>
        /// Gets user input if it is one of the allowed inputs
        /// </summary>
        /// <param name="allowedInputs">The allowed values</param>
        /// <param name="prompt">The prompt to get the integer</param>
        /// <param name="errorPrompt">The error prompt that is given if input is not an integer</param>
        /// <param name="isCaseSensitive">Whether the input is case-sensitive or not</param>
        /// <param name="writeOptions">Any extra write options for the input text (and error prompts if these are not given)</param>
        /// <param name="errorWriteOptions">The write options for the errors</param>
        /// <param name="maxCharLength">The maximum length the the user input can take</param>
        /// <param name="maxCharHeight">The maximum height that the user input can take</param>
        /// <param name="preventIncorrectText">Whether to prevent the user from entering incorrect values</param>
        /// <returns>The user-inputted integer</returns>
        public static string FancyGetAllowedInput(IEnumerable<string> allowedInputs, string prompt = "Input: ", string errorPrompt = "Input is invalid", bool isCaseSensitive = true, FormattedWriteSettings writeOptions = null, FormattedWriteSettings errorWriteOptions = null, int maxCharLength = int.MaxValue, int maxCharHeight = 1, bool preventIncorrectText = false)
        {
            string input = "";
            bool invalidInput = true;
            bool hadError = false;
            Location finalCursorPosition = new Location(0, 0);
            Location promptFinalPos = new Location(0, 0);
            FormattedWriteSettings changedWriteOptions;

            // If inputs are not case-sensitive, make inputs all lowered
            if (!isCaseSensitive)
            {
                allowedInputs = allowedInputs.Select(x => x.ToLower());
            }

            // Fix char length if it is less than 0
            if (maxCharLength < 0)
            {
                maxCharLength = 0;
            }

            // Get prompt settings and set more error prompt settings
            if (writeOptions == null)
            {
                writeOptions = new FormattedWriteSettings() { NewLine = false };
            }
            if (errorWriteOptions == null)
            {
                changedWriteOptions = new FormattedWriteSettings(writeOptions);
            }
            else
            {
                changedWriteOptions = errorWriteOptions;
            }

            // Create initial prompt and set more error prompt settings
            Console.CursorTop = writeOptions.Location.Y;
            promptFinalPos = FormattedWrite(prompt, writeOptions);
            Location inputCursorPos = new Location();
            if (changedWriteOptions.Location.X == -1)
            {
                changedWriteOptions.Location.X = 0;
            }
            if (errorWriteOptions == null || changedWriteOptions.Location.Y == -1)
            {
                changedWriteOptions.Location.Y = promptFinalPos.Y + 2;
            }
            changedWriteOptions.ResetCursorPosition = true;

            // Gets the integer from the user
            while (invalidInput)
            {
                // Get the input
                Console.SetCursorPosition(inputCursorPos.X, inputCursorPos.Y);
                if (preventIncorrectText)
                {
                    input = GetSpecificInput(allowedInputs, maxCharLength, maxCharHeight);
                }
                else
                {
                    input = GetLimitedSizeInput(maxCharLength, maxCharHeight);
                }

                // Determine if input is valid
                if (isCaseSensitive)
                {
                    invalidInput = !allowedInputs.Contains(input);
                }
                else
                {
                    invalidInput = !allowedInputs.Contains(input.ToLower());
                }

                // Check if input is acceptable
                if (invalidInput)
                {
                    // Clear the input
                    Location secondLocation = new Location(0, Console.CursorTop);
                    if (inputCursorPos.X + maxCharLength < inputCursorPos.X)
                    {
                        secondLocation.X = int.MaxValue;
                    }
                    else
                    {
                        secondLocation.X = inputCursorPos.X + maxCharLength;
                    }
                    ClearArea(inputCursorPos, secondLocation);

                    // Clear the space taken by the previous error
                    if (hadError)
                    {
                        ClearArea(changedWriteOptions.Location, finalCursorPosition);
                    }

                    // Write the error
                    finalCursorPosition = FormattedWrite(errorPrompt, changedWriteOptions);
                    hadError = true;
                }
            }

            // Remove any errors that were created
            if (hadError)
            {
                ClearArea(changedWriteOptions.Location, finalCursorPosition);
            }

            // Reset cursor position
            Console.CursorLeft = writeOptions.Location.X;
            return input;
        }

        /// <summary>
        /// Gets user input if it is not one of the disallowed inputs
        /// </summary>
        /// <param name="unacceptableInputs">The allowed values</param>
        /// <param name="prompt">The prompt to get the integer</param>
        /// <param name="errorPrompt">The error prompt that is given if input is not an integer</param>
        /// <param name="isCaseSensitive">Whether the input is case-sensitive or not</param>
        /// <param name="writeOptions">Any extra write options for the input text (and error prompts if these are not given)</param>
        /// <param name="errorWriteOptions">The write options for the errors</param>
        /// <param name="maxCharLength">The maximum length the the user input can take</param>
        /// <param name="maxCharHeight">The maximum height that the user input can take</param>
        /// <returns>The user-inputted integer</returns>
        public static string FancyGetInverseAllowedInput(IEnumerable<string> unacceptableInputs, string prompt = "Input: ", string errorPrompt = "Input is invalid", bool isCaseSensitive = true, FormattedWriteSettings writeOptions = null, FormattedWriteSettings errorWriteOptions = null, int maxCharLength = int.MaxValue, int maxCharHeight = 1)
        {
            string input = "";
            bool invalidInput = true;
            bool hadError = false;
            Location finalCursorPosition = new Location(0, 0);
            Location promptFinalPos = new Location(0, 0);
            FormattedWriteSettings changedWriteOptions;

            // If inputs are not case-sensitive, make inputs all lowered
            if (!isCaseSensitive)
            {
                unacceptableInputs = unacceptableInputs.Select(x => x.ToLower());
            }

            // Fix char length if it is less than 0
            if (maxCharLength < 0)
            {
                maxCharLength = 0;
            }

            // Get prompt settings and set more error prompt settings
            if (writeOptions == null)
            {
                writeOptions = new FormattedWriteSettings() { NewLine = false };
            }
            if (errorWriteOptions == null)
            {
                changedWriteOptions = new FormattedWriteSettings(writeOptions);
            }
            else
            {
                changedWriteOptions = errorWriteOptions;
            }

            // Create initial prompt and set more error prompt settings
            Console.CursorTop = writeOptions.Location.Y;
            promptFinalPos = FormattedWrite(prompt, writeOptions);
            Location inputCursorPos = new Location();
            if (changedWriteOptions.Location.X == -1)
            {
                changedWriteOptions.Location.X = 0;
            }
            if (errorWriteOptions == null || changedWriteOptions.Location.Y == -1)
            {
                changedWriteOptions.Location.Y = promptFinalPos.Y + 2;
            }
            changedWriteOptions.ResetCursorPosition = true;

            // Gets the integer from the user
            while (invalidInput)
            {
                // Get the input
                Console.SetCursorPosition(inputCursorPos.X, inputCursorPos.Y);
                input = GetLimitedSizeInput(maxCharLength, maxCharHeight);

                // Check if input is acceptable
                if (isCaseSensitive)
                {
                    invalidInput = unacceptableInputs.Contains(input);
                }
                else
                {
                    invalidInput = unacceptableInputs.Contains(input.ToLower());
                }

                // Apply changes if invalid
                if (invalidInput)
                {
                    // Clear the input
                    Location secondLocation = new Location(0, Console.CursorTop);
                    if (inputCursorPos.X + maxCharLength < inputCursorPos.X)
                    {
                        secondLocation.X = int.MaxValue;
                    }
                    else
                    {
                        secondLocation.X = inputCursorPos.X + maxCharLength;
                    }
                    ClearArea(inputCursorPos, secondLocation);

                    // Clear the space taken by the previous error
                    if (hadError)
                    {
                        ClearArea(changedWriteOptions.Location, finalCursorPosition);
                    }

                    // Write the error
                    finalCursorPosition = FormattedWrite(errorPrompt, changedWriteOptions);
                    hadError = true;
                }
            }

            // Remove any errors that were created
            if (hadError)
            {
                ClearArea(changedWriteOptions.Location, finalCursorPosition);
            }

            // Reset cursor position
            Console.CursorLeft = writeOptions.Location.X;
            return input;
        }

        /// <summary>
        /// Gets a user input and prevents user input getting longer than a set number of digits on one line
        /// </summary>
        /// <param name="maxLength">The maximum length/number of characters</param>
        /// <returns>The user-inputted string</returns>
        public static string GetLimitedSizeInput(int maxLength)
        {
            return GetLimitedSizeInput(maxLength, 1);
        }

        /// <summary>
        /// Gets a user input and prevents user input getting larger than a certain area
        /// </summary>
        /// <param name="maxLength">The maximum length of the input</param>
        /// <param name="maxHeight">The maximum number of lines the input can occupy</param>
        /// <returns>The user-inputted string</returns>
        public static string GetLimitedSizeInput(int maxLength, int maxHeight)
        {
            List<char> inputChars = new List<char>();
            ConsoleKeyInfo keyInfo;
            bool done = false;
            Location originalCursorPosition = new Location();

            while (!done)
            {
                keyInfo = Console.ReadKey(true);

                switch (keyInfo.KeyChar)
                {
                    // What to do if backspace is pressed
                    case '\b':

                        // Remove the previous text
                        if (inputChars.Count > 0)
                        {
                            Console.CursorLeft--;
                            Console.Write(" ");
                            Console.CursorLeft--;
                            inputChars.RemoveAt(inputChars.Count - 1);
                            if (Console.CursorLeft == originalCursorPosition.X && Console.CursorTop != originalCursorPosition.Y)
                            {
                                Console.SetCursorPosition(Console.CursorLeft + maxLength, Console.CursorTop - 1);
                            }
                        }
                        break;

                    // What to do if return/enter is pressed
                    case '\r':
                        done = true;
                        break;

                    // What to do if any other key is pressed
                    default:

                        // Check text validity
                        if (inputChars.Count < (long)maxLength * maxHeight)
                        {
                            if (inputChars.Count % maxLength == 0 && inputChars.Count != 0)
                            {
                                Console.SetCursorPosition(originalCursorPosition.X, Console.CursorTop + 1);
                            }
                            Console.Write(keyInfo.KeyChar);
                            inputChars.Add(keyInfo.KeyChar);
                        }
                        break;
                }
            }

            // Set cursor position to starting X position plus one Y pos
            Console.SetCursorPosition(originalCursorPosition.X, Console.CursorTop + 1);
            return new string(inputChars.ToArray());
        }

        /// <summary>
        /// Gets user input and prevents user input getting larger than a certain area or entering incorrect strings
        /// </summary>
        /// <param name="allowedText">The list of allowed text options that the user can type</param>
        /// <returns>The user-inputted string</returns>
        public static string GetSpecificInput(IEnumerable<string> allowedText, bool isCaseSensitive = false)
        {
            return GetSpecificInput(allowedText, int.MaxValue, 1);
        }

        /// <summary>
        /// Gets user input and prevents user input getting larger than a certain area or entering incorrect strings
        /// </summary>
        /// <param name="allowedText">The list of allowed text options that the user can type</param>
        /// <param name="maxLength">The maximum length of the input</param>
        /// <returns>The user-inputted string</returns>
        public static string GetSpecificInput(IEnumerable<string> allowedText, int maxLength, bool isCaseSensitive = false)
        {
            return GetSpecificInput(allowedText, maxLength, 1);
        }

        /// <summary>
        /// Gets user input and prevents user input getting larger than a certain area or entering incorrect strings
        /// </summary>
        /// <param name="allowedText">The list of allowed text options that the user can type</param>
        /// <param name="maxLength">The maximum length of the input</param>
        /// <param name="maxHeight">The maximum number of lines the input can occupy</param>
        /// <param name="isCaseSensitive">Whether the checker is case sensitive</param>
        /// <returns>The user-inputted string</returns>
        public static string GetSpecificInput(IEnumerable<string> allowedText, int maxLength, int maxHeight, bool isCaseSensitive = false)
        {
            if (!isCaseSensitive)
            {
                allowedText = allowedText.Select(x => x.ToLower());
            }

            List<char> inputChars = new List<char>();
            ConsoleKeyInfo keyInfo;
            bool done = false;
            Location originalCursorPosition = new Location();

            while (!done)
            {
                keyInfo = Console.ReadKey(true);

                switch (keyInfo.KeyChar)
                {
                    // What to do if backspace is pressed
                    case '\b':

                        // Remove the previous text
                        if (inputChars.Count > 0)
                        {
                            Console.CursorLeft--;
                            Console.Write(" ");
                            Console.CursorLeft--;
                            inputChars.RemoveAt(inputChars.Count - 1);
                            if (Console.CursorLeft == originalCursorPosition.X && Console.CursorTop != originalCursorPosition.Y)
                            {
                                Console.SetCursorPosition(Console.CursorLeft + maxLength, Console.CursorTop - 1);
                            }
                        }
                        break;

                    // What to do if return/enter is pressed
                    case '\r':
                        if (allowedText.Contains(new string(inputChars.ToArray())))
                        {
                            done = true;
                        }
                        break;

                    // What to do if any other key is pressed
                    default:

                        // If not case-sensitive, lower the text
                        char keyPressed = keyInfo.KeyChar;
                        if (!isCaseSensitive)
                        {
                            keyPressed = keyPressed.ToString().ToLower()[0];
                        }

                        // Check text validity
                        if (inputChars.Count < (long)maxLength * maxHeight && LimitedTextAllowed(allowedText, inputChars.Concat(new char[] { keyPressed })))
                        {
                            if (inputChars.Count % maxLength == 0 && inputChars.Count != 0)
                            {
                                Console.SetCursorPosition(originalCursorPosition.X, Console.CursorTop + 1);
                            }
                            Console.Write(keyInfo.KeyChar);
                            inputChars.Add(keyPressed);
                        }
                        break;
                }
            }

            // Set cursor position to starting X position plus one Y pos
            Console.SetCursorPosition(originalCursorPosition.X, Console.CursorTop + 1);
            return new string(inputChars.ToArray());
        }

        /// <summary>
        /// Checks whether the inputted text is valid
        /// </summary>
        /// <param name="allowedText">The list of allowed options</param>
        /// <param name="inputChars">The current inputted text</param>
        /// <returns>Whether the text is a part of any of the allowed text</returns>
        private static bool LimitedTextAllowed(IEnumerable<string> allowedText, IEnumerable<char> inputChars)
        {
            string input = new string(inputChars.ToArray());

            // Check every bit of text in the allowed list
            foreach (string text in allowedText)
            {
                string gradualIncreasingText = "";

                // Check if input is in any interation of the text 
                foreach (char textCharacter in text)
                {
                    gradualIncreasingText += textCharacter;
                    if (input == gradualIncreasingText)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
