using System;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Signals;

namespace BrainPad {
    public class Distance : IOModule {
        PulseFeedback pulseFeedback;
        GpioPin distanceTrigger;
        GpioPin distanceEcho;
        public Distance(double trigger, double echo) {
            var triggerPin = Controller.GetGpioFromPin(trigger);
            var echoPin = Controller.GetGpioFromPin(echo);

            this.Initialize(triggerPin, echoPin);
        }

        void Initialize(int triggerPin, int echoPin) {
            if (triggerPin < 0 || echoPin < 0) {
                throw new ArgumentException("trigger or echo pin invalid.");
            }

            this.distanceTrigger = Controller.Gpio.OpenPin(triggerPin);
            this.distanceEcho = Controller.Gpio.OpenPin(echoPin);

            this.pulseFeedback = new PulseFeedback(this.distanceTrigger, this.distanceEcho, PulseFeedbackMode.EchoDuration) {
                DisableInterrupts = false,
                Timeout = TimeSpan.FromSeconds(1),
                PulseLength = TimeSpan.FromTicks(100),
                PulseValue = GpioPinValue.High,
                EchoValue = GpioPinValue.High,
            };
        }

        public override double In() {
            var time = this.pulseFeedback.Trigger();
            var microsecond = time.TotalMilliseconds * 1000.0;

            var distance = microsecond * 0.036 / 2;

            return (double)distance;
        }

        public override void Dispose() {
            this.distanceTrigger?.Dispose();
            this.distanceEcho?.Dispose();

            this.distanceTrigger = null;
            this.distanceEcho = null;
        }
    }
}
