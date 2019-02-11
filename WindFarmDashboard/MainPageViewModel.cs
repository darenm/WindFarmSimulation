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
        private DeviceMessage _deviceMessage;
        private string _studentId;
        private long _studentIdNumeric;
        private string _studentIdErrors;
        private bool _isStudentIdValid;
        private bool _isTelemetryRunning;

        public MainPageViewModel()
        {
            _deviceMessage = new DeviceMessage
            {
                Metadata = new MetadataDto
                {
                    DeviceType = "SimulatedTurbine",
                    StudentId = "AABBCCDD"
                }
            };
            _capturedRandom = new Random();

            _windDirectionWithVariance = new VarianceDelayedDouble(_capturedRandom.NextDouble() * 359)
            { StepDelay = TimeSpan.FromMilliseconds(500), ValueLag = TimeSpan.FromSeconds(5), Variance = 3.3 };
            _windSpeedWithVariance = new VarianceDelayedDouble(_capturedRandom.NextDouble() * 20)
            { StepDelay = TimeSpan.FromMilliseconds(500), ValueLag = TimeSpan.FromSeconds(5), Variance = 1 };

            InitializeTurbineModels();

            _tickTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _tickTimer.Tick += TickTimerOnTick;

            // Don't start here anymore - start when a valid student ID is entered and start is hit
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

        public string StudentId
        {
            get => _studentId;
            set
            {
                if (value.Equals(_studentId)) return;
                _studentId = value;
                _deviceMessage.Metadata.StudentId = _studentId;
                StudentIdErrors = string.Empty;
                if (string.IsNullOrWhiteSpace(_studentId))
                {
                    StudentIdErrors = "Student ID cannot be empty";
                    IsStudentIdValid = false;
                }
                else if (_studentId.Length != 8)
                {
                    StudentIdErrors = "Student ID must be an 8 character Hex string";
                    IsStudentIdValid = false;
                }
                else 
                {
                    try
                    {
                        _studentIdNumeric = Convert.ToInt64(_studentId, 16);
                        IsStudentIdValid = true;
                    }
                    catch 
                    {
                        StudentIdErrors = "Student ID must be an 8 character Hex string";
                        IsStudentIdValid = false;
                    }
                }
                OnPropertyChanged();
            }
        }

        public string StudentIdErrors
        {
            get => _studentIdErrors;
            set
            {
                if (value.Equals(_studentIdErrors)) return;
                _studentIdErrors = value;
                OnPropertyChanged();
            }
        }

        public bool IsStudentIdValid
        {
            get => _isStudentIdValid;
            set
            {
                if (value.Equals(_isStudentIdValid)) return;
                _isStudentIdValid = value;
                OnPropertyChanged();
            }
        }

        public bool IsTelemetryRunning
        {
            get => _isTelemetryRunning;
            set
            {
                if (value.Equals(_isTelemetryRunning)) return;
                _isTelemetryRunning = value;
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

        public void StartTelemetry()
        {
            if (_isStudentIdValid)
            {
                _tickTimer?.Start();
                IsTelemetryRunning = true;

                // update the turbine windspeeds
                foreach (var windTurbineModel in _turbineModels)
                {
                    windTurbineModel.WindSpeed = _windSpeedWithVariance.Value;
                }
            }
        }

        public void StopTelemetry()
        {
            _tickTimer?.Stop();
            IsTelemetryRunning = false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}