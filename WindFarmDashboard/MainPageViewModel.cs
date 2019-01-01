using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Simulator.Library;
using WindFarmDashboard.Annotations;
using WindFarmDashboard.Models;

namespace WindFarmDashboard
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _tickTimer;
        private readonly Random _capturedRandom;

        private readonly WindTurbineModel[] _turbineModels =
        {
            new WindTurbineModel {Name = "CWF-001"},
            new WindTurbineModel {Name = "CWF-002"},
            new WindTurbineModel {Name = "CWF-003"},
            new WindTurbineModel {Name = "CWF-004"},
            new WindTurbineModel {Name = "CWF-005"},
            new WindTurbineModel {Name = "CWF-006"},
            new WindTurbineModel {Name = "CWF-007"},
            new WindTurbineModel {Name = "CWF-008"},
            new WindTurbineModel {Name = "CWF-009"},
            new WindTurbineModel {Name = "CWF-010"}
        };

        public ObservableCollection<WindTurbine> Turbines
        {
            get => _turbines;
            private set
            {
                if (Equals(value, _turbines)) return;
                _turbines = value;
                OnPropertyChanged();
            }
        }

        private double _windDirection;
        private readonly VarianceDelayedDouble _windDirectionWithVariance;
        private string _windSpeed;
        private readonly VarianceDelayedDouble _windSpeedWithVariance;
        private ObservableCollection<WindTurbine> _turbines;
        private int _numberOfTurbines;
        private double _totalPower;

        public MainPageViewModel()
        {
            _capturedRandom = new Random();

            _windDirectionWithVariance = new VarianceDelayedDouble(_capturedRandom.NextDouble() * 359)
            { StepDelay = TimeSpan.FromMilliseconds(500), ValueLag = TimeSpan.FromSeconds(5), Variance = 3.3 };
            _windSpeedWithVariance = new VarianceDelayedDouble(_capturedRandom.NextDouble() * 20)
            { StepDelay = TimeSpan.FromMilliseconds(500), ValueLag = TimeSpan.FromSeconds(5), Variance = 1 };

            InitializeTurbineModels();

            _tickTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _tickTimer.Tick += TickTimerOnTick;
            _tickTimer.Start();
        }

        private void InitializeTurbineModels()
        {
            Turbines = new ObservableCollection<WindTurbine>();
            foreach (var windTurbineModel in _turbineModels)
            {
                var turbine = new WindTurbine();
                turbine.Update(windTurbineModel.ToDto());
                turbine.DeviceConnectionString =
                    ApplicationData.Current.LocalSettings.Values[$"device-connection-string-{turbine.Name}"]?.ToString();
                Turbines.Add(turbine);
            }

            _turbines[0].DeviceConnectionString =
                "HostName=capstonehub.azure-devices.net;DeviceId=CWF-001;SharedAccessKey=RKfh8J136ZXx3o7D7rJGaU+zT9cxxjkkazodNAnpae4=";
        }

        public double WindDirection
        {
            get => _windDirection;
            set
            {
                if (value.Equals(_windDirection)) return;
                _windDirection = value;
                OnPropertyChanged();
            }
        }

        public string WindSpeed
        {
            get => _windSpeed;
            set
            {
                if (value.Equals(_windSpeed)) return;
                _windSpeed = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfTurbines
        {
            get => _numberOfTurbines;
            set
            {
                if (value == _numberOfTurbines) return;
                _numberOfTurbines = value;
                OnPropertyChanged();
            }
        }

        public double TotalPower
        {
            get => _totalPower;
            set
            {
                if (value.Equals(_totalPower)) return;
                _totalPower = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void TickTimerOnTick(object sender, object e)
        {
            try
            {
                await UpdateWindData();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async Task UpdateWindData()
        {
            var windspeedChanged = false;
            if (_capturedRandom.NextDouble() > 0.97)
            {
                // change direction
                _windDirectionWithVariance.Value = _capturedRandom.NextDouble() * 359;
                _windSpeedWithVariance.Value = _capturedRandom.NextDouble() * 20;
                windspeedChanged = true;
            }

            var windDirection = _windDirectionWithVariance.Value;
            if (windDirection > 359) windDirection -= 360;
            if (windDirection < 0) windDirection += 360;
            WindDirection = windDirection;

            var windSpeed = _windSpeedWithVariance.Value;
            if (windSpeed < 0) windSpeed = 0;
            WindSpeed = $"{windSpeed:N2} m/s";

            for (var index = 0; index < _turbineModels.Length; index++)
            {
                var windTurbineModel = _turbineModels[index];
                if (windspeedChanged)
                {
                    windTurbineModel.WindSpeed = windSpeed;
                }
                _turbines[index].Update(windTurbineModel.ToDto());
                await _turbines[index].SendTelemetry();
            }

            TotalPower = Turbines.Sum(t => t.Power);
            NumberOfTurbines = Turbines.Count();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}