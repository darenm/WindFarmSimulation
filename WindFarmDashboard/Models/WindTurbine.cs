using System.ComponentModel;
using System.Runtime.CompilerServices;
using Simulator.Library.Dtos;
using WindFarmDashboard.Annotations;

namespace WindFarmDashboard.Models
{
    public class WindTurbine : INotifyPropertyChanged
    {
        private double _externalTemperatureCelsius;
        private double _generatorTemperatureCelsius;
        private double _highSpeedShaftRpm;
        private bool _isTurbineBrakeOn;
        private double _lowSpeedShaftRpm;
        private double _power;
        private double _rotorTemperatureCelsius;
        private bool _triggerGeneratorOverTemp;
        private bool _triggerRotorOverTemp;
        private double _windSpeed;
        private string _name;

        public double WindSpeed
        {
            get => _windSpeed;
            set
            {
                if (value.Equals(_windSpeed)) return;
                _windSpeed = value;
                OnPropertyChanged();
            }
        }

        public double LowSpeedShaftRpm
        {
            get => _lowSpeedShaftRpm;
            set
            {
                if (value.Equals(_lowSpeedShaftRpm)) return;
                _lowSpeedShaftRpm = value;
                OnPropertyChanged();
            }
        }

        public double HighSpeedShaftRpm
        {
            get => _highSpeedShaftRpm;
            set
            {
                if (value.Equals(_highSpeedShaftRpm)) return;
                _highSpeedShaftRpm = value;
                OnPropertyChanged();
            }
        }

        public bool TriggerGeneratorOverTemp
        {
            get => _triggerGeneratorOverTemp;
            set
            {
                if (value == _triggerGeneratorOverTemp) return;
                _triggerGeneratorOverTemp = value;
                OnPropertyChanged();
            }
        }

        public double ExternalTemperatureCelsius
        {
            get => _externalTemperatureCelsius;
            set
            {
                if (value.Equals(_externalTemperatureCelsius)) return;
                _externalTemperatureCelsius = value;
                OnPropertyChanged();
            }
        }

        public double GeneratorTemperatureCelsius
        {
            get => _generatorTemperatureCelsius;
            set
            {
                if (value.Equals(_generatorTemperatureCelsius)) return;
                _generatorTemperatureCelsius = value;
                OnPropertyChanged();
            }
        }

        public bool TriggerRotorOverTemp
        {
            get => _triggerRotorOverTemp;
            set
            {
                if (value == _triggerRotorOverTemp) return;
                _triggerRotorOverTemp = value;
                OnPropertyChanged();
            }
        }

        public double RotorTemperatureCelsius
        {
            get => _rotorTemperatureCelsius;
            set
            {
                if (value.Equals(_rotorTemperatureCelsius)) return;
                _rotorTemperatureCelsius = value;
                OnPropertyChanged();
            }
        }

        public double Power
        {
            get => _power;
            set
            {
                if (value.Equals(_power)) return;
                _power = value;
                OnPropertyChanged();
            }
        }

        public bool IsTurbineBrakeOn
        {
            get => _isTurbineBrakeOn;
            set
            {
                if (value == _isTurbineBrakeOn) return;
                _isTurbineBrakeOn = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }


        public void Update(WindTurbineDto dto)
        {
            WindSpeed = dto.WindSpeed;
            ExternalTemperatureCelsius = dto.ExternalTemperatureCelsius;
            GeneratorTemperatureCelsius = dto.GeneratorTemperatureCelsius;
            HighSpeedShaftRpm = dto.HighSpeedShaftRpm;
            IsTurbineBrakeOn = dto.IsTurbineBrakeOn;
            LowSpeedShaftRpm = dto.LowSpeedShaftRpm;
            Power = dto.Power;
            RotorTemperatureCelsius = dto.RotorTemperatureCelsius;
            Name = dto.Name;
        }
    }
}