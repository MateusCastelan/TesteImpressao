using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Plugin.BLE.Abstractions.EventArgs;
using TesteImpressao.Models;

namespace TesteImpressao.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IAdapter _adapter;
        private readonly ObservableCollection<DeviceWrapper> _devices;

        public ObservableCollection<DeviceWrapper> Devices => _devices;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private bool _isScanning;

        public MainPageViewModel()
        {
            var bluetoothLE = CrossBluetoothLE.Current;
            _adapter = bluetoothLE.Adapter;
            _devices = new ObservableCollection<DeviceWrapper>();

            _adapter.DeviceDiscovered += OnDeviceDiscovered;
            _adapter.ScanTimeoutElapsed += OnScanTimeoutElapsed;
            _adapter.DeviceConnected += OnDeviceConnected;
            _adapter.DeviceDisconnected += OnDeviceDisconnected;
            _adapter.DeviceConnectionLost += OnDeviceConnectionLost;

            Debug.WriteLine("Bluetooth initialized.");
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            try
            {
                if (args.Device != null && !_devices.Any(d => d.Device.Id == args.Device.Id))
                {
                    _devices.Add(new DeviceWrapper(args.Device));
                    StatusMessage = "Device found: " + args.Device.Name;
                    Debug.WriteLine($"Device found: {args.Device.Name}, Id: {args.Device.Id}");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error in OnDeviceDiscovered: {ex.Message}";
                Debug.WriteLine($"Exception in OnDeviceDiscovered: {ex}");
            }
        }

        private void OnScanTimeoutElapsed(object sender, EventArgs args)
        {
            IsScanning = false;
            StatusMessage = "Scan completed.";
            Debug.WriteLine("Scan completed.");
        }

        private void OnDeviceConnected(object sender, DeviceEventArgs args)
        {
            var deviceWrapper = _devices.FirstOrDefault(d => d.Device.Id == args.Device.Id);
            if (deviceWrapper != null)
            {
                deviceWrapper.IsConnected = true;
                StatusMessage = "Connected to: " + args.Device.Name;
                Debug.WriteLine($"Connected to: {args.Device.Name}, Id: {args.Device.Id}");
            }
        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs args)
        {
            var deviceWrapper = _devices.FirstOrDefault(d => d.Device.Id == args.Device.Id);
            if (deviceWrapper != null)
            {
                deviceWrapper.IsConnected = false;
                StatusMessage = "Disconnected from: " + args.Device.Name;
                Debug.WriteLine($"Disconnected from: {args.Device.Name}, Id: {args.Device.Id}");
            }
        }

        private void OnDeviceConnectionLost(object sender, DeviceEventArgs args)
        {
            var deviceWrapper = _devices.FirstOrDefault(d => d.Device.Id == args.Device.Id);
            if (deviceWrapper != null)
            {
                deviceWrapper.IsConnected = false;
                StatusMessage = "Connection lost to: " + args.Device.Name;
                Debug.WriteLine($"Connection lost to: {args.Device.Name}, Id: {args.Device.Id}");
            }
        }

        [RelayCommand]
        public async Task StartScanningAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted && CrossBluetoothLE.Current.State == BluetoothState.On)
                {
                    IsScanning = true;
                    StatusMessage = "Scanning for devices...";
                    Debug.WriteLine("Scanning for devices...");
                    _devices.Clear();
                    await _adapter.StartScanningForDevicesAsync();
                }
                else
                {
                    StatusMessage = "Bluetooth is not enabled or permission denied.";
                    Debug.WriteLine("Bluetooth is not enabled or permission denied.");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error during scanning: {ex.Message}";
                Debug.WriteLine($"Exception during scanning: {ex}");
            }
        }

        [RelayCommand]
        public async Task ConnectToDeviceAsync(DeviceWrapper deviceWrapper)
        {
            try
            {
                if (deviceWrapper?.Device != null)
                {
                    StatusMessage = "Connecting to: " + deviceWrapper.Device.Name;
                    Debug.WriteLine($"Connecting to: {deviceWrapper.Device.Name}, Id: {deviceWrapper.Device.Id}");
                    await _adapter.ConnectToDeviceAsync(deviceWrapper.Device);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error during connection: {ex.Message}";
                Debug.WriteLine($"Exception during connection: {ex}");
            }
        }
    }
}
