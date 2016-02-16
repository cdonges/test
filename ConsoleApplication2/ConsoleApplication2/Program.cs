using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("How many cars?");
            string carsString = System.Console.ReadLine();

            int numberOfCars;
            if (int.TryParse(carsString, out numberOfCars) && numberOfCars > 0)
            {
                SynchronizedRandomGenerator synchronizedRandomGenerator = new ConsoleApplication2.SynchronizedRandomGenerator(0, 100);

                List<Thread> threads = new List<Thread>();

                // set up threads
                for (int i = 0; i <= numberOfCars; i++)
                {
                    Car car = new Car(i, synchronizedRandomGenerator, 1000);
                    threads.Add(new Thread(car.Race));
                }

                // Start threads here rather than in for loop above to give later cars a smaller disadvantage
                foreach (var thread in threads)
                {
                    thread.Start();
                }

                // Wait for all cars to finish
                foreach (var thread in threads)
                {
                    thread.Join();
                }
            }
            else
            {
                System.Console.WriteLine("Please enter a number more than 0 next time");
            }
            
            System.Console.ReadKey();
        }
    }

    // A class which can return Randomized numbers in a 
    // synchronized manner.
    public sealed class SynchronizedRandomGenerator
    {
        private System.Random random = new Random();

        private object randomLock = new object();

        private int minValue;
        private int maxValue;

        // Initialize the random number generator.
        // Instantiate a "System.Random" object internally to 
        // generate
        // random numbers.
        public SynchronizedRandomGenerator(
               int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
        // Use the "Next(minValue, maxValue)" method on
        // a "System.Random" object to get a random number
        // within the requested range.
        // Note that the "System.Random" object itself is not thread
        // safe.
        public int Next()
        {
            lock (randomLock) // or could have used a threadlocal variable
            {
                return random.Next(this.minValue, this.maxValue);
            }
        }
    }

    public sealed class Car
    {
        private int total = 0;
        
        private int carId;
        private SynchronizedRandomGenerator randomGenerator;
        private int destKm;

        // Initialize the car with a random generator
        // and destination number of kilomteres required
        // to finish the race.
        public Car(int carId,
                   SynchronizedRandomGenerator randomGenerator,
                   int destKm)
        {
            this.carId = carId;
            this.randomGenerator = randomGenerator;
            this.destKm = destKm;
        }
        // A car should (in a loop):
        //   1. Generate a random number of kilometers
        //      past by using the random generator.
        //   2. Sum the number of kilometers past, 
        //      and write an update to the console.
        //   3. Sleep for 10 milliseconds (Thread.Sleep(10)).
        //   4. Once the destination number of kilometers is 
        //      achieved - write the name of the car to the 
        //      console stating that it finished.
        // Note that this method should be executed on a dedicated 
        // thread.
        public void Race()
        {
            while (total < this.destKm)
            {
                int travelled = randomGenerator.Next();
                this.total += travelled;

                Console.WriteLine("Car " + carId + ": " + this.total + "km");

                Thread.Sleep(10);
            }

            Console.WriteLine("Car " + carId + ": FINISHED");
        }
    }

}
