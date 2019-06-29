using System;
using System.Threading;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace EtwNetFull.Listeners
{
    public class KernelListener : IListener, IDisposable
    {
        private Timer _timer;
        private const int TimeoutSec = 20;
        private readonly TraceEventSession _session;

        public KernelListener()
        {
            _session = new TraceEventSession(KernelTraceEventParser.KernelSessionName);
        }

        public void Run()
        {
            var isElevated = TraceEventSession.IsElevated() ?? false;
            if (!isElevated)
            {
                Console.WriteLine("ETW tracing of kernel providers requires admin rights");
                Console.WriteLine("Please restart and run as administrator");
                return;
            }

            Console.WriteLine("Start processing");

            Console.CancelKeyPress += (sender, args) => Dispose();

            _session.EnableKernelProvider(KernelTraceEventParser.Keywords.All);

            _session.Source.Kernel.ProcessStart += data =>
            {
                Console.WriteLine($"Starting process {GetProcessName(data)}");
            };

            _session.Source.Kernel.ProcessStop += data =>
            {
                Console.WriteLine($"Stopping process {GetProcessName(data)}");
            };

            _timer = InitializeTimer();

            _session.Source.Process();
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing resources");
            _session?.Dispose();
            _timer?.Dispose();
        }

        private void StopProcessing()
        {
            if (_session == null)
            {
                return;
            }

            Console.WriteLine("Stop processing");
            _session.Source.StopProcessing();
        }

        private Timer InitializeTimer()
        {
            const int milliseconds = TimeoutSec * 1000;
            return new Timer(state =>
            {
                Console.WriteLine("Stopped after {0} sec", TimeoutSec);
                StopProcessing();
            }, null, milliseconds, Timeout.Infinite);
        }

        private static string GetProcessName(TraceEvent data)
        {
            return string.IsNullOrWhiteSpace(data.ProcessName) ? data.ProcessID.ToString() : $"{data.ProcessName} {data.ProcessID}";
        }
    }
}
