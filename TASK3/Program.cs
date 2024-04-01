using System.Diagnostics.Metrics;
using System.Threading;

namespace TASK3
{
    internal class Program
    {
        private static Mutex _mutex = new Mutex(true, "ThreadingAplication");
        private static ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
        private static AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            if(!_mutex.WaitOne(TimeSpan.Zero, true))
            {
                Console.WriteLine("Another instance of the ThreadingAplication is already running on this machine!");
                return;
            }


            Thread t1 = new Thread(() =>
            {
                Console.WriteLine("Thread 1 started");
                Thread.Sleep(1000);
                _manualResetEvent.Set();
                Console.WriteLine("Thread 1 set signal");
                Thread.Sleep(5000);
                _manualResetEvent.Reset();
                Console.WriteLine("Thread 1 reset signal");
            });

            Thread t2 = new Thread(() =>
            {
                Console.WriteLine("Thread 2 started");
                Thread.Sleep(1000);
                Console.WriteLine("Thread 2 set signal");
                _autoResetEvent.Set();
                Thread.Sleep(5000);
                Console.WriteLine("Thread 2 reset signal");
                _autoResetEvent.Reset();
                Thread.Sleep(1000);
                Console.WriteLine("Thread 2 set signal");
                _autoResetEvent.Set();
                Thread.Sleep(5000);
                Console.WriteLine("Thread 2 reset signal");
                _autoResetEvent.Reset();
            });

            Thread t3 = new Thread(() =>
            {
                Console.WriteLine("Thread 3 is waiting for a manual singal from Thread 1");
                _manualResetEvent.WaitOne();
                Console.WriteLine("Thread 3 received a manual signal, continue working");
            });

            Thread t4 = new Thread(() =>
            {
                Console.WriteLine("Thread 4 is waiting for a manual singal from Thread 1");
                _manualResetEvent.WaitOne();
                Console.WriteLine("Thread 4 received a manual signal, continue working");
            });

            Thread t5 = new Thread(() =>
            {
                Console.WriteLine("Thread 5 is waiting for a auto singal from Thread 2");
                _autoResetEvent.WaitOne();
                Console.WriteLine("Thread 5 received an auto signal, continue working");
            });

            Thread t6 = new Thread(() =>
            {
                Console.WriteLine("Thread 6 is waiting for a auto singal from Thread 2");
                _autoResetEvent.WaitOne();
                Console.WriteLine("Thread 6 received an auto signal, continue working");
            });
          
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();
            t6.Start();

            _mutex.ReleaseMutex();
        }
    }
}
