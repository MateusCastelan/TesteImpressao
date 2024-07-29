

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

        private async void OnDeviceTapped(object sender, EventArgs e)
        {
            if (sender is TextCell textCell && textCell.BindingContext is IDevice device)
            {
                await _viewModel.ConnectToDeviceAsync(device);
            }
        }
    }

}
