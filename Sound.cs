using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Sound : IOModule {
        static PwmController controller = PwmController.FromName(SC13048.Timer.Pwm.Software.Id);
        private PwmChannel pwmChannel;
        private double playTime;
        private double frequency = 1000;
        private double volume = 100;

        public Sound(string pinBp, double playtime, double volume) {
            pinBp = pinBp.ToLower();

            if (pinBp.CompareTo(BrainPad.TEXT_BUILTIN) == 0) {
                this.Initialize(this.frequency, playtime, SC13048.GpioPin.PB8, volume);
            }
            else {
                var pin = BrainPad.GetGpioFromBpPin(pinBp);
                this.Initialize(this.frequency, playtime, pin, volume);
            }
        }


        private void Initialize(double frequency, double playtime, int pinNum, double volume) {
            BrainPad.UnRegisterObject(pinNum);

            this.volume = Scale(volume, 0,100, 1,5) / 10.0; // /10 to get 0.1 to 0.5

            this.playTime = playtime;

            controller.SetDesiredFrequency(this.frequency);

            this.pwmChannel = controller.OpenChannel(pinNum);

            this.pwmChannel.SetActiveDutyCyclePercentage(this.volume);

            BrainPad.RegisterObject(this, pinNum);
        }

        public override void Out(double oValue) {

            var milisecond = this.playTime * 1000;

            if (this.frequency != oValue) {
                this.frequency = oValue;
                controller.SetDesiredFrequency(this.frequency);
            }
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

        public override void Dispose(bool disposing) {
            if (disposing)
                this.pwmChannel?.Dispose();

            this.pwmChannel = null;


        }

    }
}
