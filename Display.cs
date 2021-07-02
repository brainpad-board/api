using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Drivers.BasicGraphics;
using GHIElectronics.TinyCLR.Drivers.SolomonSystech.SSD1306;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;
using static BrainPad.Controller.Image;

namespace BrainPad.Controller.Display {

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

        public static void PrintText(string text) {
            if (text.IndexOf("\f") == 0) {
                Controller?.Clear();
                Controller?.Show();
            }
            else
                Controller?.PrintText(text);
        }
        public static void Clear() => Controller?.Clear();
        public static void SetBrightness(double brightness) => Controller?.SetBrightness(brightness);
        public static void Circle(int x, int y, int r) => Controller?.Circle(x, y, r, color);
        public static void Line(int x1, int y1, int x2, int y2) => Controller?.Line(x1, y1, x2, y2, color);
        public static void Rect(int x, int y, int w, int h) => Controller?.Rect(x, y, w, h, color);
        public static void Point(int x, int y, uint c) => Controller?.Point(x, y, c);
        public static void Text(string s, int x, int y) => Controller?.Text(s, x, y, color);
        public static void TextEx(string s, int x, int y, int scalewidth, int scaleheight) => Controller?.TextEx(s, x, y, scalewidth, scaleheight, color);
        public static Image CreateImage(int width, int height, byte[] data, int hScale, int vScale, int transform) => Controller?.CreateImage(width, height, data, hScale, vScale, (Transform)transform);
        public static void Image(object img, int x, int y) => Controller?.DrawImage((Image)img, x, y);
        public static void Show() => Controller?.Show();
        public static void Color(uint c) => color = c;
    }

    internal class DisplayController : IOModule {
#if !DEBUG
        private GpioPin lcdReset;
        private SSD1306Controller pulseLcd;
        private BasicGraphics.BasicGraphics pulseGfx;
        private TickMatrixController tickGfx;
        private string[] messages = new string[8];
        private I2cDevice i2cDevice;
#endif
        public DisplayController() {
#if !DEBUG
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
#endif
        }

        public void SetBrightness(double brightness) {
#if !DEBUG
            if (BrainPad.Type.IsPulse == false) {
                this.tickGfx.SetBrightness(brightness);
            }
            else {

            }
#endif
        }

        private void InitPulseDisplay() {
#if !DEBUG
            this.lcdReset = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB2);
            this.lcdReset.SetDriveMode(GpioPinDriveMode.Output);
            this.lcdReset.Write(GpioPinValue.Low);
            Thread.Sleep(50);
            this.lcdReset.Write(GpioPinValue.High);

            var i2c = I2cController.FromName(SC13048.I2cBus.I2c2);

            this.i2cDevice = i2c.GetDevice(SSD1306Controller.GetConnectionSettings());

            this.pulseLcd = new SSD1306Controller(this.i2cDevice);
#endif
        }

        public void PrintText(string s) {
#if !DEBUG
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
#endif
        }

        public void Show() {
#if !DEBUG
            if (BrainPad.Type.IsPulse == false) {
                // nothing!
            }
            else {
                this.pulseLcd.DrawBufferNative(this.pulseGfx.Buffer);
            }
#endif
        }

        public void Clear() {
#if !DEBUG
            if (BrainPad.Type.IsPulse == false) {
                this.tickGfx.Clear();
            }
            else {
                this.pulseGfx.Clear();
                for(var i=0; i < 8; i++) {
                    this.messages[i] = "";
                }
            }
#endif
        }
        public void Circle(int x, int y, int r, uint c) {
#if !DEBUG
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawCircle((uint)c, x, y, r);
#endif
        }

        public void Line(int x1, int y1, int x2, int y2, uint c) {
#if !DEBUG
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawLine((uint)c, x1, y1, x2, y2);
#endif
        }

        public void Rect(int x, int y, int w, int h, uint c) {
#if !DEBUG
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawRectangle((uint)c, x, y, w, h);
#endif
        }

        public void Point(int x, int y, uint c) {
#if !DEBUG
            if (BrainPad.Type.IsPulse)
                this.pulseGfx.SetPixel(x, y, c);
            else {
                this.tickGfx.SetPixel(x, y, c);
            }
#endif
        }

        public void Text(string s, int x, int y, uint c) {
#if !DEBUG
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawString(s, c, x, y);
#endif
        }

        public void TextEx(string s, int x, int y, int xs, int ys, uint c) {
#if !DEBUG
            if (BrainPad.Type.IsPulse == false)
                return;
            this.pulseGfx.DrawString(s, c, x, y, xs, ys);
#endif
        }

        public Image CreateImage(int width, int height, byte[] data, int hScale, int vScale, Transform transform) => new Image(data, width, height, hScale, vScale, transform);

        public void DrawImage(Image img, int x, int y) {
#if !DEBUG
            if (BrainPad.Type.IsPulse == false)
                return;

            var index = 0;
            for (var vsize = 0; vsize < img.Height; vsize++) {
                for (var hsize = 0; hsize < img.Width; hsize++) {
                    this.pulseGfx.SetPixel(x + hsize, y + vsize, img.Data[index++]);
                }
            }
#endif
        }

        public override void Dispose() {
#if !DEBUG
            this.lcdReset?.Dispose();
            this.pulseLcd?.Dispose();
            this.tickGfx?.Dispose();
            this.i2cDevice?.Dispose();

            this.lcdReset = null;
            this.pulseLcd = null;
            this.tickGfx = null;
            this.i2cDevice = null;
#endif
        }
    }
}
