using GHIElectronics.TinyCLR.Devices.Gpio;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class LineSensor : IOModule {

        private GpioPin gpioPin;

        static GpioController Controller = GpioController.GetDefault();        
        public LineSensor(BrainPad.Pin bpPin) {
            var pinNum = BrainPad.GetGpioFromBpPin(bpPin);
            this.Initialize(pinNum);

        }

        public LineSensor(string bpPin) {
            var pinNum = BrainPad.GetGpioFromBpPin(bpPin);

            this.Initialize(pinNum);
        }

        private void Initialize(int pinNum) {
            if (pinNum < 0) {
                throw new ArgumentException("Invalid pin number.");
            }


            BrainPad.UnRegisterObject(pinNum);

            this.gpioPin = Controller.OpenPin(pinNum);
            this.gpioPin.SetDriveMode(GpioPinDriveMode.Input);

            BrainPad.RegisterObject(this, pinNum);

        }

        public override double In() => this.gpioPin.Read() == GpioPinValue.High ? 1 : 0;

        public override void Dispose(bool disposing) {
            if (disposing)
                this.gpioPin?.Dispose();

            this.gpioPin = null;
        }
    }
}
