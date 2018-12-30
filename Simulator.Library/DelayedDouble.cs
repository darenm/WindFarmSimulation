using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator.Library
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
        public int NumberOfSteps { get; set; } = 10;

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
                Task.Factory.StartNew(async () =>
                {
                    var captureToken = _cancellationTokenSource.Token;
                    var stepDelayTicks = ValueLag.Ticks / NumberOfSteps;
                    var stepDelay = TimeSpan.FromTicks(stepDelayTicks);
                    var stepCount = NumberOfSteps;
                    var valueStep = (_targetValue - _actualValue) / NumberOfSteps;

                    while (stepCount-- > 0)
                    {
                        if (captureToken.IsCancellationRequested) break;
                        await Task.Delay(stepDelay, captureToken);
                        _actualValue += valueStep;
                    }

                    _cancellationTokenSource = null;
                }, _cancellationTokenSource.Token);
            }
        }
    }
}