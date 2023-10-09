using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using ZC.TimeCalculator.Resources.Extension;

namespace ZC.TimeCalculator.Resources.Utils
{
    public class TimeUtils
    {
        public const int MINUTES_IN_HOUR = 60;                                                          // Constant value of minutes in an hour
        public const int HOURS_IN_DAY = 24;                                                             // Constant value of hours in a day
        public const int MINUTES_IN_DAY = MINUTES_IN_HOUR * HOURS_IN_DAY;

        public static Regex timeFormat = new Regex("(-?)([_01][_0-9]|[_2][_0-3]):([_0-5][_0-9])");      // Regex for the default time format
        public static Regex timeFinishedFormat = new Regex("(-?)([01][0-9]|2[0-3]):([0-5][0-9])");      // Regex for the date format when entierly written

        /// <summary>
        /// On preview key down, only allow specified keys
        /// and write specfic character
        /// </summary>
        /// <param name="textBox">The textbox that triggered the event</param>
        /// <param name="e">The event args</param>
        /// <param name="textBoxes">The list of all textboxes focusable</param>
        /// <param name="negativeTextBoxes">The list of all textboxes that support negatives values</param>
        public static void Field_PreviewKeyDown(TextBox textBox, KeyEventArgs e, List<TextBox> textBoxes, List<TextBox> negativeTextBoxes)
        {
            // Tag the event as handled
            e.Handled = true;

            // If the textbox is on read-only mode, abort the event
            if (textBox.IsReadOnly) return;

            // Defines the minimum limit of the caret index
            int limit = textBox.Text.StartsWith("-") ? 1 : 0;

            // If the caret index is under the limit, assign the limit value to the caret index
            if (textBox.CaretIndex < limit) textBox.CaretIndex = limit;

            int caretPosition = textBox.CaretIndex;

            string keyName = e.Key.ToString();
            string keyValue = "";
            // If the key is a digit
            if (e.Key.IsDigit())
            {
                // If the caret position is at the end of the text, return
                if (caretPosition == textBox.Text.Length) return;
                // Get the digit value at the end of the key name
                keyValue = keyName.Substring(keyName.Length - 1, 1);
                // If the caret position is on a colon, get to next position
                if (textBox.Text[caretPosition] == ':') caretPosition++;
            }
            // If the caret index is higher than the limit and the key is the backspace key
            else if (caretPosition > limit && e.Key == Key.Back)
            {
                keyValue = "_";
                // Decrement the caret position to be before the character to change
                caretPosition--;
                // If the caret position is on a colon, get to the previous position
                if (textBox.Text[caretPosition] == ':') caretPosition--;
            }
            // If the key is the delete key
            else if (e.Key == Key.Delete)
            {
                keyValue = "_";
                // If the caret position is on a colon, get to the next position
                if (textBox.Text[caretPosition] == ':') caretPosition++;
            }
            // If the key is the left key
            else if (e.Key == Key.Left)
            {
                // If the caret index is higher than the limit
                if (textBox.CaretIndex > limit)
                    // Move the caret index by 1 to the left
                    textBox.CaretIndex = caretPosition - 1;
                // Else get the focus on the previous field
                else textBox.FocusPreviousField(textBoxes);
            }
            // If the key is the right key
            else if (e.Key == Key.Right)
            {
                // If the caret index is not at the end of the textbox
                if (textBox.CaretIndex < textBox.Text.Length)
                    // Move the caret index by 1 to the right
                    textBox.CaretIndex = caretPosition + 1;
                // Else get the focus on the next field
                else textBox.FocusNextField(textBoxes);
            }
            // If the key is the tab key
            else if (e.Key == Key.Tab)
                // Get the focus on the next field
                textBox.FocusNextField(textBoxes);
            // If the key is one of the minus or add keys
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus || e.Key == Key.Add)
            {
                // Browse the list to verify if the textbox is part of negativeTextBoxes
                foreach (TextBox item in negativeTextBoxes)
                {
                    // If the textbox is the txtTimeSupp TextBox
                    if (item.Equals(textBox))
                    {
                        // If the key is one of the minus key and the textbox's text doesn't start with a minus
                        if (e.Key == Key.Subtract || e.Key == Key.OemMinus && !textBox.Text.StartsWith("-"))
                        {
                            // Add a minus to the textbox's text
                            textBox.Text = "-" + textBox.Text;
                            // Set the caret index next the minus character
                            textBox.CaretIndex = 1;
                        }
                        // If the key is the add key and the textbox's text starts with a minus, removes it
                        else if (e.Key == Key.Add && textBox.Text.StartsWith("-")) textBox.Text = textBox.Text.Substring(1);
                    }
                }
            }

