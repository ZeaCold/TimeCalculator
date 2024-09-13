using System.Collections.Generic;
using System.Windows.Controls;

namespace ZC.TimeCalculator.Resources.Extension
{
    /// <summary>
    /// TextBox extension class
    /// </summary>
    public static class TextBoxExtension
    {
        /// <summary>
        /// Get the focus on the next field of the window
        /// </summary>
        /// <param name="textBox">The current textBox</param>
        /// <param name="textBoxes">List of all textBoxes available to focus</param>
        public static void FocusNextField(this TextBox textBox, List<TextBox> textBoxes)
        {
            int idx = 0;
            // Goes by every TextBox in the list
            foreach (TextBox item in textBoxes)
            {
                // If item is the event source textbox and the textbox is not the last one
                if (item.Equals(textBox))
                {
                    TextBox nextTextBox;                    // Reference to the next textBox in the list
                    // If the index is lower than the last textBox in the list, assign the next textbox
                    if (idx < textBoxes.Count - 1)
                        nextTextBox = textBoxes[idx + 1];
                    // Else assign the first one
                    else nextTextBox = textBoxes[0];

                    // Get the focus on the textBox and set its caret index to 0
                    nextTextBox.Focus();
                    nextTextBox.CaretIndex = 0;
                }
                idx++;
            }
        }


        /// <summary>
        /// Get the focus on the previous field of the window
        /// </summary>
        /// <param name="textBox">The current textBox</param>
        /// <param name="textBoxes">List of all textBoxes available to focus</param>
        public static void FocusPreviousField(this TextBox textBox, List<TextBox> textBoxes)
        {
            int idx = 0;
            // Goes by every TextBox in the list
            foreach (TextBox item in textBoxes)
            {
                // If item is the event source textbox and the index is higher than 0
                if (item.Equals(textBox))
                {
                    TextBox previousTextBox;                // Reference to the previous textBox in the list
                    // If the index is higher than 0, assign the previous text box
                    if (idx > 0)
                        previousTextBox = textBoxes[idx - 1];
                    // Else assign the last one
                    else previousTextBox = textBoxes[textBoxes.Count - 1];

                    // Get the focus on the textBox and set its caret index to 0
                    previousTextBox.Focus();
                    previousTextBox.CaretIndex = previousTextBox.Text.Length;

                }
                idx++;
            }
        }
    }
}
