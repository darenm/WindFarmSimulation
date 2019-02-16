using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Simulator.Library;
using Simulator.Library.Models;
using Simulator.Library.Utilities;
using WindFarmDashboard.Annotations;
using WindFarmDashboard.Models;

namespace WindFarmDashboard
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _tickTimer;

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

        private Random _capturedRandom;
        private readonly MetadataDto _metadataDto;
        private bool _isStudentIdValid;
        private bool _isTelemetryRunning;
        private int _numberOfTurbines;
        private string _studentId;
        private string _studentIdErrors;
        private int _studentIdNumeric;
        private double _totalPower;
        private ObservableCollection<WindTurbine> _turbines;
        private double _windDirection;
        private VarianceDelayedDouble _windDirectionWithVariance;
        private string _windSpeed;
        private VarianceDelayedDouble _windSpeedWithVariance;
        private int _wornTurbine;

        public MainPageViewModel()
        {
            _metadataDto = new MetadataDto
            {
                DeviceType = "SimulatedTurbine"
            };


            InitializeTurbineModels();

            _tickTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _tickTimer.Tick += TickTimerOnTick;

            var tempId = ApplicationData.Current.LocalSettings.Values["StudentId"]?.ToString();
            if (tempId != null)
            {
                StudentId = tempId;
            }
        }

        public ObservableCollection<WindTurbine> Turbines
        {
            get => _turbines;
            private set
            {
                if (Equals(value, _turbines))
                {
                    return;
                }

                _turbines = value;
                OnPropertyChanged();
            }
        }

        public double WindDirection
        {
            get => _windDirection;
            set
            {
                if (value.Equals(_windDirection))
                {
                    return;
                }

                _windDirection = value;
                OnPropertyChanged();
            }
        }

        public string WindSpeed
        {
            get => _windSpeed;
            set
            {
                if (value.Equals(_windSpeed))
                {
                    return;
                }

                _windSpeed = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfTurbines
        {
            get => _numberOfTurbines;
            set
            {
                if (value == _numberOfTurbines)
                {
                    return;
                }

                _numberOfTurbines = value;
                OnPropertyChanged();
            }
        }

        public double TotalPower
        {
            get => _totalPower;
            set
            {
                if (value.Equals(_totalPower))
                {
                    return;
                }

                _totalPower = value;
                OnPropertyChanged();
            }
        }

        public string StudentId
        {
            get => _studentId;
            set
            {
                if (value.Equals(_studentId))
                {
                    return;
                }

                _studentId = value;
                ProcessStudentId();
                OnPropertyChanged();
            }
        }

        public int WornTurbine
        {
            get => _wornTurbine;
            set
            {
                if (value.Equals(_wornTurbine))
                {
                    return;
                }

                _wornTurbine = value;
                OnPropertyChanged();
            }
        }

        public string StudentIdErrors
        {
            get => _studentIdErrors;
            set
            {
                if (value.Equals(_studentIdErrors))
                {
                    return;
                }

                _studentIdErrors = value;
                OnPropertyChanged();
            }
        }

        public bool IsStudentIdValid
        {
            get => _isStudentIdValid;
            set
            {
                if (value.Equals(_isStudentIdValid))
                {
                    return;
                }

                _isStudentIdValid = value;
                OnPropertyChanged();
            }
        }

        public bool IsTelemetryRunning
        {
            get => _isTelemetryRunning;
            set
            {
                if (value.Equals(_isTelemetryRunning))
                {
                    return;
                }

                _isTelemetryRunning = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InitializeTurbineModels()
        {
            Turbines = new ObservableCollection<WindTurbine>();
            foreach (var windTurbineModel in _turbineModels)
            {
                var turbine = new WindTurbine();
                turbine.Update(windTurbineModel.ToDto());
                turbine.DeviceConnectionString =
                    ApplicationData.Current.LocalSettings.Values[$"device-connection-string-{turbine.Name}"]
                        ?.ToString();
                Turbines.Add(turbine);
            }

            //_turbines[0].DeviceConnectionString =
            //    "HostName=capstonehub.azure-devices.net;DeviceId=CWF-001;SharedAccessKey=RKfh8J136ZXx3o7D7rJGaU+zT9cxxjkkazodNAnpae4=";
        }

        private void ProcessStudentId()
        {
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
                    _studentIdNumeric = Convert.ToInt32(_studentId, 16);
                    IsStudentIdValid = true;
                    ApplicationData.Current.LocalSettings.Values["StudentId"] = _studentId;
                    _metadataDto.StudentId = _studentId;
                    _capturedRandom = new Random(_studentIdNumeric);
                    WornTurbine = _capturedRandom.Next(1, 11);
                    for (var index = 0; index < _turbineModels.Length; index++)
                    {
                        _turbineModels[index].LowPowerOutput = index == WornTurbine - 1;
                    }

                    _windDirectionWithVariance = new VarianceDelayedDouble(_capturedRandom.NextDouble() * 359)
                    {
                        StepDelay = TimeSpan.FromMilliseconds(1), ValueLag = TimeSpan.FromMilliseconds(1),
                        Variance = 3.3
                    };
                    _windSpeedWithVariance = new VarianceDelayedDouble(9.2)
                        {StepDelay = TimeSpan.FromMilliseconds(500), ValueLag = TimeSpan.FromSeconds(5), Variance = 1};
                }
                catch
                {
                    StudentIdErrors = "Student ID must be an 8 character Hex string";
                    IsStudentIdValid = false;
                }
            }
        }

        private async void TickTimerOnTick(object sender, object e)
        {
            try
            {
                await UpdateWindData();
            }
            catch (Exception exception)
            {
                StopTelemetry();
                var cd = new ContentDialog
                {
                    CloseButtonText = "Ok",
                    Title = "Error Sending Telemetry",
                    Content = $"Stopping Telemetry - Check the device connection strings.{Environment.NewLine}{exception.Message}"
                };
                await cd.ShowAsync();
            }
        }

        private async Task UpdateWindData()
        {
            var windSpeedChanged = false;
            if (_capturedRandom.NextDouble() > 0.97)
            {
                // change direction
                _windDirectionWithVariance.Value = _capturedRandom.NextDouble() * 359;
                _windSpeedWithVariance.Value = _capturedRandom.NextDouble() * 20;
                windSpeedChanged = true;
            }

            var windDirection = _windDirectionWithVariance.Value;
            if (windDirection > 359)
            {
                windDirection -= 360;
            }

            if (windDirection < 0)
            {
                windDirection += 360;
            }

            WindDirection = windDirection;

            var windSpeed = _windSpeedWithVariance.Value;
            if (windSpeed < 0)
            {
                windSpeed = 0;
            }

            WindSpeed = $"{windSpeed:N2} m/s";

            for (var index = 0; index < _turbineModels.Length; index++)
            {
                var windTurbineModel = _turbineModels[index];
                if (windSpeedChanged)
                {
                    windTurbineModel.WindSpeed = windSpeed;
                }

                _turbines[index].Update(windTurbineModel.ToDto());
                await _turbines[index].SendTelemetry(_metadataDto);
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

                // update the turbine wind speeds
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