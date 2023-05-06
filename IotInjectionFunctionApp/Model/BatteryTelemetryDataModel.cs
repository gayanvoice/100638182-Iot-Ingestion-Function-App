namespace IotInjectionFunctionApp.Model
{
    internal class BatteryTelemetryDataModel
    {
        public double Temperature { get; set; }
        public int Voltage { get; set; }
        public bool IsCharging { get; set; }
        public string Power { get; set; }
    }
}