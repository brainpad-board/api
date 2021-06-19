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
        public Sound(BrainPad.Pin pinBp, double playtime) {
            var pin = BrainPad.GetGpioFromBpPin(pinBp);
            this.Initialize(this.frequency, playtime, pin);
        }

        public Sound(string pinBp, double playtime) {
            pinBp = pinBp.ToLower();

            if (pinBp.CompareTo("builtin") == 0) {
                this.Initialize(this.frequency, playtime, SC13048.GpioPin.PB8);
            }
            else {
                var pin = BrainPad.GetGpioFromBpPin(pinBp);
                this.Initialize(this.frequency, playtime, pin);
            }
        }


        private void Initialize(double frequency, double playtime, int pinNum) {
            BrainPad.UnRegisterObject(pinNum);

            this.playTime = playtime;

            controller.SetDesiredFrequency(this.frequency);

            this.pwmChannel = controller.OpenChannel(pinNum);

            this.pwmChannel.SetActiveDutyCyclePercentage(0.5);

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

        
        public override void Dispose(bool disposing) {
            if (disposing)
                this.pwmChannel?.Dispose();

            this.pwmChannel = null;

            
        }
       
    }
}
