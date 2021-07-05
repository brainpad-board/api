using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Drivers.BasicGraphics;
using GHIElectronics.TinyCLR.Drivers.SolomonSystech.SSD1306;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {

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

        internal static void Print(string text) {
            if (text == "\\f") {
                Controller.Clear();
                Controller.Show();
            }
            else {
                Controller.PrintText(text);
            }
        }
        public static void Clear() => Controller.Clear();
        public static void SetBrightness(double brightness) => Controller.SetBrightness(brightness);
        public static void Circle(int x, int y, int r) => Controller.Circle(x, y, r, color);
        public static void Line(int x1, int y1, int x2, int y2) => Controller.Line(x1, y1, x2, y2, color);
        public static void Rect(int x, int y, int w, int h) => Controller.Rect(x, y, w, h, color);
        public static void Point(int x, int y, uint c) => Controller.Point(x, y, c);
        public static void Text(string s, int x, int y) => Controller.Text(s, x, y, color);
        public static void TextEx(string s, int x, int y, int scalewidth, int scaleheight) => Controller.TextEx(s, x, y, scalewidth, scaleheight, color);
        public static Image CreateImage(int width, int height, byte[] data, int hScale, int vScale, int transform) => Controller.CreateImage(width, height, data, hScale, vScale, (Image.Transform)transform);
        public static Image CreateImage(int width, int height, string data, int hScale, int vScale, int transform) => Controller.CreateImage(width, height, data, hScale, vScale, (Image.Transform)transform);
        public static void Image(object img, int x, int y) => Controller.DrawImage((Image)img, x, y);
        public static void Show() => Controller.Show();
        public static void Color(uint c) => color = c;
    }

    internal class DisplayController : IOModule {
        private GpioPin lcdReset;
        private SSD1306Controller pulseLcd;
        private BasicGraphics gfx;
        private string[] messages = new string[8];
        private I2cDevice i2cDevice;

        public DisplayController() {
            if (Controller.IsPulse == false) {
                this.gfx = new TickMatrixController();
            }
            else {
                this.InitPulseDisplay();
                this.gfx = new BasicGraphics(128, 64, ColorFormat.OneBpp);
                for (var i = 0; i < 8; i++)
                    this.messages[i] = "";
            }

            this.Clear();
        }

        public void SetBrightness(double brightness) {
            if (Controller.IsPulse == false) {
                ((TickMatrixController)this.gfx).SetBrightness(brightness);
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

        public void PrintText(string s) {
            this.gfx.Clear();
            if (Controller.IsPulse == false) {
                ((TickMatrixController)this.gfx).DrawText(s);
            }
            else {
                Array.Copy(this.messages, 1, this.messages, 0, this.messages.Length - 1);
                this.messages[7] = s;
                for (var i = 0; i < 8; i++)
                    this.gfx.DrawString(this.messages[i], 1, 0, i * 8);
            }
            this.Show();
        }

        public void Show() {
            if (Controller.IsPulse) {
                this.pulseLcd.DrawBufferNative(this.gfx.Buffer);
            }
        }

        public void Clear() {
            this.gfx.Clear();
            if (Controller.IsPulse) { 
                for (var i = 0; i < 8; i++) {
                    this.messages[i] = "";
                }
            }
        }
        public void Circle(int x, int y, int r, uint c) => this.gfx.DrawCircle((uint)c, x, y, r);

        public void Line(int x1, int y1, int x2, int y2, uint c) => this.gfx.DrawLine((uint)c, x1, y1, x2, y2);

        public void Rect(int x, int y, int w, int h, uint c)  => this.gfx.DrawRectangle((uint)c, x, y, w, h);
    
        public void Point(int x, int y, uint c) => this.gfx.SetPixel(x, y, c);
           
        public void Text(string s, int x, int y, uint c) {
            if (Controller.IsPulse) {
                this.gfx.DrawString(s, c, x, y);
            } else {
                ((TickMatrixController)this.gfx).DrawText(s);
            }
        }

        public void TextEx(string s, int x, int y, int xs, int ys, uint c) {
            if (Controller.IsPulse) {
                this.gfx.DrawString(s, c, x, y, xs, ys);
            }
            else {
                ((TickMatrixController)this.gfx).DrawText(s);
            }
        }

        public Image CreateImage(int width, int height, byte[] data, int hScale, int vScale, Image.Transform transform) => new Image(data, width, height, hScale, vScale, transform);
        public Image CreateImage(int width, int height, string data, int hScale, int vScale, Image.Transform transform) => new Image(data, width, height, hScale, vScale, transform);

        public void DrawImage(Image img, int x, int y) {
            var index = 0;
            for (var vsize = 0; vsize < img.Height; vsize++) {
                for (var hsize = 0; hsize < img.Width; hsize++) {
                    this.gfx.SetPixel(x + hsize, y + vsize, img.Data[index++]);
                }
            }
        }

        public override void Dispose() {
            this.lcdReset?.Dispose();
            this.pulseLcd?.Dispose();
            if (this.gfx is IDisposable disposable) disposable.Dispose();
            this.i2cDevice?.Dispose();

            this.lcdReset = null;
            this.pulseLcd = null;
            this.gfx = null;
            this.i2cDevice = null;
        }
    }
}
