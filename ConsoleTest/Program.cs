using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Simulator.Library;

namespace ConsoleTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var turbine = new WindTurbineModel();
            PrintTurbine(turbine);

            var count = 1000;
            while (count-- > 0)
            {
                turbine.WindSpeed = 15 + VarianceGenerator.Generate(3);
                for (int i = 0; i < 100; i++)
                {
                    await Task.Delay(500);
                    PrintTurbine(turbine);
                }
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }

        private static void PrintTurbine(WindTurbineModel turbine)
        {
            //Console.WriteLine(
            //    $"Wind Speed: {turbine.WindSpeed:N2} m/s - Low Speed Shaft: {turbine.LowSpeedShaftRpm:N2} " +
            //    $"RPM - Power output: {turbine.Power:N2} kW - Is Brake On: {turbine.IsTurbineBrakeOn} - Gen Temp: {turbine.GeneratorTemperatureCelsius:N2} Celsius");
            var json = turbine.ToJson();
            Console.WriteLine(json);
        }
    }
}