            // If the key value has been set
            if (keyValue != "")
            {
                // Insert the key value at the current caret position in a new string
                string time = textBox.Text.Substring(0, caretPosition) + keyValue +
                    textBox.Text.Substring(caretPosition + 1, textBox.Text.Length - (caretPosition + 1));
                // If the new time matches the default time regex
                if (timeFormat.IsMatch(time))
                {
                    // Assign the new time to the textBox's text
                    textBox.Text = time;
                    // If the key is the backspace key
                    if (e.Key == Key.Back)
                    {
                        // If the caret position is higher than the limit and the char at this position is a colon, move backward the caret index by 1
                        if (caretPosition > limit && textBox.Text[caretPosition - 1] == ':') textBox.CaretIndex = caretPosition - 1;
                        // Else assign the current caret position to the caret index
                        else textBox.CaretIndex = caretPosition;
                    }
                    // If the key is the delete key, assign the current caret position to the caret index
                    else if (e.Key == Key.Delete) textBox.CaretIndex = caretPosition;
                    // If the next caret position is not the last position
                    else if (caretPosition + 1 < textBox.Text.Length)
                    {
                        // Move the caret index forward by 1
                        textBox.CaretIndex = caretPosition + 1;
                        // If the character at the caret position is a colon, move forward the caret index by 1
                        if (textBox.Text[textBox.CaretIndex] == ':') textBox.CaretIndex = textBox.CaretIndex + 1;
                    }
                    // Else get the focus on the next field
                    else textBox.FocusNextField(textBoxes);
                }
            }
        }

        /// <summary>
        /// Calculate the end time
        /// </summary>
        /// <param name="timeRequired">The time required to work</param>
        /// <param name="timeStart">The time when the user started work</param>
        /// <param name="timeStartBreak">The time when the user started his break</param>
        /// <param name="timeEndBreak">The time when the user re-started work after his break</param>
        /// <returns>Return a table of end time with the hours and minutes separated</returns>
        public static int[] CalculateEnd(int[] timeRequired, int[] timeStart, int[] timeStartBreak, int[] timeEndBreak)
        {
            int[] timeEnd = new int[2];                                                         // End time in hours and minutes separated

            int minutesRequired = timeRequired[0] * 60 + timeRequired[1];                       // Time required converted in minutes
            int minutesStart = timeStart[0] * 60 + timeStart[1];                                // Time start converted in minutes
            int minutesStartBreak = timeStartBreak[0] * 60 + timeStartBreak[1];                 // Time start break converted in minutes

            int minutesMinimumEnd = minutesRequired + minutesStart;                             // Minimum end time calculated in minutes

            // If minutesMinimumEnd is over 24 hours, removes the over a day minutes
            if (minutesMinimumEnd >= MINUTES_IN_DAY) minutesMinimumEnd -= MINUTES_IN_DAY;
            // Minimum end time is before the start break time
            if (minutesMinimumEnd < minutesStartBreak)
            {
                // Foreach 60 minutes, adds an hour and removes them
                while (minutesMinimumEnd >= 60)
                {
                    minutesMinimumEnd -= 60;
                    timeEnd[0]++;
                }
                // If end time hours are over 24, removes them
                if (timeEnd[0] >= 24) timeEnd[0] -= 24;
                // Assign remaining minutes to end time minutes
                timeEnd[1] = minutesMinimumEnd;

                // Returns the end time
                return timeEnd;
            }

            int minutesEndBreak = timeEndBreak[0] * 60 + timeEndBreak[1];                       // Time end break converted in minutes

            int minutesMorning = minutesStartBreak - minutesStart;                              // Time done in the morning calculated in minutes
            int minutesRemaining = minutesRequired - minutesMorning;                            // Remaining time calculated in minutes
            int minutesEnd = minutesEndBreak + minutesRemaining;                                // End time calculated in minutes

            // Foreach 60 minutes, adds an hour and remove them
            while (minutesEnd >= 60)
            {
                minutesEnd -= 60;
                timeEnd[0]++;
            }
            // While end time hours are over 24, removes them
            while (timeEnd[0] >= 24) timeEnd[0] -= 24;
            // Assign remaining minutes to end time minutes
            timeEnd[1] = minutesEnd;

            // Returns the end time
            return timeEnd;
        }

