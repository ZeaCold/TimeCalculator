using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZC.TimeCalculator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int hStart, mStart;
        int hEnd, mEnd;
        int hStartPause, mStartPause;
        int hEndPause, mEndPause;
        int hSupp, mSupp;

        List<TextBox> textBoxes = new List<TextBox>();

        private void txtEnd_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (hEnd > 0 && mEnd > 0)
            {
                txtHEnd.IsReadOnly = !txtHEnd.IsReadOnly;
                txtMEnd.IsReadOnly = !txtMEnd.IsReadOnly;

                if (txtHEnd.IsReadOnly)
                {
                    txtHEnd.Text = hEnd.ToString("D2");
                    txtMEnd.Text = mEnd.ToString("D2");

                    txtHTimeSupp.Text = "";
                    txtMTimeSupp.Text = "";

                    txtHTimeSupp.Focus();
                }
                else
                {
                    txtHEnd.Focus();
                    if (txtHEnd.Text.Length > 0) txtHEnd.SelectAll();
                }
            }
        }

        private void txtEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.IsReadOnly) return;

            if (textBox.Name.Contains("H"))
            {
                if (textBox.Text.Length == 2)
                {
                    txtMEnd.Focus();
                    if (txtMEnd.Text.Length > 0) txtMEnd.SelectAll();
                }
                else if (textBox.Text.Length > 2) textBox.Text = textBox.Text.Substring(0, 2);
            }
            if (hEnd > 0 && mEnd > 0 && txtHEnd.Text.Length == 2 && txtMEnd.Text.Length == 2)
            {
                hSupp = int.Parse(txtHEnd.Text) - hEnd;
                mSupp = int.Parse(txtMEnd.Text) - mEnd;
                if (mSupp < 0)
                {
                    hSupp--;
                    mSupp = 60 + mSupp;
                }
                if (mSupp > 60)
                {
                    hSupp++;
                    mSupp -= 60;
                }

                txtHTimeSupp.Text = hSupp.ToString("D2");
                txtMTimeSupp.Text = mSupp.ToString("D2");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            txtHStart.Focus();
            foreach (var item in mainGrid.Children)
            {
                if (item.GetType() == txtHStart.GetType())
                {
                    textBoxes.Add(item as TextBox);
                }
            }
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text.Length == 2)
            {
                if (int.Parse(textBox.Text) >= 0)
                {
                    if (int.Parse(textBox.Text) < 24 && textBox.Name.Contains("H") || int.Parse(textBox.Text) < 60 && textBox.Name.Contains("M"))
                    {
                        int idx = 0;
                        foreach (TextBox item in textBoxes)
                        {
                            ++idx;
                            if (item == textBox)
                            {
                                textBoxes[idx].Focus();
                                if (textBoxes[idx].Text.Length > 0)
                                {
                                    textBoxes[idx].SelectAll();
                                }
                            }
                        }
                    }
                    else
                    {
                        textBox.Text = "";
                    }
                }
                else
                {
                    textBox.Text = "";
                }
            }
            else if (textBox.Text.Length > 2)
            {
                textBox.Text = textBox.Text.Substring(0, 2);
            }

            if (txtHStart.Text.Length == 2 && txtHStartPause.Text.Length == 2 && txtHEndPause.Text.Length == 2 &&
                txtMStart.Text.Length == 2 && txtMStartPause.Text.Length == 2 && txtMEndPause.Text.Length == 2)
            {
                hStart = int.Parse(txtHStart.Text);
                mStart = int.Parse(txtMStart.Text);
                hStartPause = int.Parse(txtHStartPause.Text);
                mStartPause = int.Parse(txtMStartPause.Text);
                hEndPause = int.Parse(txtHEndPause.Text);
                mEndPause = int.Parse(txtMEndPause.Text);

                calculateEndPause();
            }
        }

        private void txt_TextSuppChanged(object sender, TextChangedEventArgs e)
        {
            if (txtHEnd.IsReadOnly)
            {
                if ((txtHTimeSupp.Text.Length == 2 && !txtHTimeSupp.Text.StartsWith("-")) || txtHTimeSupp.Text.Length == 3 && txtHTimeSupp.Text.StartsWith("-"))
                {
                    txtMTimeSupp.Focus();
                }
                if (txtHEnd.Text.Length == 2 &&
                    ((txtHTimeSupp.Text.Length == 2 && !txtHTimeSupp.Text.StartsWith("-")) ||
                    (txtHTimeSupp.Text.Length == 3 && txtHTimeSupp.Text.StartsWith("-"))) &&
                    txtMTimeSupp.Text.Length == 2)
                {
                    calculateHSupp();
                }
            }
        }

        private void calculateEndPause()
        {
            int hRemaining, mRemaining;
            int hMatin, mMatin;

            hMatin = hStartPause - hStart;
            if (mStartPause < mStart)
            {
                --hMatin;
                mMatin = 60 - mStart + mStartPause;
            }
            else
            {
                mMatin = mStartPause - mStart;
            }

            hRemaining = 8 - hMatin;
            if (mMatin < 24)
            {
                --hRemaining;
                mRemaining = 60 - mMatin + 24;
            }
            else
            {
                mRemaining = 24 - mMatin;
            }

            hEnd = hEndPause + hRemaining;
            if (mRemaining + mEndPause > 60)
            {
                ++hEnd;
                mEnd = mRemaining + mEndPause - 60;
            }
            else
            {
                mEnd = mRemaining + mEndPause;
            }

            txtHEnd.Text = hEnd.ToString("D2");
            txtMEnd.Text = mEnd.ToString("D2");
        }

        private void calculateHSupp()
        {
            int hEndv2 = int.Parse(txtHEnd.Text) - int.Parse(txtHTimeSupp.Text);

            if (hEndv2 >= 1)
            {
                int minTimeSupp = int.Parse(txtHTimeSupp.Text) < 0 ? int.Parse(txtMTimeSupp.Text) * -1 : int.Parse(txtMTimeSupp.Text);
                int mEndv2 = int.Parse(txtMEnd.Text) - minTimeSupp;
                if (mEndv2 < 0)
                {
                    --hEndv2;
                    mEndv2 = 60 + mEndv2;
                }
                else if (mEndv2 >= 60)
                {
                    ++hEndv2;
                    mEndv2 -= 60;
                }
                txtHEndv2.Text = hEndv2.ToString("D2");
                txtMEndv2.Text = mEndv2.ToString("D2");
            }
            else
            {
                txtHEndv2.Text = txtHStart.Text;
                txtMEndv2.Text = txtMStart.Text;
            }
        }
    }
}
