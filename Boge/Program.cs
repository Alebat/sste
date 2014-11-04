using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WS_STE
{
    static class Program
    {
        public static IniFile _settings;
        public const string short_version = "4.2.4";

        [STAThread]
        static void Main()
        {

            bool exec = false;
            try
            {
                _settings = new IniFile(
#if DEBUG
                    "main.debug.ini"
#endif
                    );
            }
            catch (Exception)
            {
                MainForm.CriticalErrorMessage("INI file not found. Important data is stored there.\nThe program will be stopped.");
            }
            string cu = _settings.GetValue("Local", "culture");
            if (cu != null)
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cu);
            if (!exec)
            {
                Application.EnableVisualStyles();
                Application.Run(new MainForm());
            }
        }

        public static double ToUnixTimestamp(this DateTime value)
        {
            return (double)(value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds;
        }

        public static void Shuffle<T>(this IList<T> list, Comparison<T> nonConsecutive = null)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            if (nonConsecutive != null && list.Count > 2)
            {
                int i = -1;
                while ((i = list.Consec(nonConsecutive)) >= 0)
                {
                    int k = rng.Next(list.Count);
                    T value = list[k];
                    list[k] = list[i];
                    list[i] = value;
                }
            }
        }

        public static int Consec<T>(this IList<T> list, Comparison<T> equality)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (equality(list[i], list[i + 1]) == 0)
                    return i;
            }
            return -1;
        }
    }
}
