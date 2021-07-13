using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    internal class TickMatrixController : GHIElectronics.TinyCLR.Drivers.BasicGraphics.BasicGraphics {
        private PwmChannel brightnessChannel;

        private readonly uint white = ColorFromRgb(0xff, 0xff, 0xff);

        private byte[] buffer;
        public void SetBrightness(double brightness) => this.brightnessChannel.SetActiveDutyCyclePercentage(brightness / 100.0);
        public void DrawText(string text) {
            if (text.Length == 1) {
                this.DrawTinyCharacter(text[0], this.white, 0, 0, true);
            }
            else {
                var len = text.Length * 6;
                for (var x = 5; x > -len; x--) {
                    this.DrawTinyString(text, this.white, x, 0, true);
                    this.Show();
                    Thread.Sleep(80);
                }
            }
        }

        public override void Clear() =>    
            Array.Clear(this.buffer, 0, this.buffer.Length);


        public override void SetPixel(int x, int y, uint color) {
            if (x < 0 || x > 4) return;
            if (y < 0 || y > 4) return;
            
            this.buffer[(y * 5) + x] = (byte)(color & 0xFF);
        }

        public void Show() {
            for (var index = 0; index < this.buffer.Length; index++) {
                this.ledMatrix[index].Write(this.buffer[index] != 0 ? GpioPinValue.High : GpioPinValue.Low); 
            }
        }

        GpioPin[] ledMatrix;
        public TickMatrixController() : base() {
            var gpio = Controller.Gpio;
            var pwmController1 = PwmController.FromName(SC13048.Timer.Pwm.Controller1.Id);            

            this.brightnessChannel = pwmController1.OpenChannel(SC13048.Timer.Pwm.Controller1.PA9);
            this.brightnessChannel.Controller.SetDesiredFrequency(1000);
            this.brightnessChannel.SetActiveDutyCyclePercentage(0.5);
            this.brightnessChannel.Start();

            this.buffer = new byte[5 * 5];

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

            foreach (var pin in this.ledMatrix) {
                pin.SetDriveMode(GpioPinDriveMode.Output);
                pin.Write(GpioPinValue.Low);
            }

        }

        internal void Dispose() {
            this.brightnessChannel?.Dispose();

            this.brightnessChannel = null;

            for (var i = 0; i < this.ledMatrix.Length; i++) {
                this.ledMatrix[i]?.Dispose();
                this.ledMatrix[i] = null;
            }
        }
    }
}
