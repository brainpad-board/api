using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Signals;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class DistanceSensor : IOModule {

        const int RegisterNum = 0xA2;

        PulseFeedback pulseFeedback;
        GpioPin distanceTrigger;
        GpioPin distanceEcho;

        static GpioController controller = GpioController.GetDefault();

        public DistanceSensor(string trigger, string echo) {
            var triggerPin = BrainPad.GetGpioFromString(trigger);
            var echoPin = BrainPad.GetGpioFromString(echo);

            this.Initialize(triggerPin, echoPin);

        }

        void Initialize(int triggerPin, int echoPin) {

            if (triggerPin < 0 || echoPin < 0) {
                throw new ArgumentException("trigger or echo pin invalid.");
            }

            BrainPad.UnRegisterObject(RegisterNum);

            this.distanceTrigger = controller.OpenPin(triggerPin);
            this.distanceEcho = controller.OpenPin(echoPin);

            this.pulseFeedback = new PulseFeedback(this.distanceTrigger, this.distanceEcho, PulseFeedbackMode.EchoDuration) {
                DisableInterrupts = false,
                Timeout = TimeSpan.FromSeconds(1),
                PulseLength = TimeSpan.FromTicks(100),
                PulseValue = GpioPinValue.High,
                EchoValue = GpioPinValue.High,
            };

            BrainPad.RegisterObject(this, RegisterNum);
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