        /// <summary>
        /// Calculate the end time with the supp hours
        /// </summary>
        /// <param name="timeRequired">The time required to work</param>
        /// <param name="timeStart">The time when the user started to work</param>
        /// <param name="timeEnd">The end time calculated</param>
        /// <param name="timeSupp">The supp time available</param>
        /// <returns>Returns a table of time supp with the hours and mintues separated</returns>
        public static int[] CalculateEndSupp(int[] timeRequired, int[] timeStart, int[] timeEnd, int[] timeSupp)
        {
            int[] timeEndSupp = new int[2];                                                     // End time in hours and minutes separated

            // If supp time is higher or equal to required time, display start time as supp end time
            if (timeSupp[0] > timeRequired[0] || (timeSupp[0] == timeRequired[0] && timeSupp[1] == timeRequired[1]))
                timeEndSupp = timeStart;
            else
            {
                // If the negative flag of time supp is set, set the time supp minutes to negative
                timeSupp[1] = timeSupp[2] == 1 ? timeSupp[1] * -1 : timeSupp[1];
                int minutesSupp = timeSupp[0] * 60 + timeSupp[1];                               // Supp time converted in minutes
                int minutesEnd = timeEnd[0] * 60 + timeEnd[1];                                  // End time converted in minutes

                int minutesEndSupp = minutesEnd - minutesSupp;                                  // End supp time converted in minutes

                // Foreach 60 minutes, adds an hour and removes them
                while (minutesEndSupp >= 60)
                {
                    minutesEndSupp -= 60;
                    timeEndSupp[0]++;
                }
                // While end supp time hours are over 24, removes them
                while (timeEndSupp[0] >= 24) timeEndSupp[0] -= 24;
                // Assign remaining minutes to end supp time minutes
                timeEndSupp[1] = minutesEndSupp;
            }

            // Returns the end supp time
            return timeEndSupp;
        }

        /// <summary>
        /// Calculate the supp time with the end calculated and the end entered by the user
        /// </summary>
        /// <param name="timeEndUser">End time entered by the user</param>
        /// <param name="timeEndCalculated">End time calculated</param>
        /// <returns>Returns a tuple with the supp time and a flag if the hours are negative but equals to 0</returns>
        public static (int[] timeSupp, bool isNegativeBut0Hour) CalculateSuppTime(int[] timeEndUser, int[] timeEndCalculated)
        {
            int[] timeSupp = new int[2];                                                        // Supp time in hours and minutes separated
            bool isNegativeBut0Hour = false;                                                    // Flag if time supp is negative but is under 1 hour

            int minutesUser = timeEndUser[0] * 60 + timeEndUser[1];                             // End time of the user converted in minutes
            int minutesCalculated = timeEndCalculated[0] * 60 + timeEndCalculated[1];           // End time calculated converted in minutes

            // Calculate the delta of the end time the user entered and the end time calculated
            int minutesSuppTime = minutesUser - minutesCalculated;                              // Supp time calculated in minutes

            // If the delta time is positive
            if (minutesSuppTime >= 0)
            {
                // Foreach 60 minutes, add an hour and removes them
                while (minutesSuppTime >= 60)
                {
                    minutesSuppTime -= 60;
                    timeSupp[0]++;
                }
                // Assign the remaining minutes
                timeSupp[1] = minutesSuppTime; 
            }
            // Else -> Delta time is negative
            else
            {
                // If the delta time is between 60 and 0, the flag of negative hours but 0 is set to true
                if (minutesSuppTime > -60 && minutesSuppTime < 0) 
                    isNegativeBut0Hour = true;
                // While deltaTime is under -60
                while (minutesSuppTime <= -60)
                {
                    // Add 60 minutes and removes an hour
                    minutesSuppTime += 60;
                    timeSupp[0]--;
                }
                // Assign the remaining negatives minutes and make them absolute
                timeSupp[1] = Math.Abs(minutesSuppTime);
            }

            // Returns supp time and the flag
            return (timeSupp, isNegativeBut0Hour);
        }
    }
}
