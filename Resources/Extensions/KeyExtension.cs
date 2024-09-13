using System.Windows.Input;

namespace ZC.TimeCalculator.Resources.Extension
{
    /// <summary>
    /// Key extension class
    /// </summary>
    internal static class KeyExtension
    {
        /// <summary>
        /// Returns true if the key is a digit key
        /// </summary>
        /// <param name="key">The key to verify</param>
        /// <returns>Returns if the key specified is a digit key</returns>
        public static bool IsDigit(this Key key)
        {
            // Goes by every digit from 0 to 9
            for (int i = 0; i < 10; i++)
            {
                // If the key ends with the digit, returns true
                if (key.ToString().EndsWith(i.ToString())) return true;
            }
            // If the function still hasn't return anything, return false
            return false;
        }
    }
}
