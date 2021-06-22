using GHIElectronics.TinyCLR.Devices.Gpio;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Digital : IOModule {

        private GpioPin gpioPin;

        public GpioPinDriveMode DriverMode { get; set; } = GpioPinDriveMode.Input;

        public Digital(string bpPin, string driverMode) {
            var pinNum = BrainPad.GetGpioFromString(bpPin);

            driverMode = driverMode.ToLower();

            if (driverMode.IndexOf(BrainPad.TEXT_PULLUP) >= 0) {
                this.DriverMode = GpioPinDriveMode.InputPullUp;
            }
            else if (driverMode.IndexOf(BrainPad.TEXT_PULLDOWN) >= 0) {
                this.DriverMode = GpioPinDriveMode.InputPullDown;
            }
            else
                this.DriverMode = GpioPinDriveMode.Input;

            this.Initialize(pinNum);
        }

        private void Initialize(int pinNum) {
            if (pinNum < 0) {
                throw new ArgumentException("Invalid pin number.");
            }            

            this.gpioPin = BrainPad.Gpio.OpenPin(pinNum);            

        }

        public override double In() {

            this.gpioPin.SetDriveMode(this.DriverMode);
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
