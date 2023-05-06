namespace IotInjectionFunctionApp.Model
{
    internal class CameraTelemetryDataModel
    {
        public double Illuminance { get; set; }
        public static CameraTelemetryDataModel GetCameraTelemetryDataModel(double illuminance)
        {
            CameraTelemetryDataModel cameraTelemetryDataModel = new CameraTelemetryDataModel();
            cameraTelemetryDataModel.Illuminance = illuminance;
            return cameraTelemetryDataModel;
        }
    }
}