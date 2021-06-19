using GHIElectronics.TinyCLR.Devices.Adc;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Analog : IOModule {


        static AdcController adcController = AdcController.FromName(SC13048.Adc.Controller1.Id);
        private AdcChannel adcChannel;
        static PwmController pwmController = PwmController.FromName(SC13048.Timer.Pwm.Software.Id);
        private PwmChannel pwmChannel;
        private int pinNum;
        private double dutyCycle = 0;

        public Analog(BrainPad.Pin bpPin) {
            var pinNum = BrainPad.GetGpioFromBpPin(bpPin);
            this.Initialize(pinNum);
        }


        public Analog(string bpPin) {
            var pinNum = BrainPad.GetGpioFromBpPin(bpPin);

            this.Initialize(pinNum);
        }


        private void Initialize(int pinNum) {
            this.pinNum = pinNum;

            BrainPad.UnRegisterObject(pinNum);

            pwmController.SetDesiredFrequency(1000);

            BrainPad.RegisterObject(this, pinNum);

        }


        public override void Dispose(bool disposing) {
            if (disposing) {

                this.adcChannel?.Dispose();
                this.pwmChannel?.Dispose();
            }

            this.adcChannel = null;
            this.pwmChannel = null;
        }

        public override double In() {
            if (this.pwmChannel != null) {
                this.pwmChannel.Dispose();
                this.pwmChannel = null;
                this.dutyCycle = 0;
            }
            if (this.adcChannel == null) {
                var channelNum = GetChannelFromPin(this.pinNum);

                if (channelNum < 0) {
                    throw new ArgumentException("Invalid channel.");
                }


                this.adcChannel = adcController.OpenChannel(channelNum);
            }

            return this.adcChannel.ReadRatio();
        }

        public override void Out(double oValue) {
            if (this.adcChannel != null) {
                this.adcChannel.Dispose();
                this.adcChannel = null;

            }
            if (this.pwmChannel == null) {
                this.pwmChannel = pwmController.OpenChannel(this.pinNum);
                this.dutyCycle = 0;
            }

            if (this.dutyCycle != oValue) {
                this.dutyCycle = oValue;

                this.pwmChannel.Stop();

                this.pwmChannel.SetActiveDutyCyclePercentage(this.dutyCycle / 100);

                this.pwmChannel.Start();
            }
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
                    return SC13048.Adc.Controller1.PA1;

                case SC13048.GpioPin.PA0:
                    return SC13048.Adc.Controller1.PA0;

                case SC13048.GpioPin.PA7:
                    return SC13048.Adc.Controller1.PA7;

                case SC13048.GpioPin.PA4:
                    return SC13048.Adc.Controller1.PA4;

                case SC13048.GpioPin.PB0:
                    return SC13048.Adc.Controller1.PB0;

                case SC13048.GpioPin.PA9:
                    return -1;

                case SC13048.GpioPin.PB1:
                    return SC13048.Adc.Controller1.PB1;

                case SC13048.GpioPin.PA6:
                    return SC13048.Adc.Controller1.PA6;

                case SC13048.GpioPin.PB6:
                    return -1;

                case SC13048.GpioPin.PA10:
                    return -1;

                case SC13048.GpioPin.PB3:
                    return -1;

                case SC13048.GpioPin.PB4:
                    return -1;

                case SC13048.GpioPin.PB5:
                    return -1;

                case SC13048.GpioPin.PB12:
                    return -1;
            }

            return -1;
        }

    }
}
