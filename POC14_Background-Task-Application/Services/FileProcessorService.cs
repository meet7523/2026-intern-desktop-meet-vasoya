using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace POC14_Background_Task_Application.Services
{
    public class FileProcessorServices
    {
        public class FileProcessorService
        {
            public object Logs { get; private set; }

            public async Task ProcessFileAsync(IProgress<int> progress, IProgress<string> operation,CancellationToken token)
            {
                for (int i = 1; i <= 50; i++)
                {
                    token.ThrowIfCancellationRequested();

                    await Task.Delay(500, token);

                    progress.Report(i * 2);

                    operation.Report($"[{DateTime.Now:HH:mm:ss}] Processing Count {i} of 50");
                }
            }
        }
    }
}

