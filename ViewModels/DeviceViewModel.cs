using Plugin.BLE.Abstractions.Contracts;

namespace TesteImpressao.ViewModels
{
    public class DeviceViewModel
    {
        public IDevice Device { get; }

        public string Name => Device.Name ?? "Unknown";
        public string Id => Device.Id.ToString();

        public DeviceViewModel(IDevice device)
        {
            Device = device;
        }
    }
}