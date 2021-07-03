using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Drivers.Motor.Servo;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace BrainPad {
    public class Servo : IOModule {
        PwmChannel pwmChannel;
        ServoController servo;
        public Servo(string bpPin) {
            var pinNum = Controller.GetGpioFromString(bpPin);
            this.Initialize(pinNum);
        }

        private void Initialize(int pinNum) {
            if (pinNum < 0) {
                throw new ArgumentException("Invalid pin number.");
            }            

            this.pwmChannel = Controller.PwmSoftware.OpenChannel(pinNum);

            this.servo = new ServoController(Controller.PwmSoftware, this.pwmChannel);            

        }
        public override void Out(double oValue) => this.servo.Set(oValue);

        public override void Dispose() {
            this.pwmChannel?.Dispose(); ;

            this.pwmChannel = null;
        }

    }
}
