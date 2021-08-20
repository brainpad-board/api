using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    public static class Controller {
        public const int P0 = 0;
        public const int P1 = 1;
        public const int P2 = 2;
        public const int P3 = 3;
        public const int P4 = 4;
        public const int P5 = 5;
        public const int P6 = 6;
        public const int P7 = 7;
        public const int P8 = 8;
        public const int P9 = 9;
        public const int P10 = 10;
        public const int P11 = 11;
        public const int P12 = 12;
        public const int P13 = 13;
        public const int P14 = 14;
        public const int P15 = 15;
        public const int P16 = 16;
        public const int P19 = 19;
        public const int P20 = 20;

        public const int LED = 21;
        public const int Led = 21;
        public const int Buzzer = 22;
        public const int ButtonA = 23;
        public const int ButtonB = 24;

        public const int AccelX = 25;
        public const int AccelY = 26;
        public const int AccelZ = 27;

        public static readonly int[] PinMapPulse = {
            SC13048.GpioPin.PA5, //P0
            SC13048.GpioPin.PA3, //P1
            SC13048.GpioPin.PA2, //P2
            SC13048.GpioPin.PA1, //P3
            SC13048.GpioPin.PA0, //P4
            SC13048.GpioPin.PA7, //P5
            SC13048.GpioPin.PA4, //P6
            SC13048.GpioPin.PB0, //P7
            SC13048.GpioPin.PA9, //P8
            SC13048.GpioPin.PB1, //P9
            SC13048.GpioPin.PA6, //P10
            SC13048.GpioPin.PB6, //P11
            SC13048.GpioPin.PA10, //P12
            SC13048.GpioPin.PB3, //P13
            SC13048.GpioPin.PB4, //P14
            SC13048.GpioPin.PB5, //P15
            SC13048.GpioPin.PB12, //P16
            -1                  , //P17
            -1                  , //P18
            SC13048.GpioPin.PB10, //P19
            SC13048.GpioPin.PB11, //P20
            SC13048.GpioPin.PA8, //LED
            SC13048.GpioPin.PB8, //BUZZER
            SC13048.GpioPin.PC13, //A
            SC13048.GpioPin.PB7, //B
        };

        public static readonly int[] PinMapTick = {
            SC13048.GpioPin.PA5, //P0
            SC13048.GpioPin.PA3, //P1
            SC13048.GpioPin.PA2, //P2
            -1                 , //P3
            -1                 , //P4
            -1                 , //P5
            -1                 , //P6
            -1                 , //P7
            -1                 , //P8
            -1                 , //P9
            -1                 , //P10
            -1                 , //P11
            SC13048.GpioPin.PA5, //P12
            SC13048.GpioPin.PB3, //P13
            SC13048.GpioPin.PB4, //P14
            SC13048.GpioPin.PB5, //P15
            SC13048.GpioPin.PA3, //P16
            -1                 , //P17
            -1                 , //P18
            SC13048.GpioPin.PB10, //P19
            SC13048.GpioPin.PB11, //P20
            SC13048.GpioPin.PA8, //LED
            SC13048.GpioPin.PB8, //BUZZER
            SC13048.GpioPin.PC13, //A
            SC13048.GpioPin.PB7, //B
        };

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

        public static bool IsPwmFromPin(double pin) {
            if (IsPulse) {
                switch (pin) {
                    case P3:
                    case P0:
                    case P1:
                    case P8:
                    case P12:
                    case P2:
                    case Buzzer:
                        return true;

                }
            }
            else {
                switch (pin) {
                    case P0:
                    case P2:
                    case P1:
                    case P15:
                    case P16:
                    case Buzzer:
                        return true;
                }
            }

            return false;
        }
        public static int GetGpioFromPin(double pin) => Controller.IsPulse ? Controller.PinMapPulse[(int)pin] : Controller.PinMapTick[(int)pin];

        public static void Wait(double seconds) => Thread.Sleep((int)(seconds * 1000));
        public static IOModule Analog(double pin) => new Analog(pin);        
        public static IOModule Digital(double pin) => new Digital(pin);
        public static IOModule Sound(double pin, double playtime, double volume) => new Sound(pin, playtime, volume);        
        public static IOModule Button(double button, double detectPeriod) => new BrainPad.Button(button, detectPeriod);        
        public static IOModule Accel(double xyz) => new Accel(xyz);
        public static IOModule Servo(double pin) => new Servo(pin);
        public static IOModule Neopixel(double pin, double lednums) => new Neopixel(pin, lednums);
        public static IOModule I2cBus(double address) => new I2cBus((int)address);
        public static IOModule Distance(double triggerPin, double echoPin) => new Distance(triggerPin, echoPin);
        public static IOModule Touch(double pin, double senstitiveLevel) => new Touch(pin, senstitiveLevel);
        public static IOModule Infrared(double receivePin) => new Infrared(receivePin);
        public static void Print(object obj) => Display.Print(obj.ToString());
        public static double In(IOModule module) => module.In();
        public static void Out(IOModule module, double[] oValue) => module.Out(oValue);
        public static void Out(IOModule module, double oValue) => module.Out(oValue);
        public static double OutIn(IOModule module, double[] dataOut, double[] dataIn) => module.OutIn(dataOut, dataIn);

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
