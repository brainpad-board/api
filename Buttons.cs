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

        static GpioController Controller = GpioController.GetDefault();
        private bool wasPressed;
        private double expireTime = 0;
        private DateTime lastPressed;
        private bool WasPressed {
            get {
                if (this.wasPressed) {
                    this.wasPressed = false;
                    var diff = (DateTime.Now - this.lastPressed).TotalMilliseconds;

                    if (diff <= this.expireTime)
                        return true;
                    else
                        return false;
                }

                return false;
            }

            set {
                this.wasPressed = value == false ? false : true;
            }
        }

        public Buttons(Button button, object expireTime) {
            this.Initialize((int)button, expireTime);
        }

        public Buttons(BrainPad.Pin pinBp, object expireTime) {
            var pinNum = BrainPad.GetGpioFromBpPin(pinBp);

            this.Initialize(pinNum, expireTime);
        }

        public Buttons(string button, object expireTime) {
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
                pinNum = BrainPad.GetGpioFromBpPin(button);
            }

            this.Initialize(pinNum, expireTime);
        }

        private void Initialize(int pinNum, object expireTime) {

            if (pinNum < 0) {
                throw new ArgumentException("Invalid button.");
            }

            BrainPad.UnRegisterObject(pinNum);

            if (expireTime is double d) {
                this.expireTime = d * 1000;
            }
            else if (expireTime is int i) {
                this.expireTime = i * 1000;
            }

            this.gpioPin = Controller.OpenPin(pinNum);

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
