using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Signals;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class DistanceSensor : IOModule {
#if !DEBUG
        PulseFeedback pulseFeedback;
        GpioPin distanceTrigger;
        GpioPin distanceEcho;
#endif
        public DistanceSensor(string trigger, string echo) {
#if !DEBUG
            var triggerPin = BrainPad.GetGpioFromString(trigger);
            var echoPin = BrainPad.GetGpioFromString(echo);

            this.Initialize(triggerPin, echoPin);
#endif
        }

        void Initialize(int triggerPin, int echoPin) {
#if !DEBUG
            if (triggerPin < 0 || echoPin < 0) {
                throw new ArgumentException("trigger or echo pin invalid.");
            }            

            this.distanceTrigger = BrainPad.Gpio.OpenPin(triggerPin);
            this.distanceEcho = BrainPad.Gpio.OpenPin(echoPin);

            this.pulseFeedback = new PulseFeedback(this.distanceTrigger, this.distanceEcho, PulseFeedbackMode.EchoDuration) {
                DisableInterrupts = false,
                Timeout = TimeSpan.FromSeconds(1),
                PulseLength = TimeSpan.FromTicks(100),
                PulseValue = GpioPinValue.High,
                EchoValue = GpioPinValue.High,
            };
#endif            
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "<Pending>")]
        public override double In() {
#if !DEBUG
            var time = this.pulseFeedback.Trigger();
            var microsecond = time.TotalMilliseconds * 1000.0;

            var distance = microsecond * 0.036 / 2;

            return (double)distance;
#else
            return 0;
#endif
        }

        public override void Dispose() {
#if !DEBUG
            this.distanceTrigger?.Dispose();
            this.distanceEcho?.Dispose();

            this.distanceTrigger = null;
            this.distanceEcho = null;
#endif
        }
    }
}
