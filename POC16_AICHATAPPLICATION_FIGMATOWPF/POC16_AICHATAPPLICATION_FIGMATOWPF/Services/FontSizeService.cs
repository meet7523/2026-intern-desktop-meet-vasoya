using System.Windows;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public static class FontSizeService
    {
        public static void ApplyFontSize(AppFontSize fontSize)
        {
            double size = 16;

            switch (fontSize)
            {
                case AppFontSize.Small:
                    size = 14;
                    break;

                case AppFontSize.Medium:
                    size = 16;
                    break;

                case AppFontSize.Large:
                    size = 18;
                    break;
            }

            Application.Current.Resources["ChatFontSize"] = size;
        }
    }
}