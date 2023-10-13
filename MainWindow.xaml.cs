using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZC.TimeCalculator.Resources.Utils;

namespace ZC.TimeCalculator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DEFAULT_TEXT = "__:__";                // Constant value of the default text

        int[] timeStart = new int[2];                       // Start time in hours and minutes separated
        int[] timeEnd = new int[2];                         // End time in hours and minutes separated
        int[] timeStartBreak = new int[2];                  // Start break time in hours and minutes separated
        int[] timeEndBreak = new int[2];                    // End break time in hours and minutes separated
        int[] timeSupp = new int[3];                        // Supp hours in hours and minutes separated
        int[] timeEndSupp = new int[2];                     // End time calculated with supp time
        int[] timeRequired = new int[2];                    // Work time required in hours and minutes separated

        bool componentInitialized = false;                  // Contains if the components are already initialized

        Regex timeFinishedFormat = new Regex("(-?)([01][0-9]|2[0-3]):([0-5][0-9])");        // Regex for the date format when entierly written

        List<TextBox> textBoxes = new List<TextBox>();              // List of all textboxes in the app
        List<TextBox> negativeTextBoxes = new List<TextBox>();      // List of all textboxes that supports negative values

        /// <summary>
        /// Property that returns the value of timeEnd or assign it a value and change time end related text field
        /// </summary>
        public int[] TimeEnd { 
            // Return the value of timeEnd
            get => timeEnd;
            set
            {
                // Assign the value to timeEnd
                timeEnd = value;

                // Display the result in 2 digits
                txtTimeEnd.Text = timeEnd[0].ToString("D2") + ":" + timeEnd[1].ToString("D2");
            }
        }

        /// <summary>
        /// On preview key down, only allow specified keys
        /// and write specfic character
        /// </summary>
        /// <param name="sender">The textbox that triggered the event</param>
        /// <param name="e">Args of the key event</param>
        private void Field_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            TimeUtils.Field_PreviewKeyDown(textBox, e, textBoxes, negativeTextBoxes);
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

                    // Start calculating end time
                    TimeEnd = TimeUtils.CalculateEnd(timeRequired, timeStart, timeStartBreak, timeEndBreak);
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
                    txtTimeEnd.Text = TimeEnd[0].ToString("D2") + ":" + TimeEnd[1].ToString("D2");

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

                // If textbox is read-only
                if (textBox.IsReadOnly) return;

                // If end time was calculated and both end fields content are set
                if (TimeEnd[0] > 0 && TimeEnd[1] > 0 && timeFinishedFormat.IsMatch(txtTimeEnd.Text))
                {
                    string[] endTime = txtTimeEnd.Text.Split(':');
                    int[] timeSuppUser = new int[2];
                    // Delta of hours and minutes
                    timeSuppUser[0] = int.Parse(endTime[0]);
                    timeSuppUser[1] = int.Parse(endTime[1]);

                    (int[] timeSupp, bool isNegativeBut0Hour) suppTime = TimeUtils.CalculateSuppTime(timeSuppUser, TimeEnd);

                    // Display the results in 2 digits
                    txtTimeSupp.Text = (suppTime.isNegativeBut0Hour ? "-" : "") + suppTime.timeSupp[0].ToString("D2") + ":" + suppTime.timeSupp[1].ToString("D2");
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

            negativeTextBoxes.Add(txtTimeSupp);
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
                    TimeEnd = TimeUtils.CalculateEnd(timeRequired, timeStart, timeStartBreak, timeEndBreak);
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
                    {
                        // Calculate supp end time hours
                        string[] fieldSupp = txtTimeSupp.Text.Split(':');

                        timeSupp[0] = int.Parse(fieldSupp[0]);
                        timeSupp[1] = int.Parse(fieldSupp[1]);
                        timeSupp[2] = txtTimeSupp.Text.StartsWith("-") ? 1 : 0;

                        // Start calculating supp end time
                        timeEndSupp = TimeUtils.CalculateEndSupp(timeRequired, timeStart, TimeEnd, timeSupp);

                        // Display the supp end time
                        txtTimeEndSupp.Text = timeEndSupp[0].ToString("D2") + ":" + timeEndSupp[1].ToString("D2");
                    }
                }
            }
        }
    }
}
