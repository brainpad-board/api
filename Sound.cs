using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Sound : IOModule {
        private PwmChannel pwmChannel;
        private PwmController pwmController;
        private double playTime;
        private double volume = 100;
        private string timer;
        private int channel;

        public Sound(string pinBp, double playtime, double volume) {
            pinBp = pinBp.ToLower();

            if (pinBp.CompareTo(BrainPad.TEXT_BUILTIN) == 0) {
                this.Initialize(playtime, SC13048.GpioPin.PB8, volume);
            }
            else {
                var pin = BrainPad.GetGpioFromString(pinBp);

                if (!BrainPad.IsPwmFromString(pinBp)) {
                    throw new ArgumentException("Not support on this pin.");
                }

                this.Initialize(playtime, pin, volume);
            }
        }


        private void Initialize(double playtime, int pinNum, double volume) {
            if (pinNum < 0) {
                throw new ArgumentException("Invalid pin number.");
            }

            this.channel = BrainPad.GetPwmChannelFromPin(pinNum);
            this.timer = BrainPad.GetPwmTimerFromPin(pinNum);

            if (this.channel < 0 || this.timer == null) {
                throw new ArgumentException("Not supported on this pin.");
            }

            this.volume = Scale(volume, 0, 100, 1, 50) / 100.0; // /100 to get 0.01 to 0.5

            this.playTime = playtime;

            this.pwmController = PwmController.FromName(this.timer);
            this.pwmChannel = this.pwmController.OpenChannel(this.channel);

        }

        public override void Out(double oValue) {

            var milisecond = this.playTime * 1000;
   
            this.pwmController.SetDesiredFrequency(oValue);
            this.pwmChannel.SetActiveDutyCyclePercentage(this.volume);
            this.pwmChannel.Start();

            if (milisecond > 0) {
                Thread.Sleep((int)milisecond);
                this.pwmChannel.Stop();
            }
        }

        static int Scale(double value, int originalMin, int originalMax, int scaleMin, int scaleMax) {
            var scale = (double)(scaleMax - scaleMin) / (originalMax - originalMin);
            var ret = (int)(scaleMin + ((value - originalMin) * scale));

            return ret > scaleMax ? scaleMax : (ret < scaleMin ? scaleMin : ret);
        }

        public override void Dispose() {
            this.pwmController?.Dispose();
            this.pwmChannel?.Dispose();
            this.pwmChannel = null;
            this.pwmController = null;
        }
    }
}
