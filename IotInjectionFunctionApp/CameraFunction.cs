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
using IotInjectionFunctionApp.Model;

namespace IotInjectionFunctionApp
{
    public static class CameraFunction
    {
        static string deviceConnectionString = Environment.GetEnvironmentVariable("IOT_DEVICE_CONNECTION_STRING");
        [FunctionName("CameraIotIngestion")]
        public static async Task<IActionResult> RunAsync(
           [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            float illuminance = 0.0f;
            if (req.Query.ContainsKey("Illuminance"))
            {
                if (!float.TryParse(req.Query["Illuminance"], out illuminance))
                {
                    return new BadRequestObjectResult("Invalid illuminance value. Please provide a valid float.");
                }
            }
            else
            {
                return new BadRequestObjectResult("Please provide an illuminance value.");
            }
            await SendDeviceToCloudMessageAsync(illuminance);
            return new OkObjectResult($"Illuminance received: {illuminance}");
        }
        public static async Task SendDeviceToCloudMessageAsync(float illuminance)
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString);
            CameraTelemetryDataModel cameratelemetryDataModel = new CameraTelemetryDataModel();
            cameratelemetryDataModel = CameraTelemetryDataModel.GetCameraTelemetryDataModel(illuminance);
            string messageString = JsonSerializer.Serialize(cameratelemetryDataModel);
            Message message = new Message(Encoding.UTF8.GetBytes(messageString))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8"
            };
            await deviceClient.SendEventAsync(message);
            Console.WriteLine($"{DateTime.Now} > Sending message: {messageString}");
        }
    }
}