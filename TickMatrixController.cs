using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Drivers.BasicGraphics;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    internal class TickMatrixController : GHIElectronics.TinyCLR.Drivers.BasicGraphics.BasicGraphics {
        private PwmChannel brightnessChannel;

        private uint white = ColorFromRgb(0xff, 0xff, 0xff);
        public void SetBrightness(double brightness) => this.brightnessChannel.SetActiveDutyCyclePercentage(brightness);
        public void DrawText(string text) {
            if (text.Length == 1) {
                this.DrawTinyCharacter(text[0], this.white, 0, 0, true);
                Thread.Sleep(80);
            }
            else {
                var len = text.Length * 6;
                for (var x = 5; x > -len; x--) {
                    this.DrawTinyString(text, this.white, x, 0, true);
                    Thread.Sleep(80);
                }
            }
        }

        public override void Clear() {
            foreach (var pin in this.ledMatrix) {
                pin.Write(GpioPinValue.Low);
            }
        }


        public override void SetPixel(int x, int y, uint color) {
            if (x < 0 || x > 4) return;
            if (y < 0 || y > 4) return;
            var index = (y * 5) + x;
            if (color != 0)
                this.ledMatrix[index].Write(GpioPinValue.High);
            else
                this.ledMatrix[index].Write(GpioPinValue.Low);
        }
        GpioPin[] ledMatrix;
        public TickMatrixController() {
            var gpio = GpioController.GetDefault();

            var pwmController1 = PwmController.FromName(SC13048.Timer.Pwm.Controller1.Id);
            this.brightnessChannel = pwmController1.OpenChannel(SC13048.Timer.Pwm.Controller1.PA9);
            this.brightnessChannel.Controller.SetDesiredFrequency(1000);
            this.brightnessChannel.SetActiveDutyCyclePercentage(0.5);
            this.brightnessChannel.Start();

            this.ledMatrix = new GpioPin[]
            {
               gpio.OpenPin(SC13048.GpioPin.PB14),//D1
               gpio.OpenPin(SC13048.GpioPin.PA10),
               gpio.OpenPin(SC13048.GpioPin.PA14),
               gpio.OpenPin(SC13048.GpioPin.PA15),
               gpio.OpenPin(SC13048.GpioPin.PA8),

               gpio.OpenPin(SC13048.GpioPin.PB13),//D6
               gpio.OpenPin(SC13048.GpioPin.PB15),
               gpio.OpenPin(SC13048.GpioPin.PB6),
               gpio.OpenPin(SC13048.GpioPin.PA13),
               gpio.OpenPin(SC13048.GpioPin.PC14),

               gpio.OpenPin(SC13048.GpioPin.PB12),//D11
               gpio.OpenPin(SC13048.GpioPin.PB2),
               gpio.OpenPin(SC13048.GpioPin.PH3),
               gpio.OpenPin(SC13048.GpioPin.PC15),
               gpio.OpenPin(SC13048.GpioPin.PB8),

               gpio.OpenPin(SC13048.GpioPin.PB1),//D16
               gpio.OpenPin(SC13048.GpioPin.PB0),
               gpio.OpenPin(SC13048.GpioPin.PA4),
               gpio.OpenPin(SC13048.GpioPin.PH1),
               gpio.OpenPin(SC13048.GpioPin.PB9),

               gpio.OpenPin(SC13048.GpioPin.PA7),//D21
               gpio.OpenPin(SC13048.GpioPin.PA6),
               gpio.OpenPin(SC13048.GpioPin.PA1),
               gpio.OpenPin(SC13048.GpioPin.PA0),
               gpio.OpenPin(SC13048.GpioPin.PH0),
            };
            /*OLDledMatrix = new GpioPin[]
            {
               gpio.OpenPin(SC13048.GpioPin.PA8),//D1
               gpio.OpenPin(SC13048.GpioPin.PA10),
               gpio.OpenPin(SC13048.GpioPin.PA14),
               gpio.OpenPin(SC13048.GpioPin.PA15),
               gpio.OpenPin(SC13048.GpioPin.PA13),

               gpio.OpenPin(SC13048.GpioPin.PA9),//D6
               gpio.OpenPin(SC13048.GpioPin.PB15),
               gpio.OpenPin(SC13048.GpioPin.PB6),
               gpio.OpenPin(SC13048.GpioPin.PH1),
               gpio.OpenPin(SC13048.GpioPin.PH0),

               gpio.OpenPin(SC13048.GpioPin.PB14),//D11
               gpio.OpenPin(SC13048.GpioPin.PB13),
               gpio.OpenPin(SC13048.GpioPin.PA6),
               gpio.OpenPin(SC13048.GpioPin.PC15),
               gpio.OpenPin(SC13048.GpioPin.PC14),

               gpio.OpenPin(SC13048.GpioPin.PB12),//D16
               gpio.OpenPin(SC13048.GpioPin.PB10),
               gpio.OpenPin(SC13048.GpioPin.PB1),
               gpio.OpenPin(SC13048.GpioPin.PA1),
               gpio.OpenPin(SC13048.GpioPin.PA0),

               gpio.OpenPin(SC13048.GpioPin.PB11),//D21
               gpio.OpenPin(SC13048.GpioPin.PB2),
               gpio.OpenPin(SC13048.GpioPin.PB0),
               gpio.OpenPin(SC13048.GpioPin.PA7),
               gpio.OpenPin(SC13048.GpioPin.PA4),
            };*/
            foreach (var pin in this.ledMatrix) {
                pin.SetDriveMode(GpioPinDriveMode.Output);
                pin.Write(GpioPinValue.Low);
            }

        }

        bool disposed = false;
        internal void Dispose() {
            if (this.disposed)
                return;

            this.disposed = true;

            this.brightnessChannel.Dispose();

            foreach (var pin in this.ledMatrix) {
                pin.Dispose();
            }
        }
    }
}
