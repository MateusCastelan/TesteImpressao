

using Plugin.BLE.Abstractions.Contracts;
using TesteImpressao.ViewModels;

namespace TesteImpressao
{
    public partial class MainPage : ContentPage
    {

        private MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = BindingContext as MainPageViewModel;
        }

        private async void OnDeviceTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is IDevice device)
            {
                _viewModel.SelectedDevice = device;
                await _viewModel.ConnectToDeviceAsync(device);
            }
        }
    }

}
