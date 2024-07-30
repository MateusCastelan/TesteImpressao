using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace TesteImpressao
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private const int RequestBluetoothPermissionsId = 1000;
        private readonly string[] _bluetoothPermissions =
        {
        Android.Manifest.Permission.BluetoothScan,
        Android.Manifest.Permission.BluetoothConnect
    };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Check if the app already has Bluetooth permissions
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.BluetoothScan) != Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.BluetoothConnect) != Permission.Granted)
            {
                // Request Bluetooth permissions
                ActivityCompat.RequestPermissions(this, _bluetoothPermissions, RequestBluetoothPermissionsId);
            }
        }

        // Handle the result of the permission request
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestBluetoothPermissionsId)
            {
                // Check if the permissions were granted or not
                bool allPermissionsGranted = grantResults.All(result => result == Permission.Granted);

                if (!allPermissionsGranted)
                {
                    // Handle the case when permissions are not granted
                    // You may want to show a message to the user or disable certain features
                }
            }
        }
    }
}