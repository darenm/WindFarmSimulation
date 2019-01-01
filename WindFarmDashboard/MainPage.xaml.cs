using System;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using WindFarmDashboard.Controls;
using WindFarmDashboard.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindFarmDashboard
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void TurbineClicked(object sender, ItemClickEventArgs e)
        {
            // if we haven't stored a connection string let's prompt the user
            if (e.ClickedItem is WindTurbine turbine)
            {
                var dlg = new ConnectionStringDialog
                {
                    ConnectionString = {Text = turbine.DeviceConnectionString ?? string.Empty}
                };
                var result = await dlg.ShowAsync();
                if (result.ToString() == "Primary")
                {
                    turbine.DeviceConnectionString = dlg.ConnectionString.Text;
                    ApplicationData.Current.LocalSettings.Values[$"device-connection-string-{turbine.Name}"] =
                        turbine.DeviceConnectionString;
                }
            }
        }
    }
}