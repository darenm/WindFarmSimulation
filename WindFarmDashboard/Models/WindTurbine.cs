﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Simulator.Library.Dtos;
using Simulator.Library.Utilities;
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
        private WindTurbineDto _dto;


        private DeviceClient _deviceClient;
        private bool _ioTHubConnected;

        // Select one of the following transports used by DeviceClient to connect to IoT Hub.
        //private static TransportType _transportType = TransportType.Amqp;
        //private static TransportType _transportType = TransportType.Mqtt;
        private static TransportType _transportType = TransportType.Http1;
        //private static TransportType _transportType = TransportType.Amqp_WebSocket_Only;
        //private static TransportType _transportType = TransportType.Mqtt_WebSocket_Only;

        public string DeviceConnectionString { get; set; }

        public bool IoTHubConnected
        {
            get => _ioTHubConnected;
            set
            {
                if (value == _ioTHubConnected) return;
                _ioTHubConnected = value;
                OnPropertyChanged();
            }
        }

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
            _dto = dto;
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

        public async Task SendTelemetry(MetadataDto metadataDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DeviceConnectionString))
                {
                    IoTHubConnected = false;
                    return;
                }

                var deviceMessage = new DeviceMessage
                {
                    Metadata = new MetadataDto {DeviceType = metadataDto.DeviceType, StudentId = metadataDto.StudentId}
                };

                var key = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                var studentId = Encoding.ASCII.GetBytes(deviceMessage.Metadata.StudentId);
                for (var i = 0; i < studentId.Length; i++) key[i] = studentId[i];

                var checkSumString = $"{_dto.Name}-{_dto.WindSpeed:N2}-{_dto.Power:N2}";
                var sipHash = new SipHash(key);
                var messageBytes = Encoding.ASCII.GetBytes(checkSumString);
                var result = sipHash.Compute(messageBytes);
                var resultInHex = $"{result:X}";
                deviceMessage.Metadata.Uid = resultInHex;

                if (_deviceClient == null)
                {
                    _deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, _transportType);
                    IoTHubConnected = true;
                    await _deviceClient.OpenAsync();
                }

                deviceMessage.Telemetry = _dto;

                var json = JsonConvert.SerializeObject(deviceMessage);
                var eventMessage = new Message(Encoding.UTF8.GetBytes(json));
                eventMessage.Properties.Add("generatorTemperatureAlert", (_dto.GeneratorTemperatureCelsius > 60.0) ? "true" : "false");
                eventMessage.Properties.Add("rotorTemperatureAlert", (_dto.RotorTemperatureCelsius > 35.0) ? "true" : "false");

                await _deviceClient.SendEventAsync(eventMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}