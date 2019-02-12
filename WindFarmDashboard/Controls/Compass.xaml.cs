using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WindFarmDashboard.Controls
{
    public sealed partial class Compass : UserControl
    {
        public static readonly DependencyProperty WindDirectionProperty = DependencyProperty.Register(
            "WindDirection", typeof(double), typeof(Compass), new PropertyMetadata(23.3, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Compass;
            control?.UpdateAngle();
        }

        private void UpdateAngle()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                AngleStoryboard.Begin();
            }
        }

        public Compass()
        {
            InitializeComponent();
        }

        public double WindDirection
        {
            get => (double) GetValue(WindDirectionProperty);
            set => SetValue(WindDirectionProperty, value);
        }

        public static string FormatWindDirection(double value)
        {
            return $"{value:N0}°" ;
        }
    }
}