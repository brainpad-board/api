using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Drivers.BrainPadController.Display;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public static class BrainPad {
        internal const string TEXT_BUILTIN = "builtin";
        internal const string TEXT_PULLUP = "pullup";
        internal const string TEXT_PULLDOWN = "pulldown";
        internal const string TEXT_NOPULL = "nopull";

        internal const int DISPLAY_REGISTER_ID = 0xA0;
        internal const int I2C_REGISTER_ID = 0xA1;
        internal const int DISTANCESENSOR_REGISTER_ID = 0xA2;
        internal const int ACCEL_REGISTER_ID = 0xA3;

        internal const uint PIN_RESERVED_0 = (1 << 0);
        internal const uint PIN_RESERVED_1 = (1 << 1);
        internal const uint PIN_RESERVED_12 = (1 << 12);
        internal const uint PIN_RESERVED_16 = (1 << 16);

        internal static GpioController Gpio = GpioController.GetDefault();
        internal static PwmController PwmSoftware = PwmController.FromName(SC13048.Timer.Pwm.Software.Id);

        public enum Button {
            A = SC13048.GpioPin.PC13,
            B = SC13048.GpioPin.PB7,
        }

        internal static BrainPadType Type = new BrainPadType();

        public static Hashtable Modules = new Hashtable();

        private static uint pinReserved = 0;

        internal static void ClearPinReserved(int pin) {
            if (Type.IsPulse)
                return;
           

            switch (pin) {
                case SC13048.GpioPin.PA5:               
                    pinReserved &= ~(PIN_RESERVED_0 | PIN_RESERVED_12);
                    return;

                case SC13048.GpioPin.PA3:                
                    pinReserved &= ~(PIN_RESERVED_1 | PIN_RESERVED_16);
                    return;
            }

        }

        internal static void AddPinReserved(string pin) {
            if (Type.IsPulse)
                return;

            pin = pin.ToLower();
            switch (pin) {
                case "p0":
                    pinReserved |= PIN_RESERVED_0;
                    return;
                case "p1":
                    pinReserved |= PIN_RESERVED_1;
                    return;
                case "p12":
                    pinReserved |= PIN_RESERVED_12;
                    return;
                case "p16":
                    pinReserved |= PIN_RESERVED_16;
                    return;

            }

        }
        internal static bool IsPinReserved(string pin) {
            if (Type.IsPulse)
                return false;

            pin = pin.ToLower();

            switch (pin) {
                case "p0":
                    if ((pinReserved & (PIN_RESERVED_12)) != 0) {
                        return true;
                    }
                    break;

                case "p1":
                    if ((pinReserved & (PIN_RESERVED_16)) != 0) {
                        return true;
                    }
                    break;

                case "p12":
                    if ((pinReserved & (PIN_RESERVED_0)) != 0) {
                        return true;
                    }
                    break;

                case "p16":
                    if ((pinReserved & (PIN_RESERVED_1)) != 0) {
                        return true;
                    }
                    break;
            }

            return false;
        }
        public static int GetGpioFromString(string pin) {
            pin = pin.ToLower();

            if (IsPinReserved(pin))
                return -1;

            AddPinReserved(pin);

            switch (pin) {
                case "p0":
                    return Type.IsPulse ? SC13048.GpioPin.PA5 : SC13048.GpioPin.PA5; // same P12 on tick

                case "p1":
                    return Type.IsPulse ? SC13048.GpioPin.PA3 : SC13048.GpioPin.PA3; // same P16 on tick

                case "p2":
                    return SC13048.GpioPin.PA2;

                case "p3":
                    return Type.IsPulse ? SC13048.GpioPin.PA1 : -1;

                case "p4":
                    return Type.IsPulse ? SC13048.GpioPin.PA0 : -1;

                case "p5":
                    return Type.IsPulse ? SC13048.GpioPin.PA7 : -1;

                case "p6":
                    return Type.IsPulse ? SC13048.GpioPin.PA4 : -1;

                case "p7":
                    return Type.IsPulse ? SC13048.GpioPin.PB0 : -1;

                case "p8":
                    return Type.IsPulse ? SC13048.GpioPin.PA9 : -1;

                case "p9":
                    return Type.IsPulse ? SC13048.GpioPin.PB1 : -1;

                case "p10":
                    return Type.IsPulse ? SC13048.GpioPin.PA6 : -1;

                case "p11":
                    return Type.IsPulse ? SC13048.GpioPin.PB6 : -1;

                case "p12":
                    return Type.IsPulse ? SC13048.GpioPin.PA10 : SC13048.GpioPin.PA5;

                case "p13":
                    return SC13048.GpioPin.PB3;

                case "p14":
                    return SC13048.GpioPin.PB4;

                case "p15":
                    return SC13048.GpioPin.PB5;

                case "p16":
                    return Type.IsPulse ? SC13048.GpioPin.PB12 : SC13048.GpioPin.PA3;

                case "p19":
                    return SC13048.GpioPin.PB10;

                case "p20":
                    return SC13048.GpioPin.PB11;
            }

            return -1;

        }

        public static bool IsPulse() => BrainPad.Type.IsPulse == true ? true : false;

        public static void Wait(double seconds) {
            var t = (int)(seconds * 1000);
            Thread.Sleep(t);
        }

        public static IOModule Analog(string pin) => new Analog(pin);

        public static IOModule Digital(string pin, string pull) => new Digital(pin, pull);

        public static IOModule Sound(string pin, double playtime, double volume) => new Sound(pin, playtime, volume);

        public static IOModule Buttons(string button, double detectPeriod) => new Buttons(button, detectPeriod);

        public static IOModule Accel(string xyz) => new Accel(xyz);

        public static IOModule Servo(string pin) => new Servo(pin);

        public static IOModule Neopixel(string pin, int lednums) => new Neopixel(pin, lednums);

        public static IOModule I2cBus(int address) => new I2cBus(address);

        public static IOModule DistanceSensor(string triggerPin, string echoPin) => new DistanceSensor(triggerPin, echoPin);

        public static double In(IOModule module) {
            if (module is Analog analog) {
                return analog.In();
            }

            else if (module is Digital digitalIn) {
                return digitalIn.In();
            }
            else if (module is Buttons buttons) {
                return buttons.In();
            }
            else if (module is Accel accel) {
                return accel.In();
            }
            else if (module is I2cBus i2c) {
                return i2c.In();
            }
            else if (module is DistanceSensor ds) {
                return ds.In();
            }
            else throw new Exception("Module is not supported.");
        }
        public static void Out(IOModule module, double[] oValue) {
            if (module is Neopixel neopixel) {
                neopixel.Out(oValue);

            }
            else if (module is I2cBus i2c) {
                i2c.Out(oValue);
            }
            else throw new Exception("Module is not supported.");
        }

        public static void Out(IOModule module, double oValue) {
            if (module is Analog analog) {
                analog.Out(oValue);
            }
            else if (module is Digital digital) {
                digital.Out(oValue);
            }

            else if (module is Sound sound) {
                sound.Out(oValue);
            }

            else if (module is Servo servo) {
                servo.Out(oValue);

            }
            else if (module is Neopixel neopixel) {
                neopixel.Out(oValue);

            }
            else if (module is I2cBus i2c) {
                i2c.Out(oValue);
            }
            else throw new Exception("Module is not supported.");
        }
        public static double OutIn(IOModule module, byte[] dataOut, byte[] dataIn) {
            if (module is I2cBus i2c) {
                return i2c.OutIn(dataOut, dataIn);
            }
            else
                throw new Exception("Module is not supported.");
        }


        internal static void Dispose(object module) {
            if (module is Analog analog) {
                analog.Dispose();
            }

            else if (module is Digital digitalIn) {
                digitalIn.Dispose();
            }


            else if (module is Sound sound) {
                sound.Dispose();
            }

            else if (module is Buttons buttons) {
                buttons.Dispose();
            }

            else if (module is Accel accel) {
                accel.Dispose();
            }
            else if (module is DisplayController display) {
                display.Dispose();
            }

            else if (module is Servo servo) {
                servo.Dispose();
            }
            else if (module is Neopixel neopixel) {
                neopixel.Dispose();
            }
            else if (module is I2cBus i2c) {
                i2c.Dispose();
            }

        }

        internal static void RegisterObject(object obj, int pinNum) => BrainPad.Modules[pinNum] = obj;


        internal static void UnRegisterObject(int pinNum) {
            if (BrainPad.Modules.Contains(pinNum)) {
                var module = BrainPad.Modules[pinNum];

                BrainPad.Dispose(module);

                BrainPad.Modules.Remove(module);

                ClearPinReserved(pinNum);
            }
        }
    }

}
