using System;
using GHIElectronics.TinyCLR.Devices.Adc;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    public class Analog : IOModule {
        static AdcController adcController = AdcController.FromName(SC13048.Adc.Controller1.Id);
        private AdcChannel adcChannel;

        private PwmChannel pwmChannel;
        private int pinNum;

        public Analog(string bpPin) {
            var pinNum = Controller.GetGpioFromString(bpPin);

            this.Initialize(pinNum);
        }
        private void Initialize(int pinNum) {
            if (pinNum < 0)
                throw new ArgumentException("Invalid pin number.");

            this.pinNum = pinNum;
        }


        public override void Dispose() {
            this.adcChannel?.Dispose();
            this.pwmChannel?.Dispose();

            this.adcChannel = null;
            this.pwmChannel = null;
        }

        public override double In() {
            if (this.pwmChannel != null) {
                this.pwmChannel.Dispose();
                this.pwmChannel = null;
            }
            if (this.adcChannel == null) {
                var channelNum = GetChannelFromPin(this.pinNum);

                if (channelNum < 0) {
                    throw new ArgumentException("Invalid channel.");
                }


                this.adcChannel = adcController.OpenChannel(channelNum);
            }

            var v = this.adcChannel.ReadRatio();
            
            return (int)BrainPad.Controller.Scale(v, 0, 1, 0, 100);
        }

        public override void Out(double oValue) {
            if (this.adcChannel != null) {
                this.adcChannel.Dispose();
                this.adcChannel = null;

            }

            this.pwmChannel?.Dispose();

            Controller.PwmSoftware.SetDesiredFrequency(1000);

            this.pwmChannel = Controller.PwmSoftware.OpenChannel(this.pinNum);

            this.pwmChannel.SetActiveDutyCyclePercentage(oValue / 100);

            this.pwmChannel.Start();
        }

        static int GetChannelFromPin(int pin) {
            switch (pin) {
                case SC13048.GpioPin.PA5:
                    return SC13048.Adc.Controller1.PA5;

                case SC13048.GpioPin.PA3:
                    return SC13048.Adc.Controller1.PA3;

                case SC13048.GpioPin.PA2:
                    return SC13048.Adc.Controller1.PA2;

                case SC13048.GpioPin.PA1:
                    return Controller.IsPulse ? SC13048.Adc.Controller1.PA1 : -1;

                case SC13048.GpioPin.PA0:
                    return Controller.IsPulse ? SC13048.Adc.Controller1.PA0 : -1;

                case SC13048.GpioPin.PA7:
                    return Controller.IsPulse ? SC13048.Adc.Controller1.PA7 : -1;

                case SC13048.GpioPin.PA4:
                    return Controller.IsPulse ? SC13048.Adc.Controller1.PA4 : -1;

                case SC13048.GpioPin.PB0:
                    return Controller.IsPulse ? SC13048.Adc.Controller1.PB0 : -1;

                case SC13048.GpioPin.PB1:
                    return Controller.IsPulse ? SC13048.Adc.Controller1.PB1 : -1;

                case SC13048.GpioPin.PA6:
                    return Controller.IsPulse ? SC13048.Adc.Controller1.PA6 : -1;
            }

            return -1;
        }

    }
}
