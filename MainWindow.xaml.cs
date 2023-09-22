using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZC.TimeCalculator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int M_In_H = 60;                  // Constant value of minutes in an hour
        const int H_In_D = 24;                  // Constant value of minutes in a day

        int hStart, mStart;                     // Start time in hours and minutes separated
        int hEnd, mEnd;                         // End time in hours and minutes separated
        int hStartBreak, mStartBreak;           // Start break time in hours and minutes separated
        int hEndBreak, mEndBreak;               // End break time in hours and minutes separated
        int hSupp, mSupp;                       // Supp hours in hours and minutes separated
        int hRequired, mRequired;               // Work time required in hours and minutes separated

        List<TextBox> textBoxes = new List<TextBox>();

        /// <summary>
        /// On double-click, toggle mode between supp end time and default end time calculation
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void FieldsEnd_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // If end time was already calculated
            if (hEnd > 0 && mEnd > 0)
            {
                // Toggle read-only mode on textBoxes
                txtHEnd.IsReadOnly = !txtHEnd.IsReadOnly;
                txtMEnd.IsReadOnly = !txtMEnd.IsReadOnly;

                // If end fields are read-only
                if (txtHEnd.IsReadOnly)
                {
                    // Reset end time values to the calculated ones and h.supp to none
                    txtHEnd.Text = hEnd.ToString("D2");
                    txtMEnd.Text = mEnd.ToString("D2");

                    txtHSupp.Text = "";
                    txtMSupp.Text = "";

                    txtHSupp.Focus();
                }
                // Else set focus on the end hour field
                else
                {
                    txtHEnd.Focus();
                    if (txtHEnd.Text.Length > 0) txtHEnd.SelectAll();
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
            TextBox textBox = sender as TextBox;

            // If textbox is read-only
            if (textBox.IsReadOnly) return;

            // If the source textbox is the hour field
            if (textBox.Name.Contains("H"))
            {
                // If the textbox's text has a length of 2
                if (textBox.Text.Length == 2)
                {
                    txtMEnd.Focus();
                    if (txtMEnd.Text.Length > 0) txtMEnd.SelectAll();
                }
                // If length is more than 2
                else if (textBox.Text.Length > 2) textBox.Text = textBox.Text.Substring(0, 2);
            }
            // If end time was calculated and both end fields content are set
            if (hEnd > 0 && mEnd > 0 && txtHEnd.Text.Length == 2 && txtMEnd.Text.Length == 2)
            {
                // Delta of hours and minutes
                hSupp = int.Parse(txtHEnd.Text) - hEnd;
                mSupp = int.Parse(txtMEnd.Text) - mEnd;

                // If delta of minutes is negative
                if (mSupp < 0)
                {
                    // Transform negative supp minutes into supp hours
                    hSupp--;
                    mSupp = M_In_H + mSupp;
                }
                // If delta of minutes is over an hour
                else if (mSupp > M_In_H)
                {
                    // Transform over an hour supp minutes into supp hours
                    hSupp++;
                    mSupp -= M_In_H;
                }

                // Display the results in 2 digits
                txtHSupp.Text = hSupp.ToString("D2");
                txtMSupp.Text = mSupp.ToString("D2");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            hRequired = 8;
            mRequired = 24;

            // Start the app with a focus on the start field
            txtHStart.Focus();
            
            // Get all the references of textboxes of the app into a list
            foreach (var item in mainGrid.Children)
                if (item.GetType() == txtHStart.GetType())
                    textBoxes.Add(item as TextBox);
        }

        /// <summary>
        /// On text changes, calculates end time
        /// </summary>
        /// <param name="sender">The textbox that triggered the event</param>
        /// <param name="e">Not used</param>
        private void FieldsWorkTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // If textbox's text length equals 2
            if (textBox.Text.Length == 2)
            {
                // If textbox's text is higher or equal than 0
                if (int.Parse(textBox.Text) >= 0)
                {
                    // If textbox's name start with H and its text is lower than 24 or its name starts with M and its text is lower than 60
                    if (int.Parse(textBox.Text) < H_In_D && textBox.Name.Contains("H") || int.Parse(textBox.Text) < M_In_H && textBox.Name.Contains("M"))
                    {
                        // Get the event source textBox reference in the list 
                        int idx = 0;
                        foreach (TextBox item in textBoxes)
                        {
                            ++idx;
                            // If item is the event source textbox
                            if (item == textBox)
                            {
                                textBoxes[idx].Focus();
                                if (textBoxes[idx].Text.Length > 0)
                                    textBoxes[idx].SelectAll();
                            }
                        }
                    }
                    // Else reset its text
                    else textBox.Text = "";
                }
                // Else reset its text
                else textBox.Text = "";
            }
            // Else if textbox's text length is over 2
            else if (textBox.Text.Length > 2) textBox.Text = textBox.Text.Substring(0, 2);

            // If all the field required to calculate end time are sets
            if (txtHStart.Text.Length == 2 && txtHStartPause.Text.Length == 2 && txtHEndPause.Text.Length == 2 &&
                txtMStart.Text.Length == 2 && txtMStartPause.Text.Length == 2 && txtMEndPause.Text.Length == 2)
            {
                // Retrieve all fields value into variables
                hStart = int.Parse(txtHStart.Text);
                mStart = int.Parse(txtMStart.Text);
                hStartBreak = int.Parse(txtHStartPause.Text);
                mStartBreak = int.Parse(txtMStartPause.Text);
                hEndBreak = int.Parse(txtHEndPause.Text);
                mEndBreak = int.Parse(txtMEndPause.Text);

                // Start calculating end time
                CalculateEnd();
            }
        }

        /// <summary>
        /// On text changes, calculates supp end time
        /// </summary>
        /// <param name="sender">The textbox that triggered the event</param>
        /// <param name="e">Not used</param>
        private void FieldsSupp_TextChanged(object sender, TextChangedEventArgs e)
        {
            // If end fields are read-only
            if (txtHEnd.IsReadOnly)
            {
                // If supp hours length is 2 and not negative or length is 3 and negative
                if ((txtHSupp.Text.Length == 2 && !txtHSupp.Text.StartsWith("-")) || txtHSupp.Text.Length == 3 && txtHSupp.Text.StartsWith("-"))
                {
                    txtMSupp.Focus();
                }
                // If all the values required to calculate supp end time are sets
                if (txtHEnd.Text.Length == 2 &&
                    ((txtHSupp.Text.Length == 2 && !txtHSupp.Text.StartsWith("-")) ||
                    (txtHSupp.Text.Length == 3 && txtHSupp.Text.StartsWith("-"))) &&
                    txtMSupp.Text.Length == 2)
                    // Start calculating supp end time
                    CalculateEndSupp();
            }
        }

        /// <summary>
        /// Calculate the default end time
        /// </summary>
        private void CalculateEnd()
        {
            int hRemaining, mRemaining;             // Remaining time in hours and minutes separated
            int hMorning, mMorning;                 // hours done in the morning

            // Calculate the hours done in the morning
            hMorning = hStartBreak - hStart;
            // If start time minutes are higher than start break time minutes
            if (mStartBreak < mStart)
            {
                // Remove an hour and adapt minutes in consequence
                --hMorning;
                mMorning = M_In_H - mStart + mStartBreak;
            }
            // Else basic calculation
            else mMorning = mStartBreak - mStart;

            // Hours remaining to finish the day
            hRemaining = hRequired - hMorning;
            // If minutes dones are lower than required
            if (mMorning < mRequired)
            {
                // Remove an hour and adapt minutes in consequence
                --hRemaining;
                mRemaining = M_In_H - mMorning + mRequired;
            }
            // Else basic calculation
            else mRemaining = mRequired - mMorning;

            // Calculate the end hour
            hEnd = hEndBreak + hRemaining;
            // If sum of minutes remaining and minutes of the end break time are higher than 60
            if (mRemaining + mEndBreak > M_In_H)
            {
                // Add an hor and adapt minutes in consequence
                ++hEnd;
                mEnd = mRemaining + mEndBreak - M_In_H;
            }
            // Else basic calculation
            else mEnd = mRemaining + mEndBreak;

            // Display the result in 2 digits
            txtHEnd.Text = hEnd.ToString("D2");
            txtMEnd.Text = mEnd.ToString("D2");
        }

        /// <summary>
        /// Calculate the end time with supp hours
        /// </summary>
        private void CalculateEndSupp()
        {
            // Calculate supp end time hours
            hSupp = int.Parse(txtHEnd.Text) - int.Parse(txtHSupp.Text);

            // If supp end time hour is higher or equal to 1
            if (hSupp >= 1)
            {
                // If supp hours are negatives, sets negative minutes else sets positive minutes
                mSupp = int.Parse(txtHSupp.Text) < 0 ? int.Parse(txtMSupp.Text) * -1 : int.Parse(txtMSupp.Text);
                // Calculates supp end time minutes
                mSupp = int.Parse(txtMEnd.Text) - mSupp;

                // If supp end time minutes are lower than 0
                if (mSupp < 0)
                {
                    // Remove an hour and adapt minutes in consequence
                    --hSupp;
                    mSupp = M_In_H + mSupp;
                }
                // Else if supp end time minutes are higher than an hour
                else if (mSupp >= M_In_H)
                {
                    // Add an hour and adapt minutes in consequences
                    ++hSupp;
                    mSupp -= M_In_H;
                }

                // Display results in 2 digits
                txtHEndSupp.Text = hSupp.ToString("D2");
                txtMEndSupp.Text = mSupp.ToString("D2");
            }
            // Else display start time
            else
            {
                txtHEndSupp.Text = txtHStart.Text;
                txtMEndSupp.Text = txtMStart.Text;
            }
        }
    }
}
