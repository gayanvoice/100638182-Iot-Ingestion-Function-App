using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using System.Text;
using System.Text.Json;
using IotInjectionBatteryFunctionApp.Model;

namespace IotInjectionCameraFunctionApp
{
    public static class BatteryFunction
    {
       static string deviceConnectionString = Environment.GetEnvironmentVariable("IOT_DEVICE_CONNECTION_STRING");

        //http://localhost:7137/api/Info?Temperature=36.0&Voltage=4430&IsCharging=true&Power=USB
        [FunctionName("Info")]
        public static async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            float temperature = 0.0f;
            int voltage = 0;
            bool isCharging = false;
            string power = null;

            if (req.Query.ContainsKey("Temperature"))
            {
                if (!float.TryParse(req.Query["Temperature"], out temperature))
                {
                    return new BadRequestObjectResult("Invalid Temperature value. Please provide a valid float.");
                }
            }
            else
            {
                return new BadRequestObjectResult("Please provide an Temperature value.");
            }

            if (req.Query.ContainsKey("Voltage"))
            {
                if (!int.TryParse(req.Query["Voltage"], out voltage))
                {
                    return new BadRequestObjectResult("Invalid Voltage value. Please provide a valid int.");
                }
            }
            else
            {
                return new BadRequestObjectResult("Please provide an Voltage value.");
            }

            if (req.Query.ContainsKey("IsCharging"))
            {
                if (!bool.TryParse(req.Query["IsCharging"], out isCharging))
                {
                    return new BadRequestObjectResult("Invalid IsCharging value. Please provide a valid bool.");
                }
            }
            else
            {
                return new BadRequestObjectResult("Please provide an IsCharging value.");
            }

            if (req.Query.ContainsKey("Power"))
            {
                power = req.Query["Power"];
            }
            else
            {
                return new BadRequestObjectResult("Please provide an Power value.");
            }

            await SendDeviceToCloudMessageAsync(temperature, voltage, isCharging, power);
            return new OkObjectResult($"Temperature: {temperature} Voltage: {voltage}  IsCharging: {isCharging} Power: {power}");
        }

        public static async Task SendDeviceToCloudMessageAsync(float temperature, int voltage, bool isCharging, string power)
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);
            BatteryTelemetryDataModel batterytelemetryDataModel = new BatteryTelemetryDataModel();
            batterytelemetryDataModel.Temperature = temperature;
            batterytelemetryDataModel.Voltage = voltage;
            batterytelemetryDataModel.IsCharging = isCharging;
            batterytelemetryDataModel.Power = power;
            var messageString = JsonSerializer.Serialize(batterytelemetryDataModel);
            var message = new Message(Encoding.UTF8.GetBytes(messageString))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8"
            };
            await deviceClient.SendEventAsync(message);
            Console.WriteLine($"{DateTime.Now} > Sending message: {messageString}");
        }

    }
}