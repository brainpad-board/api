using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Signals;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class DistanceSensor : IOModule {

        const int registerNum = 0xA2;

        PulseFeedback pulseFeedback;
        GpioPin distanceTrigger;
        GpioPin distanceEcho;

        static GpioController Controller = GpioController.GetDefault();

        public DistanceSensor(BrainPad.Pin trigger, BrainPad.Pin echo) {
            var triggerPin = BrainPad.GetGpioFromBpPin(trigger);
            var echoPin = BrainPad.GetGpioFromBpPin(echo);

            this.Initialize(triggerPin, echoPin);
        }

        public DistanceSensor(string trigger, string echo) {
            var triggerPin = BrainPad.GetGpioFromBpPin(trigger);
            var echoPin = BrainPad.GetGpioFromBpPin(echo);

            this.Initialize(triggerPin, echoPin);

        }

        void Initialize(int triggerPin, int echoPin) {

            if (triggerPin < 0 || echoPin < 0) {
                throw new ArgumentException("trigger or echo pin invalid.");
            }

            BrainPad.UnRegisterObject(registerNum);

            this.distanceTrigger = Controller.OpenPin(triggerPin);
            this.distanceEcho = Controller.OpenPin(echoPin);

            this.pulseFeedback = new PulseFeedback(this.distanceTrigger, this.distanceEcho, PulseFeedbackMode.EchoDuration) {
                DisableInterrupts = false,
                Timeout = TimeSpan.FromSeconds(1),
                PulseLength = TimeSpan.FromTicks(100),
                PulseValue = GpioPinValue.High,
                EchoValue = GpioPinValue.High,
            };

            BrainPad.RegisterObject(this, registerNum);
        }

        public override double In() {
            var time = this.pulseFeedback.Trigger();
            var microsecond = time.TotalMilliseconds * 1000.0;

            var distance = microsecond * 0.036 / 2;

            return (double)distance;
        }

        public override void Dispose(bool disposing) {
            if (disposing) {
                this.distanceTrigger?.Dispose();
                this.distanceEcho?.Dispose();
            }

            this.distanceTrigger = null;
            this.distanceEcho = null;
        }
    }
}
