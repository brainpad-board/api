using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;
using static GHIElectronics.TinyCLR.Drivers.BrainPadController.BrainPad;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Buttons : IOModule {
        private GpioPin gpioPin;

        static GpioController controller = GpioController.GetDefault();
        private bool wasPressed;
        private double detectPeriod = 0;
        private DateTime lastPressed;
        private bool WasPressed {
            get {
                if (this.wasPressed) {
                    this.wasPressed = false;
                    var diff = (DateTime.Now - this.lastPressed).TotalMilliseconds;

                    if (diff <= this.detectPeriod)
                        return true;
                    else
                        return false;
                }

                return false;
            }

            set => this.wasPressed = value == false ? false : true;
        }

        public Buttons(string button, double detectPeriod) {
            var pinNum = -1;

            button = button.ToLower();

            switch (button) {
                case "a":
                    pinNum = (int)Button.A;
                    break;
                case "b":
                    pinNum = (int)Button.B;
                    break;
            }

            if (pinNum == -1) {
                pinNum = BrainPad.GetGpioFromString(button);
            }

            this.Initialize(pinNum, detectPeriod);
        }

        private void Initialize(int pinNum, double detectPeriod) {

            if (pinNum < 0) {
                throw new ArgumentException("Invalid button.");
            }

            BrainPad.UnRegisterObject(pinNum);

            this.detectPeriod = detectPeriod * 1000;

            this.gpioPin = controller.OpenPin(pinNum);

            this.gpioPin.SetDriveMode(GpioPinDriveMode.InputPullUp);

            this.gpioPin.ValueChangedEdge = GpioPinEdge.FallingEdge;

            this.gpioPin.ValueChanged += this.Btn_ValueChanged;

            BrainPad.RegisterObject(this, pinNum);

            this.lastPressed = DateTime.Now;
        }

        private void Btn_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e) {
            this.wasPressed = true;

            this.lastPressed = DateTime.Now;
        }

        public override double In() {
            if (this.gpioPin.Read() == GpioPinValue.Low || this.WasPressed)
                return 1;

            return 0;
        }

        public override void Dispose(bool disposing) {
            if (disposing)
                this.gpioPin?.Dispose(); ;

            this.gpioPin = null;
        }

    }
}
