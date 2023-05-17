using OpenHardwareMonitor.Hardware;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxGameBarTemperature
{
    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

    public class HardwareInfoService
    {
        static float cpuTemp;
        // CPU Usage
        static float cpuUsage;
        // CPU Power Draw (Package)
        static float cpuPowerDrawPackage;
        // CPU Frequency
        static float cpuFrequency;
        // GPU Temperature
        static float gpuTemp;
        // GPU Usage
        static float gpuUsage;
        // GPU Core Frequency
        static float gpuCoreFrequency;
        // GPU Memory Frequency
        static float gpuMemoryFrequency;
        readonly Computer computer;
        readonly IHardware nVidia;
        readonly ISensor nVidiaTemperature;

        public HardwareInfoService()
        {
            computer = new()
            {
                CPUEnabled = true,
                GPUEnabled = true,
                RAMEnabled = false,
                MainboardEnabled = false,
                FanControllerEnabled = false,
                NICEnabled = false,
                HDDEnabled = false
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            nVidia = computer.Hardware.First(x => x.HardwareType == HardwareType.GpuNvidia);
            nVidiaTemperature = nVidia.Sensors.First(x => x.SensorType == SensorType.Temperature && x.Name.Contains("GPU Core"));
        }

        public async Task<float?> ReadGpuTemperature()
        {
            float? result = await Task.Run(() => {
                nVidia.Update();
                return nVidiaTemperature.Value;
            });

            return result;
        }

        public string ReadInfo()
        {
            foreach (IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.CPU)
                {
                    // only fire the update when found
                    hardware.Update();

                    // loop through the data
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("CPU Package"))
                        {
                            // store
                            cpuTemp = sensor.Value.GetValueOrDefault();
                            // print to console
                            System.Diagnostics.Debug.WriteLine("cpuTemp: " + sensor.Value.GetValueOrDefault());

                        }
                        else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
                        {
                            // store
                            cpuUsage = sensor.Value.GetValueOrDefault();
                            // print to console
                            System.Diagnostics.Debug.WriteLine("cpuUsage: " + sensor.Value.GetValueOrDefault());

                        }
                        else if (sensor.SensorType == SensorType.Power && sensor.Name.Contains("CPU Package"))
                        {
                            // store
                            cpuPowerDrawPackage = sensor.Value.GetValueOrDefault();
                            // print to console
                            System.Diagnostics.Debug.WriteLine("CPU Power Draw - Package: " + sensor.Value.GetValueOrDefault());


                        }
                        else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("CPU Core #1"))
                        {
                            // store
                            cpuFrequency = sensor.Value.GetValueOrDefault();
                            // print to console
                            System.Diagnostics.Debug.WriteLine("cpuFrequency: " + sensor.Value.GetValueOrDefault());
                        }
                    }
                }

                // Targets AMD & Nvidia GPUS
                if (hardware.HardwareType == HardwareType.GpuAti || hardware.HardwareType == HardwareType.GpuNvidia)
                {
                    // only fire the update when found
                    hardware.Update();

                    // loop through the data
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("GPU Core"))
                        {
                            // store
                            gpuTemp = sensor.Value.GetValueOrDefault();
                            // print to console
                            System.Diagnostics.Debug.WriteLine("gpuTemp: " + sensor.Value.GetValueOrDefault());
                        }
                        else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("GPU Core"))
                        {
                            // store
                            gpuUsage = sensor.Value.GetValueOrDefault();
                            // print to console
                            System.Diagnostics.Debug.WriteLine("gpuUsage: " + sensor.Value.GetValueOrDefault());
                        }
                        else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("GPU Core"))
                        {
                            // store
                            gpuCoreFrequency = sensor.Value.GetValueOrDefault();
                            // print to console
                            System.Diagnostics.Debug.WriteLine("gpuCoreFrequency: " + sensor.Value.GetValueOrDefault());
                        }
                        else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("GPU Memory"))
                        {
                            // store
                            gpuMemoryFrequency = sensor.Value.GetValueOrDefault();
                            // print to console
                            System.Diagnostics.Debug.WriteLine("gpuMemoryFrequency: " + sensor.Value.GetValueOrDefault());
                        }
                    }
                }

                //foreach (ISensor sensor in hardware.Sensors)
                //{
                //    sb.AppendLine($"\tSensor: {sensor.Name}, value: {sensor.Value}");
                //}
            }

            computer.Close();

            string result = $"""
                            CPU Temperature{cpuTemp}
                            CPU Usage {cpuUsage}
                            CPU Power Draw (Package) {cpuPowerDrawPackage}
                            CPU Frequency {cpuFrequency}
                            GPU Temperature {gpuTemp}
                            GPU Usage {gpuUsage}
                            GPU Core Frequency {gpuCoreFrequency}
                            GPU Memory Frequency {gpuMemoryFrequency}
                            """;
            return result;
        }
    }
}