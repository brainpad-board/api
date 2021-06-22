using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Drivers.BasicGraphics;
using GHIElectronics.TinyCLR.Drivers.SolomonSystech.SSD1306;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController.Display {

    public static class Display {
        private static DisplayController displayController;
        private static uint color = 1;

        private static DisplayController Controller {
            get {
                if (displayController == null)
                    displayController = new DisplayController();

                return displayController;
            }
        }

        public static void Print(string text) => Controller?.Print(text);
        public static void Clear() => Controller?.Clear();
        public static void SetBrightness(double brightness) => Controller?.SetBrightness(brightness);
        public static void Circle(int x, int y, int r) => Controller?.Circle(x, y, r, color);
        public static void Line(int x1, int y1, int x2, int y2) => Controller?.Line(x1, y1, x2, y2, color);
        public static void Rect(int x, int y, int w, int h) => Controller?.Rect(x, y, w, h, color);
        public static void Point(int x, int y, uint c) => Controller?.Point(x, y, c);
        public static void Text(string s, int x, int y) => Controller?.Text(s, x, y, color);
        public static void TextEx(string s, int x, int y, int scalewidth, int scaleheight) => Controller?.TextEx(s, x, y, scalewidth, scaleheight, color);
        public static Image CreateImage(int width, int height, byte[] data) => Controller?.CreateImage(width, height, data);
        public static void Image(object img, int x, int y) => Controller?.DrawImage((Image)img, x, y);
        public static void Show() => Controller?.Show();
        public static void Color(uint c) => color = c;
    }

    internal class DisplayController : IOModule {
        private GpioPin lcdReset;
        private SSD1306Controller pulseLcd;
        private BasicGraphics.BasicGraphics pulseGfx;
        private TickMatrixController tickGfx;
        private string[] messages = new string[8];
        private I2cDevice i2cDevice;
        public DisplayController() {            

            if (BrainPad.Type.IsPulse == false) {
                this.tickGfx = new TickMatrixController();
            }
            else {
                this.InitPulseDisplay();
                this.pulseGfx = new GHIElectronics.TinyCLR.Drivers.BasicGraphics.BasicGraphics(128, 64, ColorFormat.OneBpp);
                for (var i = 0; i < 8; i++)
                    this.messages[i] = "";
            }            

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
            if (BrainPad.Type.IsPulse == false) {
                this.tickGfx.Clear();
                this.tickGfx.DrawText(s);
            }
            else {
                this.pulseGfx.Clear();
                Array.Copy(this.messages, 1, this.messages, 0, this.messages.Length - 1);
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
                for(var i=0; i < 8; i++) {
                    this.messages[i] = "";
                }
            }
        }
        public void Circle(int x, int y, int r, uint c) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawCircle((uint)c, x, y, r);
        }
        public void Line(int x1, int y1, int x2, int y2, uint c) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawLine((uint)c, x1, y1, x2, y2);
        }
        public void Rect(int x, int y, int w, int h, uint c) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawRectangle((uint)c, x, y, w, h);
        }
        public void Point(int x, int y, uint c) {
            if (BrainPad.Type.IsPulse)
                this.pulseGfx.SetPixel(x, y, c);
            else {
                this.tickGfx.SetPixel(x, y, c);
            }
        }
        public void Text(string s, int x, int y, uint c) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawString(s, c, x, y);
        }
        public void TextEx(string s, int x, int y, int xs, int ys, uint c) {
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawString(s, c, x, y, xs, ys);
        }

        public Image CreateImage(int width, int height, byte[] data) => new Image(data, width, height);

        public void DrawImage(Image img, int x, int y) {
            if (BrainPad.Type.IsPulse == false)
                return;

            this.pulseGfx.DrawImage(img, x, y);
        }

        public override void Dispose() {
            this.lcdReset?.Dispose();
            this.pulseLcd?.Dispose();
            this.tickGfx?.Dispose();
            this.i2cDevice?.Dispose();

            this.lcdReset = null;
            this.pulseLcd = null;
            this.tickGfx = null;
            this.i2cDevice = null;
        }
    }
}
