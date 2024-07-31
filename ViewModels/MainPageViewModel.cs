using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Plugin.BLE.Abstractions.EventArgs;

namespace TesteImpressao.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IAdapter _adapter;
        private readonly ObservableCollection<IDevice> _devices;

        public ObservableCollection<IDevice> Devices => _devices;

        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private bool _isScanning;

        private IDevice _selectedDevice;
        public IDevice SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public MainPageViewModel()
        {
            var bluetoothLE = CrossBluetoothLE.Current;
            _adapter = bluetoothLE.Adapter;
            _devices = new ObservableCollection<IDevice>();

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
                if (args.Device != null && !_devices.Contains(args.Device))
                {
                    _devices.Add(args.Device);
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
            StatusMessage = "Connected to: " + args.Device.Name;
            Debug.WriteLine($"Connected to: {args.Device.Name}, Id: {args.Device.Id}");
        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs args)
        {
            StatusMessage = "Disconnected from: " + args.Device.Name;
            Debug.WriteLine($"Disconnected from: {args.Device.Name}, Id: {args.Device.Id}");
        }

        private void OnDeviceConnectionLost(object sender, DeviceEventArgs args)
        {
            StatusMessage = "Connection lost to: " + args.Device.Name;
            Debug.WriteLine($"Connection lost to: {args.Device.Name}, Id: {args.Device.Id}");
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
        public async Task ConnectToDeviceAsync(IDevice device)
        {
            if (device == null)
            {
                StatusMessage = "No device selected.";
                return;
            }

            try
            {
                StatusMessage = $"Connecting to: {device.Name}, Id: {device.Id}";
                Debug.WriteLine($"Connecting to: {device.Name}, Id: {device.Id}");
                await _adapter.ConnectToDeviceAsync(device);

                StatusMessage = $"Successfully connected to: {device.Name}, Id: {device.Id}";
                Debug.WriteLine($"Successfully connected to: {device.Name}, Id: {device.Id}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error during connection: {ex.Message}";
                Debug.WriteLine($"Exception during connection: {ex}");
            }
        }

        [RelayCommand]
        public async Task SendDataToPrinterAsync(string data)
        {
            if (SelectedDevice == null)
            {
                StatusMessage = "No device selected.";
                return;
            }

            try
            {
                var service = await SelectedDevice.GetServiceAsync(Guid.Parse("3bfa1b0c-64eb-439b-aee8-c5743766ffea"));
                var characteristic = await service.GetCharacteristicAsync(Guid.Parse("3bfa1b0c-64eb-439b-aee8-c5743766ffea"));

                if (characteristic != null)
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(data);
                    await characteristic.WriteAsync(bytes);
                    StatusMessage = "Data sent to printer.";
                    Debug.WriteLine("Data sent to printer.");
                }
                else
                {
                    StatusMessage = "Characteristic not found.";
                    Debug.WriteLine("Characteristic not found.");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error during data transfer: {ex.Message}";
                Debug.WriteLine($"Exception during data transfer: {ex}");
            }
        }
    }
}