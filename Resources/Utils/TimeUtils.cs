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

        public static Regex timeFormat = new Regex("(-?)([_01][_0-9]|[_2][_0-3]):([_0-5][_0-9])");      // Regex for the default time format
        public static Regex timeFinishedFormat = new Regex("(-?)([01][0-9]|2[0-3]):([0-5][0-9])");      // Regex for the date format when entierly written

        /// <summary>
        /// On preview key down, only allow specified keys
        /// and write specfic character
        /// </summary>
        /// <param name="sender">The textbox that triggered the event</param>
        /// <param name="e">Args of the key event</param>
        public static void Field_PreviewKeyDown(TextBox textBox, KeyEventArgs e, List<TextBox> textBoxes, List<TextBox> negativeTextBoxes)
        {
            // Tag the event as handled
            e.Handled = true;

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
            int[] timeRemaining = new int[2];               // Remaining time in hours and minutes separated
            int[] timeMorning = new int[2];                 // Time done in the morning in hours and minutes separated
            int[] timeEnd = new int[2];                     // End time in hours and minutes separated

            // Delta of hours and minutes done in the morning
            timeMorning[0] = timeStartBreak[0] - timeStart[0];
            timeMorning[1] = timeStartBreak[1] - timeStart[1];
            // If delta of minutes is negative
            if (timeMorning[1] < 0)
            {
                // Tranform negative minutes in a negative hour and positive minutes
                timeMorning[0]--;
                timeMorning[1] += MINUTES_IN_HOUR;
            }

            // Delta of hours and minutes remaining
            timeRemaining[0] = timeRequired[0] - timeMorning[0];
            timeRemaining[1] = timeRequired[1] - timeMorning[1];
            // If delta of minutes is negative
            if (timeRemaining[1] < 0)
            {
                // Transform negative minutes in a negative hour and positive minutes
                timeRemaining[0]--;
                timeRemaining[1] += MINUTES_IN_HOUR;
            }

            // Add remaining time to the end break time
            timeEnd[0] = timeEndBreak[0] + timeRemaining[0];

            // If the hours calculated are more than a day
            if (timeEnd[0] > HOURS_IN_DAY)
                // Remove the 24 hours to make a correct time
                timeEnd[0] -= HOURS_IN_DAY;

            timeEnd[1] = timeEndBreak[1] + timeRemaining[1];
            // While end minutes are higher than an hour
            while (timeEnd[1] >= 60)
            {
                // Transform 60 minutes into an hour
                timeEnd[0]++;
                timeEnd[1] -= MINUTES_IN_HOUR;
            }

            // If end time is higher than 24 hours
            if (timeEnd[0] >= HOURS_IN_DAY) timeEnd[0] -= HOURS_IN_DAY;

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
            int[] timeEndSupp = new int[2];

            // If supp time is higher or equal to required time, display start time as supp end time
            if (timeSupp[0] > timeRequired[0] || (timeSupp[0] == timeRequired[0] && timeSupp[1] == timeRequired[1]))
                timeEndSupp = timeStart;
            else
            {
                // If supp hours are negatives, sets negative minutes else sets positive minutes
                timeSupp[1] = timeSupp[0] < 0 ? timeSupp[1] * -1 : timeSupp[1];

                // End time minus supp time
                timeEndSupp[0] = timeEnd[0] - timeSupp[0];
                timeEndSupp[1] = timeEnd[1] - timeSupp[1];
                // While end supp minutes are higher than an hour
                while (timeEndSupp[1] > MINUTES_IN_HOUR)
                {
                    // Transform the hour minutes in an hour
                    timeEndSupp[0]++;
                    timeEndSupp[1] -= MINUTES_IN_HOUR;
                }
                // While end supp minutes are negative
                while (timeEndSupp[1] < 0)
                {
                    // Transform the negative minutes in a negative hour
                    timeEndSupp[0]--;
                    timeEndSupp[1] += MINUTES_IN_HOUR;
                }

                // If end time hours are lower than 0, add 23 to it
                if (timeEndSupp[0] < 0) timeEndSupp[0] += HOURS_IN_DAY;
            }

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
            int[] timeSupp = new int[2];
            bool isNegativeBut0Hour = false;

            // Delta of hours and minutes
            timeSupp[0] = timeEndUser[0] - timeEndCalculated[0];
            timeSupp[1] = timeEndUser[1] - timeEndCalculated[1];

            // If delta of minutes is negative
            if (timeSupp[1] < 0)
            {
                // If delta of hours is different from 0
                if (timeSupp[0] != 0)
                {
                    // Transform negative supp hour minutes into supp hours
                    timeSupp[0]--;
                    timeSupp[1] += MINUTES_IN_HOUR;
                }
                // Else assign true to the 0 hours negative flag
                else isNegativeBut0Hour = true;

                // Multiply supp minutes by -1 to make the positive
                timeSupp[1] *= -1;
            }
            // If delta of minutes is over an hour
            else if (timeSupp[1] > MINUTES_IN_HOUR)
            {
                // Transform over hour supp minutes into supp hours
                timeSupp[0]++;
                timeSupp[1] -= MINUTES_IN_HOUR;
            }

            return (timeSupp, isNegativeBut0Hour);
        }
    }
}
