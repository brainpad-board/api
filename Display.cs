using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Drivers.BasicGraphics;
using GHIElectronics.TinyCLR.Drivers.SolomonSystech.SSD1306;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Display {
        

        private GpioPin lcdReset;
        private SSD1306Controller pulseLcd;
        private GHIElectronics.TinyCLR.Drivers.BasicGraphics.BasicGraphics pulseGfx;
        private TickMatrixController tickGfx;
        private string[] messages = new string[8];
        private I2cDevice i2cDevice;
        public Display() {
            BrainPad.UnRegisterObject(BrainPad.DISPLAY_REGISTER_ID);

            if (BrainPad.Type.IsPulse == false) {
                this.tickGfx = new TickMatrixController();
            }
            else {
                this.InitPulseDisplay();
                this.pulseGfx = new GHIElectronics.TinyCLR.Drivers.BasicGraphics.BasicGraphics(128, 64, ColorFormat.OneBpp);
                for (var i = 0; i < 8; i++)
                    this.messages[i] = "";
            }

            BrainPad.RegisterObject(this, BrainPad.DISPLAY_REGISTER_ID);

            this.Clear();
        }

        public void SetBrightness(double brightness) {
            if (BrainPad.Type.IsPulse == false) {
                this.tickGfx.SetBrightness(brightness);
            }
            else {

            }
        }

        private void InitPulseDisplay() {
            this.lcdReset = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB2);
            this.lcdReset.SetDriveMode(GpioPinDriveMode.Output);
            this.lcdReset.Write(GpioPinValue.Low);
            Thread.Sleep(50);
            this.lcdReset.Write(GpioPinValue.High);

            var i2c = I2cController.FromName(SC13048.I2cBus.I2c2);

            this.i2cDevice = i2c.GetDevice(SSD1306Controller.GetConnectionSettings());

            this.pulseLcd = new SSD1306Controller(this.i2cDevice);
        }

        public void Print(string s) {
            this.Clear();
            if (BrainPad.Type.IsPulse == false) {
                this.tickGfx.DrawText(s);
            }
            else {
                for (var i = 0; i < 7; i++)
                    this.messages[i] = this.messages[i + 1];
                this.messages[7] = s;
                for (var i = 0; i < 8; i++)
                    this.pulseGfx.DrawString(this.messages[i], 1, 0, i * 8);
            }
            this.Show();
        }
        public void Show() {
            if (BrainPad.Type.IsPulse == false) {
                // nothing!
            }
            else {
                this.pulseLcd.DrawBufferNative(this.pulseGfx.Buffer);
            }
        }
        public void Clear() {
            if (BrainPad.Type.IsPulse == false) {
                this.tickGfx.Clear();
            }
            else {
                this.pulseGfx.Clear();

                this.Show();

            }
        }
        public void Circle(int x, int y, int r) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawCircle(1, x, y, r);
        }
        public void Line(int x1, int y1, int x2, int y2) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawLine(1, x1, y1, x2, y2);
        }
        public void Rect(int x, int y, int w, int h) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawRectangle(1, x, y, w, h);
        }
        public void Point(int x, int y) {
            if (BrainPad.Type.IsPulse)
                this.pulseGfx.SetPixel(x, y, 1);
            else {
                this.tickGfx.SetPixel(x, y, 1);
            }
        }
        public void Text(string s, int x, int y) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawString(s, 1, x, y);
        }
        public void TextEx(string s, int x, int y, int xs, int ys) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawString(s, 1, x, y, xs, ys);
        }

        bool disposed = false;
        public void Dispose() {
            if (this.disposed)
                return;

            this.disposed = true;

            if (this.lcdReset != null)
                this.lcdReset.Dispose();

            if (this.pulseLcd != null)
                this.pulseLcd.Dispose();

            if (this.tickGfx != null)
                this.tickGfx.Dispose();

            if (this.i2cDevice != null)
                this.i2cDevice.Dispose();
        }
    }
}
