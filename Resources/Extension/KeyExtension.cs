using System.Windows.Input;

namespace ZC.TimeCalculator.Resources.Extension
{
    internal static class KeyExtension
    {
        public static bool IsDigit(this Key key)
        {
            for (int i = 0; i < 10; i++)
            {
                if (key.ToString().EndsWith(i.ToString())) return true;
            }
            return false;
        }
    }
}
