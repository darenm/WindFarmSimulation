﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator.Library.Utilities
{
    public class DelayedDouble
    {
        private double _actualValue;
        private CancellationTokenSource _cancellationTokenSource;
        private double _targetValue;

        public DelayedDouble(double actualValue)
        {
            _actualValue = actualValue;
        }

        public TimeSpan ValueLag { get; set; } = TimeSpan.FromMilliseconds(3000);
        public TimeSpan StepDelay { get; set; } = TimeSpan.FromMilliseconds(100);

        public virtual double Value
        {
            get => _actualValue;
            set
            {
                _targetValue = value;

                if (Math.Abs(_targetValue - _actualValue) < 0.001) return;

                _cancellationTokenSource?.Cancel();

                // Don't wait for it
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        var stepCount = ValueLag.Ticks / StepDelay.Ticks;
                        var valueStep = (_targetValue - _actualValue) / stepCount;
                        if (Math.Abs(valueStep) < 0.01)
                        {
                            valueStep = Math.Sign(valueStep) * 0.01;
                        }

                        while (stepCount-- > 0)
                        {
                            if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();
                            await Task.Delay(StepDelay);
                            _actualValue += valueStep;

                            if ((valueStep < 0 && _actualValue < _targetValue) ||
                                (valueStep > 0 && _actualValue > _targetValue))
                            {
                                _actualValue = _targetValue;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                    finally
                    {
                        _cancellationTokenSource = null;
                    }
                }, token);
            }
        }
    }
}