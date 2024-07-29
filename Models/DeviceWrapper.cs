using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;

namespace TesteImpressao.Models
{
    public class DeviceWrapper : ObservableObject
    {
        private bool _isConnected;

        public DeviceWrapper(IDevice device)
        {
            Device = device;
        }

        public IDevice Device { get; }

        public string Name => Device.Name;

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }
    }
}
