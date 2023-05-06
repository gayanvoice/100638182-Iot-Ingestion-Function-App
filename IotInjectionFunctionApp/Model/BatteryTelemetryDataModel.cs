namespace IotInjectionFunctionApp.Model
{
    internal class BatteryTelemetryDataModel
    {
        public double Temperature { get; set; }
        public int Voltage { get; set; }
        public bool IsCharging { get; set; }
        public string Power { get; set; }

        public static BatteryTelemetryDataModel GetBatteryTelemetryDataModel
            (float temperature, int voltage, bool isCharging, string power)
        {
            BatteryTelemetryDataModel batterytelemetryDataModel = new BatteryTelemetryDataModel();
            batterytelemetryDataModel.Temperature = temperature;
            batterytelemetryDataModel.Voltage = voltage;
            batterytelemetryDataModel.IsCharging = isCharging;
            batterytelemetryDataModel.Power = power;
            return batterytelemetryDataModel;
        }
    }
}