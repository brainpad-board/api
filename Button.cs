using System;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    public class Button : IOModule {
        private GpioPin gpioPin;

        private bool wasPressed;
        private double detectPeriod = 0;
        private DateTime lastPressed;
        private bool WasPressed {
            get {
                if (this.wasPressed) {
                    this.wasPressed = false;
                    var diff = (DateTime.Now - this.lastPressed).TotalMilliseconds;
                    return diff <= this.detectPeriod;
                }

                return false;
            }

            set => this.wasPressed = value;
        }

        public Button(double button, double detectPeriod) {
            var pinNum = Controller.GetGpioFromPin(button);

            this.Initialize(pinNum, detectPeriod);
        }
        public Button(string button, double detectPeriod) {
            var pinNum = -1;

            button = button.ToLower();

            switch (button) {
                case "a":
                    pinNum = SC13048.GpioPin.PC13;
                    break;
                case "b":
                    pinNum = SC13048.GpioPin.PB7;
                    break;
            }           

            this.Initialize(pinNum, detectPeriod);
        }

        private void Initialize(int pinNum, double detectPeriod) {

            if (pinNum < 0) {
                throw new ArgumentException("Invalid button.");
            }

            this.detectPeriod = detectPeriod * 1000;

            this.gpioPin = Controller.Gpio.OpenPin(pinNum);

            this.gpioPin.SetDriveMode(GpioPinDriveMode.InputPullUp);

            this.gpioPin.ValueChangedEdge = GpioPinEdge.FallingEdge;

            this.gpioPin.ValueChanged += this.Btn_ValueChanged;

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

        public override void Dispose() {
            this.gpioPin?.Dispose();

            this.gpioPin = null;
        }

    }
}
