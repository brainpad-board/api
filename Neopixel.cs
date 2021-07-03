using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.Worldsemi.WS2812;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace BrainPad.Controller {
    public class Neopixel : IOModule {
#if !DEBUG
        private int numLeds;
        private GpioPin gpioPin;

        WS2812Controller ws2812;
#endif
        public Neopixel(string bpPin, int numleds) {
#if !DEBUG
            this.numLeds = numleds;
            var pinNum = BrainPad.GetGpioFromString(bpPin);
            this.Initialize(pinNum);
#endif
        }

        private void Initialize(int pinNum) {
#if !DEBUG
            if (pinNum < 0) {
                throw new ArgumentException("Invalid pin number.");
            }            

            this.gpioPin = BrainPad.Gpio.OpenPin(pinNum);

            this.ws2812 = new WS2812Controller(this.gpioPin, (uint)this.numLeds, WS2812Controller.DataFormat.rgb888);            
#endif
        }

        public override void Out(double[] data) {
#if !DEBUG
            for (var i = 0; i < data.Length; ++i) {
                var v = (int)data[i];
                var r = (byte)((v >> 16) & 0xFF);
                var g = (byte)((v >> 8) & 0xFF);
                var b = (byte)((v >> 0) & 0xFF);

                this.ws2812.SetColor(i, r, g, b);
            }

            this.ws2812.Flush();
#endif
        }
        public override void Out(double data) {
#if !DEBUG
            var v = (int)data;
            var r = (byte)((v >> 16) & 0xFF);
            var g = (byte)((v >> 8) & 0xFF);
            var b = (byte)((v >> 0) & 0xFF);

            for (var i = 0; i < this.numLeds; ++i) {
                this.ws2812.SetColor(i, r, g, b);
            }

            this.ws2812.Flush();
#endif
        }



        public override void Dispose() {
#if !DEBUG
            this.gpioPin?.Dispose();

            this.gpioPin = null;
#endif
        }
    }
}
