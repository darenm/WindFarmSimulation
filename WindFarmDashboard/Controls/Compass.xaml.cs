using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WindFarmDashboard.Controls
{
    public sealed partial class Compass : UserControl
    {

        public static readonly DependencyProperty WindDirectionProperty = DependencyProperty.Register(
            "WindDirection", typeof(double), typeof(Compass), new PropertyMetadata(23.3));

        public double WindDirection
        {
            get { return (double) GetValue(WindDirectionProperty); }
            set { SetValue(WindDirectionProperty, value); }
        }
        public Compass()
        {
            this.InitializeComponent();
        }

        public static string FormatWindDirection(double value)
        {
            return value.ToString("N2");
        }
    }
}
