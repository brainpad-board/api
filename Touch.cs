using System;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Signals;

namespace BrainPad {
    public class Touch : IOModule {

        private GpioPin gpioPin;

        private CapacitiveTouchController touch;

        public Touch(string pinBp, int senstitiveLevel) {
            var pinNum = Controller.GetGpioFromString(pinBp);

            this.gpioPin = Controller.Gpio.OpenPin(pinNum);

            this.touch = new CapacitiveTouchController(this.gpioPin, senstitiveLevel);
        }

        public override double In() => this.touch.IsTouched ? 1 : 0;
        public override void Dispose() {
            this.gpioPin?.Dispose();

            this.gpioPin = null;
        }
    }

    internal class CapacitiveTouchController {
        private int level;
        private PulseFeedback pulseFeedback;
        const double CalibrateMinValue = 0.008;
        const double CalibrateMaxValue = 0.015;

        /// <summary>
        /// Capacitive Touch constructor
        /// </summary>
        /// <param name="touchPin"> Gpio pin</param>
        /// <param name="sensitiveLevel">Sensitive level [0..100].</param>
        public CapacitiveTouchController(GpioPin touchPin, int sensitiveLevel) {
            if (sensitiveLevel < 0 || sensitiveLevel > 100)
                throw new ArgumentException("Level must be in range [0,100]");

            this.level = 100 - sensitiveLevel;

            this.pulseFeedback = new PulseFeedback(touchPin,
                PulseFeedbackMode.DrainDuration) {

                DisableInterrupts = false,
                Timeout = TimeSpan.FromSeconds(1),
                PulseLength = TimeSpan.FromTicks(10000),
                PulseValue = GpioPinValue.High,
            };

        }
        public bool IsTouched {
            get {
                var scale = Controller.Scale(this.pulseFeedback.Trigger().TotalMilliseconds * 10000, (int)(CalibrateMinValue * 10000), (int)(CalibrateMaxValue * 10000), 0, 100);

                if (scale >= this.level)
                    return true;

                else return false;
            }

        }


    }
}
