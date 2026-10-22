using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public sealed class SingleInstanceService : IDisposable
    {
        private const string MutexName =
            @"Global\POC16_AICHATAPPLICATION_FIGMATOWPF_SINGLE_INSTANCE";

        private const string PipeName =
            "POC16_AICHATAPPLICATION_FIGMATOWPF_DEEPLINK";

        private readonly Mutex _mutex;
        private readonly bool _ownsMutex;

        private bool _disposed;

        public SingleInstanceService()
        {
            _mutex = new Mutex(
                false,
                MutexName);

            try
            {
                _ownsMutex = _mutex.WaitOne(0);
            }
            catch (AbandonedMutexException)
            {
                _ownsMutex = true;
            }
        }

        public bool IsFirstInstance()
        {
            return _ownsMutex;
        }

        public bool SendDeepLinkToRunningInstance(
            string deepLink)
        {
            if (string.IsNullOrWhiteSpace(deepLink))
                return false;

            try
            {
                using (NamedPipeClientStream pipe =
                       new NamedPipeClientStream(
                           ".",
                           PipeName,
                           PipeDirection.Out))
                {
                    pipe.Connect(3000);

                    using (StreamWriter writer =
                           new StreamWriter(pipe))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine(deepLink);
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public void StartDeepLinkListener(
            Action<string> deepLinkReceived)
        {
            Task.Run(async () =>
            {
                while (!_disposed)
                {
                    try
                    {
                        using (NamedPipeServerStream pipe =
                               new NamedPipeServerStream(
                                   PipeName,
                                   PipeDirection.In,
                                   1,
                                   PipeTransmissionMode.Byte,
                                   PipeOptions.Asynchronous))
                        {
                            await pipe.WaitForConnectionAsync();

                            using (StreamReader reader =
                                   new StreamReader(pipe))
                            {
                                string deepLink =
                                    await reader.ReadLineAsync();

                                if (!string.IsNullOrWhiteSpace(
                                    deepLink))
                                {
                                    deepLinkReceived?.Invoke(
                                        deepLink);
                                }
                            }
                        }
                    }
                    catch
                    {
                        if (_disposed)
                            break;
                    }
                }
            });
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_ownsMutex)
            {
                try
                {
                    _mutex.ReleaseMutex();
                }
                catch
                {
                }
            }

            _mutex.Dispose();
        }
    }
}