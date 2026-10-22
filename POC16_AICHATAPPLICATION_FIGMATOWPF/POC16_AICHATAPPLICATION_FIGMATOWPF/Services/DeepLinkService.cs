using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public static class DeepLinkService
    {
        private const string ProtocolName = "openchatai";

        public static void RegisterProtocol()
        {
            try
            {
                string exePath =
                    Process.GetCurrentProcess()
                        .MainModule
                        .FileName;

                if (string.IsNullOrWhiteSpace(exePath))
                    return;

                using (RegistryKey protocolKey =
                       Registry.CurrentUser.CreateSubKey(
                           $@"Software\Classes\{ProtocolName}"))
                {
                    if (protocolKey == null)
                        return;

                    protocolKey.SetValue(
                        null,
                        "URL:Open ChatAI Protocol",
                        RegistryValueKind.String);

                    protocolKey.SetValue(
                        "URL Protocol",
                        "",
                        RegistryValueKind.String);

                    using (RegistryKey commandKey =
                           protocolKey.CreateSubKey(
                               @"shell\open\command"))
                    {
                        if (commandKey == null)
                            return;

                        commandKey.SetValue(
                            null,
                            $"\"{exePath}\" \"%1\"",
                            RegistryValueKind.String);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "Deep link registration failed: " +
                    ex);
            }
        }

        public static bool IsDeepLink(
            string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
                return false;

            return argument.StartsWith(
                ProtocolName + "://",
                StringComparison.OrdinalIgnoreCase);
        }

        public static string GetDeepLink(
            string[] args)
        {
            if (args == null || args.Length == 0)
                return null;

            return args.FirstOrDefault(
                IsDeepLink);
        }
    }
}