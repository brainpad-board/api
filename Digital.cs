using System;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    public class Digital : IOModule {

        private GpioPin gpioPin;       

        public Digital(string bpPin) {
            var pinNum = -1;

            bpPin = bpPin.ToLower();

            if (bpPin.CompareTo(Controller.BUILTIN_TEXT_LED) == 0) {
                pinNum = SC13048.GpioPin.PA8;
            }

            this.Initialize(pinNum);
        }
        public Digital(double bpPin) {
            var pinNum = Controller.GetGpioFromPin(bpPin);

            this.Initialize(pinNum);
        }

        private void Initialize(int pinNum) {
            if (pinNum < 0) {
                throw new ArgumentException("Invalid pin number.");
            }

            this.gpioPin = Controller.Gpio.OpenPin(pinNum);

        }

        public override double In() {
            this.gpioPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            return this.gpioPin.Read() == GpioPinValue.High ? 1 : 0;
        }

        public override void Out(double oValue) {
            this.gpioPin.SetDriveMode(GpioPinDriveMode.Output);

            this.gpioPin.Write(oValue > 0 ? GpioPinValue.High : GpioPinValue.Low);
        }


        public override void Dispose() {
            this.gpioPin?.Dispose();

            this.gpioPin = null;

        }

    }
}
