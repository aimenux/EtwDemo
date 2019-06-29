using EtwNetFull.Listeners;
using System;
using EtwNetFull.Providers;

namespace EtwNetFull
{
    internal class Program
    {
        private static void Main()
        {
            SampleTwo();

            Console.WriteLine("Press any key to exit ..");
            Console.ReadKey();
        }

        private static void SampleOne()
        {
            using (var kernelListener = new KernelListener())
            {
                kernelListener.Run();
            }
        }

        private static void SampleTwo()
        {
            for (var index = 1; index <= 10; index++)
            {
                CustomEventSource.Log.MessageSubmitted("Hello ETW", DateTime.UtcNow.AddDays(-index));
            }
        }
    }
}
