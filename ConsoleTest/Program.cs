using System;
using System.Threading.Tasks;
using Simulator.Library;

namespace ConsoleTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var dd = new DelayedDouble(100);

            Console.WriteLine($"Current value: {dd.Value}");

            var count = 100;

            //dd.Value = 200;
            //while (count-- > 0)
            //{
            //    await Task.Delay(TimeSpan.FromMilliseconds(50));

            //    Console.WriteLine(dd.Value);

            //    if (count == 85)
            //    {
            //        //dd.Value = 75;
            //    }
            //}

            //dd.Value = 50;
            //count = 100;
            //while (count-- > 0)
            //{
            //    await Task.Delay(TimeSpan.FromMilliseconds(50));

            //    Console.WriteLine(dd.Value);

            //    if (count == 30) dd.Value = 400;
            //}

            var vd = new VarianceDelayedDouble(100);
            count = 20;
            while (count-- > 0)
            {
                Console.WriteLine(vd.Value);

                if (count == 10) vd.Variance = 2.5;
            }

            vd.Value = 50;
            count = 100;
            while (count-- > 0)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50));

                Console.WriteLine(vd.Value);

                if (count == 75) vd.Value = 400;
            }
        }
    }
}