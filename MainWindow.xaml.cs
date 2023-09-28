using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZC.TimeCalculator.Resources.Extension;

namespace ZC.TimeCalculator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int M_IN_H = 60;                              // Constant value of minutes in an hour
        const int H_IN_D = 24;                              // Constant value of hours in a day
        const string DEFAULT_TEXT = "__:__";                // Constant value of the default text

        int[] timeStart = new int[2];                       // Start time in hours and minutes separated
        int[] timeEnd = new int[2];                         // End time in hours and minutes separated
        int[] timeStartBreak = new int[2];                  // Start break time in hours and minutes separated
        int[] timeEndBreak = new int[2];                    // End break time in hours and minutes separated
        int[] timeSupp = new int[2];                        // Supp hours in hours and minutes separated
        int[] timeEndSupp = new int[2];                     // End time calculated with supp time
        int[] timeRequired = new int[2];                    // Work time required in hours and minutes separated

        bool componentInitialized = false;                  // Contains if the components are already initialized

        Regex timeFormat = new Regex("(-?)([_01][_0-9]|[_2][_0-3]):([_0-5][_0-9])");        // Regex for the default time format
        Regex timeFinishedFormat = new Regex("(-?)([01][0-9]|2[0-3]):([0-5][0-9])");        // Regex for the date format when entierly written

        List<TextBox> textBoxes = new List<TextBox>();      // List of all textboxes in the app

        /// <summary>
        /// On preview key down, only allow specified keys
        /// and write specfic character
        /// </summary>
        /// <param name="sender">The textbox that triggered the event</param>
        /// <param name="e">Args of the key event</param>
        private void Field_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

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
                else FocusPreviousField(textBox);
            }
            // If the key is the right key
            else if (e.Key == Key.Right)
            {
                // If the caret index is not at the end of the textbox
                if (textBox.CaretIndex < textBox.Text.Length)
                    // Move the caret index by 1 to the right
                    textBox.CaretIndex = caretPosition + 1;
                // Else get the focus on the next field
                else FocusNextField(textBox);
            }
            // If the key is the tab key
            else if (e.Key == Key.Tab)
                // Get the focus on the next field
                FocusNextField(textBox);
            // If the textbox is the txtTimeSupp TextBox
            else if (textBox.Equals(txtTimeSupp))
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
                    else FocusNextField(textBox);
                }
            }
        }

        /// <summary>
        /// Get the focus on the next field of the window
        /// </summary>
        /// <param name="currentTextBox">The current textBox</param>
        private void FocusNextField(TextBox currentTextBox)
        {
            // Get the event source textBox reference in the list 
            int idx = 0;
            foreach (TextBox item in textBoxes)
            {
                // If item is the event source textbox and the textbox is not the last one
                if (item == currentTextBox && idx < textBoxes.Count - 1)
                {
                    textBoxes[idx + 1].Focus();
                }
                ++idx;
            }
        }


        /// <summary>
        /// Get the focus on the previous field of the window
        /// </summary>
        /// <param name="currentTextBox">The current textBox</param>
        private void FocusPreviousField(TextBox currentTextBox)
        {
            // Get the event source textBox reference in the list 
            int idx = 0;
            foreach (TextBox item in textBoxes)
            {
                // If item is the event source textbox and the index is higher than 0
                if (item == currentTextBox && idx > 0)
                {
                    textBoxes[idx - 1].Focus();
                }
                ++idx;
            }
        }

        /// <summary>
        /// On text changes, calculate the new end time
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void FieldTimeRequired_TextChanged(object sender, TextChangedEventArgs e)
        {
            // If the text matches the finished time format
            if (timeFinishedFormat.IsMatch(txtTimeRequired.Text))
            {
                // Create a table of the hours and minutes splitted
                string[] fieldRequired = txtTimeRequired.Text.Split(':');

                // Assign to the time required int table the new time required string values
                timeRequired[0] = int.Parse(fieldRequired[0]);
                timeRequired[1] = int.Parse(fieldRequired[1]);

                // If the components are initialized and every textbox's text matches the finished time format
                if (componentInitialized && timeFinishedFormat.IsMatch(txtTimeStart.Text) &&
                    timeFinishedFormat.IsMatch(txtTimeStartPause.Text) &&
                    timeFinishedFormat.IsMatch(txtTimeEndPause.Text))
                    // Calculate the new end time
                    CalculateEnd();
            }
        }

        /// <summary>
        /// On double-click, toggle mode between supp end time and default end time calculation
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void FieldEnd_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // If end time was already calculated
            if (timeFinishedFormat.IsMatch(txtTimeEnd.Text))
            {
                // Toggle read-only mode on textBoxes
                txtTimeEnd.IsReadOnly = !txtTimeEnd.IsReadOnly;

                // If end fields are read-only
                if (txtTimeEnd.IsReadOnly)
                {
                    // Reset end time values to the calculated ones and h.supp to none
                    txtTimeEnd.Text = timeEnd[0].ToString("D2") + ":" + timeEnd[1].ToString("D2");

                    txtTimeSupp.Text = DEFAULT_TEXT;

                    txtTimeSupp.Focus();
                }
                // Else set focus on the end hour field
                else
                {
                    txtTimeEnd.Focus();
                }
            }
        }

        /// <summary>
        /// On text changes, calculates h.supp
        /// </summary>
        /// <param name="sender">The textbox that triggered the event</param>
        /// <param name="e">Not used</param>
        private void FieldsEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (componentInitialized)
            {
                TextBox textBox = sender as TextBox;

                bool isNegativeBut0Hour = false;

                // If textbox is read-only
                if (textBox.IsReadOnly) return;

                // If end time was calculated and both end fields content are set
                if (timeEnd[0] > 0 && timeEnd[1] > 0 && timeFinishedFormat.IsMatch(txtTimeEnd.Text))
                {
                    string[] endTime = txtTimeEnd.Text.Split(':');
                    // Delta of hours and minutes
                    timeSupp[0] = int.Parse(endTime[0]) - timeEnd[0];
                    timeSupp[1] = int.Parse(endTime[1]) - timeEnd[1];

                    // If delta of minutes is negative
                    if (timeSupp[1] < 0)
                    {
                        // If delta of hours is different from 0
                        if (timeSupp[0] != 0)
                        {
                            // Transform negative supp hour minutes into supp hours
                            timeSupp[0]--;
                            timeSupp[1] += M_IN_H;
                        }
                        // Else assign true to the 0 hours negative flag
                        else isNegativeBut0Hour = true;

                        // Multiply supp minutes by -1 to make the positive
                        timeSupp[1] *= -1;
                    }
                    // If delta of minutes is over an hour
                    else if (timeSupp[1] > M_IN_H)
                    {
                        // Transform over hour supp minutes into supp hours
                        timeSupp[0]++;
                        timeSupp[1] -= M_IN_H;
                    }

                    // Display the results in 2 digits
                    txtTimeSupp.Text = (isNegativeBut0Hour ? "-" : "") + timeSupp[0].ToString("D2") + ":" + timeSupp[1].ToString("D2");
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            componentInitialized = true;

            timeRequired[0] = 8;
            timeRequired[1] = 24;

            // Start the app with a focus on the start field
            txtTimeStart.Focus();

            // Get all the references of textboxes of the app into a list
            foreach (var item in mainGrid.Children)
                // If the item type is of the TextBox type
                if (item.GetType() == txtTimeStart.GetType())
                    textBoxes.Add(item as TextBox);
        }

        /// <summary>
        /// On text changes, calculates end time
        /// </summary>
        /// <param name="sender">The textbox that triggered the event</param>
        /// <param name="e">Not used</param>
        private void FieldsWorkTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            // If the component are initialized
            if (componentInitialized)
            {
                TextBox textBox = sender as TextBox;

                // If all the field required to calculate end time are sets
                if (timeFinishedFormat.IsMatch(txtTimeStart.Text) && timeFinishedFormat.IsMatch(txtTimeStartPause.Text) && timeFinishedFormat.IsMatch(txtTimeEndPause.Text))
                {
                    // Split fields value by double dots and insert them into a table
                    string[] fieldStart = txtTimeStart.Text.Split(':');
                    string[] fieldStartBreak = txtTimeStartPause.Text.Split(':');
                    string[] fieldEndBreak = txtTimeEndPause.Text.Split(':');

                    // Insert string time values into a table of int values
                    timeStart[0] = int.Parse(fieldStart[0]);
                    timeStart[1] = int.Parse(fieldStart[1]);
                    timeStartBreak[0] = int.Parse(fieldStartBreak[0]);
                    timeStartBreak[1] = int.Parse(fieldStartBreak[1]);
                    timeEndBreak[0] = int.Parse(fieldEndBreak[0]);
                    timeEndBreak[1] = int.Parse(fieldEndBreak[1]);

                    // Start calculating end time
                    CalculateEnd();
                }
            }
        }

        /// <summary>
        /// On text changes, calculates supp end time
        /// </summary>
        /// <param name="sender">The textbox that triggered the event</param>
        /// <param name="e">Not used</param>
        private void FieldsSupp_TextChanged(object sender, TextChangedEventArgs e)
        {
            // If the component are initialized
            if (componentInitialized)
            {
                // If end fields are read-only
                if (txtTimeEnd.IsReadOnly)
                {
                    // If all the values required to calculate supp end time are sets
                    if (timeFinishedFormat.IsMatch(txtTimeEnd.Text) && timeFinishedFormat.IsMatch(txtTimeSupp.Text))
                        // Start calculating supp end time
                        CalculateEndSupp();
                }
            }
        }

        /// <summary>
        /// Calculate the default end time
        /// </summary>
        private void CalculateEnd()
        {
            int[] timeRemaining = new int[2];               // Remaining time in hours and minutes separated
            int[] timeMorning = new int[2];                 // Time done in the morning in hours and minutes separated

            // Delta of hours and minutes done in the morning
            timeMorning[0] = timeStartBreak[0] - timeStart[0];
            timeMorning[1] = timeStartBreak[1] - timeStart[1];
            // If delta of minutes is negative
            if (timeMorning[1] < 0)
            {
                // Tranform negative minutes in a negative hour and positive minutes
                timeMorning[0]--;
                timeMorning[1] += M_IN_H;
            }

            // Delta of hours and minutes remaining
            timeRemaining[0] = timeRequired[0] - timeMorning[0];
            timeRemaining[1] = timeRequired[1] - timeMorning[1];
            // If delta of minutes is negative
            if (timeRemaining[1] < 0)
            {
                // Transform negative minutes in a negative hour and positive minutes
                timeRemaining[0]--;
                timeRemaining[1] += M_IN_H;
            }

            // Add remaining time to the end break time
            timeEnd[0] = timeEndBreak[0] + timeRemaining[0];
            timeEnd[1] = timeEndBreak[1] + timeRemaining[1];
            // While end minutes are higher than an hour
            while (timeEnd[1] >= 60)
            {
                // Transform 60 minutes into an hour
                timeEnd[0]++;
                timeEnd[1] -= M_IN_H;
            }

            // If end time is higher than 24 hours
            if (timeEnd[0] >= H_IN_D) timeEnd[0] -= H_IN_D;

            // Display the result in 2 digits
            txtTimeEnd.Text = timeEnd[0].ToString("D2") + ":" + timeEnd[1].ToString("D2");
        }

        /// <summary>
        /// Calculate the end time with supp hours
        /// </summary>
        private void CalculateEndSupp()
        {
            // Calculate supp end time hours
            string[] fieldSupp = txtTimeSupp.Text.Split(':');

            timeSupp[0] = int.Parse(fieldSupp[0]);
            timeSupp[1] = int.Parse(fieldSupp[1]);

            // If supp time is higher or equal to required time, display start time as supp end time
            if (timeSupp[0] > timeRequired[0] || (timeSupp[0] == timeRequired[0] && timeSupp[1] == timeRequired[0]))
                txtTimeEndSupp.Text = txtTimeStart.Text;
            else
            {
                // If supp hours are negatives, sets negative minutes else sets positive minutes
                timeSupp[1] = int.Parse(fieldSupp[0]) < 0 ? int.Parse(fieldSupp[1]) * -1 : int.Parse(fieldSupp[1]);

                // End time minus supp time
                timeEndSupp[0] = timeEnd[0] - timeSupp[0];
                timeEndSupp[1] = timeEnd[1] - timeSupp[1];
                // While end supp minutes are higher than an hour
                while (timeEndSupp[1] > M_IN_H)
                {
                    // Transform the hour minutes in an hour
                    timeEndSupp[0]++;
                    timeEndSupp[1] -= M_IN_H;
                }
                // While end supp minutes are negative
                while (timeEndSupp[1] < 0)
                {
                    // Transform the negative minutes in a negative hour
                    timeEndSupp[0]--;
                    timeEndSupp[1] += M_IN_H;
                }

                // If end time hours are lower than 0, add 23 to it
                if (timeEndSupp[0] < 0) timeEndSupp[0] += H_IN_D;

                // Display the supp end time
                txtTimeEndSupp.Text = timeEndSupp[0].ToString("D2") + ":" + timeEndSupp[1].ToString("D2");
            }
        }
    }
}
