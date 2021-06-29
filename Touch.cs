using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.Touch.CapacitiveTouch;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Touch : IOModule {

        private GpioPin gpioPin;

        private CapacitiveTouchController touch;

        public Touch(string pinBp, int senstitiveLevel) {
            var pinNum = BrainPad.GetGpioFromString(pinBp);

            this.gpioPin = BrainPad.Gpio.OpenPin(pinNum);

            this.touch = new CapacitiveTouchController(this.gpioPin, senstitiveLevel);
        }

        public override double In() => this.touch.IsTouched ? 1 : 0;
        public override void Dispose() {
            this.gpioPin?.Dispose();

            this.gpioPin = null;
        }
    }
}
