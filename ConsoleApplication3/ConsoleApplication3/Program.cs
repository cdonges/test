using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    /// <summary>
    /// Worker for multiplying numbers
    /// </summary>
    class Worker
    {
        /// <summary>
        /// Keep track of total
        /// </summary>
        public double total = 1;

        Factorial f;
        public Worker(Factorial f)
        {
            this.f = f;
        }

        public void Start()
        {
            // Get number to multoply with what we already have, if -1 then exit
            double value = f.GetWork();
            while (value != -1)
            {
                total *= value;
                value = f.GetWork();
                ////Thread.Sleep(1);
            }
        }
    }

    class Factorial
    {
        // Numbers that haven't been sent to worker
        private int unallocated;

        public double GetWork()
        {
            int work;
            // Use interlocked so we can be thread safe, -1 if none left
            if ((work = Interlocked.Decrement(ref this.unallocated)) > 0)
            {
                return work;
            }
            else
            {
                return -1;
            }
        }

        public double Calc(int factorial, int numThreads)
        {
            // Start at one higher because first thing we do is decrement
            this.unallocated = factorial + 1;

            // Create and start workers
            Worker[] work = new Worker[numThreads];
            Thread[] threads = new Thread[numThreads];
            for (long i = 0; i < numThreads; i++)
            {
                work[i] = new Worker(this);
                Thread thread = new Thread(work[i].Start);
                threads[i] = thread;
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            // Multiply the results of workers
            double result = 1;
            for (long i = 0; i < numThreads; i++)
            {
                result *= work[i].total;
            }

            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int threads = 1;
            int factorial = 1;

            Console.WriteLine("What number?");
            string factorialString = Console.ReadLine();

            Console.WriteLine("How many threads?");
            string threadsString = Console.ReadLine();

            if (int.TryParse(factorialString, out factorial) && int.TryParse(threadsString, out threads) && threads > 0 && factorial >= 0)
            {
                Factorial f = new Factorial();

                double result = f.Calc(factorial, threads);

                Console.WriteLine(result);
            }

            Console.ReadKey();
        }
    }
}
