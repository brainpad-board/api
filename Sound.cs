using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    public class Sound : IOModule {
        private PwmChannel pwmChannel;
        private PwmController pwmController;
        private double playTime;
        private double volume = 100;
        private string timer;
        private int channel;

        //const string BUILTIN_TEXT = "buzzer";
        public Sound(double pinBp, double playtime, double volume) {
            var pin = Controller.GetGpioFromPin(pinBp);

            if (!Controller.IsPwmFromPin(pinBp)) {
                throw new ArgumentException("Not support on this pin.");
            }

            this.Initialize(playtime, pin, volume);
        }
        //public Sound(string pinBp, double playtime, double volume) {
        //    pinBp = pinBp.ToLower();

        //    if (pinBp.CompareTo(BUILTIN_TEXT) == 0) {
        //        this.Initialize(playtime, SC13048.GpioPin.PB8, volume);
        //    }
        //    else {
        //        throw new ArgumentException("Not support on this pin.");
        //    }
        //}


        private void Initialize(double playtime, int pinNum, double volume) {
            if (pinNum < 0) {
                throw new ArgumentException("Invalid pin number.");
            }

            this.channel = Controller.GetPwmChannelFromPin(pinNum);
            this.timer = Controller.GetPwmTimerFromPin(pinNum);

            if (this.channel < 0 || this.timer == null) {
                throw new ArgumentException("Not supported on this pin.");
            }

            this.volume = volume == 0 ? 0 : Controller.Scale(volume, 0, 100, 1, 50) / 100.0; // /100 to get 0.01 to 0.5

            this.playTime = playtime;

            this.pwmController = PwmController.FromName(this.timer);
            this.pwmChannel = this.pwmController.OpenChannel(this.channel);
            this.pwmChannel.SetActiveDutyCyclePercentage(this.volume);
        }

        public override void Out(double oValue) {
            if (oValue > 0 && this.volume > 0) {
                this.pwmController.SetDesiredFrequency(oValue);
                this.pwmChannel.Start();

                if (this.playTime > 0) {
                    var milisecond = this.playTime * 1000;
                    Thread.Sleep((int)milisecond);
                    this.pwmChannel.Stop();
                }
            }
            else {
                this.pwmChannel.Stop();
            }
        }

        public override void Dispose() {
            this.pwmController?.Dispose();
            this.pwmChannel?.Dispose();
            this.pwmChannel = null;
            this.pwmController = null;
        }
    }
}
