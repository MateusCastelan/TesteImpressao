

using Plugin.BLE.Abstractions.Contracts;
using TesteImpressao.Models;
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
            if (e.Item is DeviceWrapper deviceWrapper)
            {
                await _viewModel.ConnectToDeviceAsync(deviceWrapper);
            }

        ((ListView)sender).SelectedItem = null;
        }
    }
}