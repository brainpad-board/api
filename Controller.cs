using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    public static class Controller {
        internal const string TEXT_BUILTIN = "builtin";
        internal const string TEXT_PULLUP = "pullup";
        internal const string TEXT_PULLDOWN = "pulldown";
        internal const string TEXT_NOPULL = "nopull";

        internal static GpioController Gpio = GpioController.GetDefault();
        internal static PwmController PwmSoftware = PwmController.FromName(SC13048.Timer.Pwm.Software.Id);

        private static bool isPulse;
        static Controller() {
            using (var pb15 = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB15)) {
                pb15.SetDriveMode(GpioPinDriveMode.InputPullDown);
                isPulse = pb15.Read() == GpioPinValue.High;
            }
        }

        public static bool IsPulse => isPulse;
        public static bool IsTick => !isPulse;

        public static int GetPwmChannelFromPin(int pin) {
            switch (pin) {
                case SC13048.GpioPin.PA8: return SC13048.Timer.Pwm.Controller1.PA8;
                case SC13048.GpioPin.PA9: return SC13048.Timer.Pwm.Controller1.PA9;
                case SC13048.GpioPin.PA10: return SC13048.Timer.Pwm.Controller1.PA10;

                case SC13048.GpioPin.PA5: return SC13048.Timer.Pwm.Controller2.PA5;
                case SC13048.GpioPin.PA1: return SC13048.Timer.Pwm.Controller2.PA1;
                case SC13048.GpioPin.PB10: return SC13048.Timer.Pwm.Controller2.PB10;
                case SC13048.GpioPin.PB11: return SC13048.Timer.Pwm.Controller2.PB11;


                case SC13048.GpioPin.PA2: return SC13048.Timer.Pwm.Controller15.PA2;
                case SC13048.GpioPin.PA3: return SC13048.Timer.Pwm.Controller15.PA3;


                case SC13048.GpioPin.PB8: return SC13048.Timer.Pwm.Controller16.PB8;
            }

            return -1;
        }

        public static string GetPwmTimerFromPin(int pin) {
            switch (pin) {
                case SC13048.GpioPin.PA8:
                case SC13048.GpioPin.PA9:
                case SC13048.GpioPin.PA10:
                    return SC13048.Timer.Pwm.Controller1.Id;

                case SC13048.GpioPin.PA5:
                case SC13048.GpioPin.PA1:
                case SC13048.GpioPin.PB10:
                case SC13048.GpioPin.PB11:
                    return SC13048.Timer.Pwm.Controller2.Id;

                case SC13048.GpioPin.PA2:
                case SC13048.GpioPin.PA3:
                    return SC13048.Timer.Pwm.Controller15.Id;

                case SC13048.GpioPin.PB8:
                    return SC13048.Timer.Pwm.Controller16.Id;
            }

            return null;

        }

        public static bool IsPwmFromString(string pin) {
            pin = pin.ToLower();

            if (IsPulse) {
                switch (pin) {
                    case "p3":
                    case "p0":
                    case "p1":
                    case "p8":
                    case "p12":
                    case "p2":
                        return true;

                }
            }
            else {
                switch (pin) {
                    case "p0":
                    case "p2":
                    case "p1":
                    case "p15":
                    case "p16":
                        return true;
                }
            }

            return false;
        }
        public static int GetGpioFromString(string pin) {
            pin = pin.ToLower();

            switch (pin) {
                case "p0":
                    return SC13048.GpioPin.PA5; // same P12 on tick

                case "p1":
                    return SC13048.GpioPin.PA3; // same P16 on tick

                case "p2":
                    return SC13048.GpioPin.PA2;

                case "p3":
                    return IsPulse ? SC13048.GpioPin.PA1 : -1;

                case "p4":
                    return IsPulse ? SC13048.GpioPin.PA0 : -1;

                case "p5":
                    return IsPulse ? SC13048.GpioPin.PA7 : -1;

                case "p6":
                    return IsPulse ? SC13048.GpioPin.PA4 : -1;

                case "p7":
                    return IsPulse ? SC13048.GpioPin.PB0 : -1;

                case "p8":
                    return IsPulse ? SC13048.GpioPin.PA9 : -1;

                case "p9":
                    return IsPulse ? SC13048.GpioPin.PB1 : -1;

                case "p10":
                    return IsPulse ? SC13048.GpioPin.PA6 : -1;

                case "p11":
                    return IsPulse ? SC13048.GpioPin.PB6 : -1;

                case "p12":
                    return IsPulse ? SC13048.GpioPin.PA10 : SC13048.GpioPin.PA5;

                case "p13":
                    return SC13048.GpioPin.PB3;

                case "p14":
                    return SC13048.GpioPin.PB4;

                case "p15":
                    return SC13048.GpioPin.PB5;

                case "p16":
                    return IsPulse ? SC13048.GpioPin.PB12 : SC13048.GpioPin.PA3;

                case "p19":
                    return SC13048.GpioPin.PB10;

                case "p20":
                    return SC13048.GpioPin.PB11;
            }

            return -1;
        }

        public static void Wait(double seconds) => Thread.Sleep((int)(seconds * 1000));
        public static IOModule Analog(string pin) => new Analog(pin);
        public static IOModule Digital(string pin, string pull) => new Digital(pin, pull);
        public static IOModule Sound(string pin, double playtime, double volume) => new Sound(pin, playtime, volume);
        public static IOModule Button(string button, double detectPeriod) => new BrainPad.Button(button, detectPeriod);
        public static IOModule Accel(string xyz) => new Accel(xyz);
        public static IOModule Servo(string pin) => new Servo(pin);
        public static IOModule Neopixel(string pin, int lednums) => new Neopixel(pin, lednums);
        public static IOModule I2cBus(int address) => new I2cBus(address);
        public static IOModule Ultrasonic(string triggerPin, string echoPin) => new Ultrasonic(triggerPin, echoPin);
        public static IOModule Touch(string touchPin, int senstitiveLevel) => new Touch(touchPin, senstitiveLevel);
        public static IOModule Infrared(string receivePin) => new Infrared(receivePin);
        public static void Print(string text) => Display.Print(text);
        public static void Print(object obj) => Display.Print(obj.ToString());
        public static double In(IOModule module) => module.In();
        public static void Out(IOModule module, double[] oValue) => module.Out(oValue);
        public static void Out(IOModule module, double oValue) => module.Out(oValue);
        public static double OutIn(IOModule module, byte[] dataOut, byte[] dataIn) => module.OutIn(dataOut, dataIn);

        public static void Release(object o) {
            if (o is IDisposable disposable) disposable.Dispose();
        }

        internal static int Scale(double value, int originalMin, int originalMax, int scaleMin, int scaleMax) {
            var scale = (double)(scaleMax - scaleMin) / (originalMax - originalMin);
            var ret = (int)(scaleMin + ((value - originalMin) * scale));

            return ret > scaleMax ? scaleMax : (ret < scaleMin ? scaleMin : ret);
        }

    }

}
