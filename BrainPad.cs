using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public static class BrainPad {
        internal const string TEXT_BUILTIN = "builtin";
        public enum Pin {
            P0,
            P1,
            P2,
            P3,
            P4,
            P5,
            P6,
            P7,
            P8,
            P9,
            P10,
            P11,
            P12,
            P13,
            P14,
            P15,
            P16
        }
        public enum Button {
            A = SC13048.GpioPin.PC13,
            B = SC13048.GpioPin.PB7,
        }

        internal static BrainPadType Type = new BrainPadType();

        public static Hashtable Modules = new Hashtable();

        private static Display display;
        //public static int GetGpioFromBpPin(Pin pin) {
        //    switch (pin) {
        //        case Pin.P0:
        //            return SC13048.GpioPin.PA5;

        //        case Pin.P1:
        //            return SC13048.GpioPin.PA3;

        //        case Pin.P2:
        //            return SC13048.GpioPin.PA2;

        //        case Pin.P3:
        //            return SC13048.GpioPin.PA1;

        //        case Pin.P4:
        //            return SC13048.GpioPin.PA0;

        //        case Pin.P5:
        //            return SC13048.GpioPin.PA7;

        //        case Pin.P6:
        //            return SC13048.GpioPin.PA4;

        //        case Pin.P7:
        //            return SC13048.GpioPin.PB0;

        //        case Pin.P8:
        //            return SC13048.GpioPin.PA9;

        //        case Pin.P9:
        //            return SC13048.GpioPin.PB1;

        //        case Pin.P10:
        //            return SC13048.GpioPin.PA6;

        //        case Pin.P11:
        //            return SC13048.GpioPin.PB6;

        //        case Pin.P12:
        //            return SC13048.GpioPin.PA10;

        //        case Pin.P13:
        //            return SC13048.GpioPin.PB3;

        //        case Pin.P14:
        //            return SC13048.GpioPin.PB4;

        //        case Pin.P15:
        //            return SC13048.GpioPin.PB5;

        //        case Pin.P16:
        //            return SC13048.GpioPin.PB12;
        //    }

        //    return -1;

        //}

        public static int GetGpioFromBpPin(string pin) {
            pin = pin.ToLower();

            switch (pin) {
                case "p0":
                    return SC13048.GpioPin.PA5;

                case "p1":
                    return SC13048.GpioPin.PA3;

                case "p2":
                    return SC13048.GpioPin.PA2;

                case "p3":
                    return SC13048.GpioPin.PA1;

                case "p4":
                    return SC13048.GpioPin.PA0;

                case "p5":
                    return SC13048.GpioPin.PA7;

                case "p6":
                    return SC13048.GpioPin.PA4;

                case "p7":
                    return SC13048.GpioPin.PB0;

                case "p8":
                    return SC13048.GpioPin.PA9;

                case "p9":
                    return SC13048.GpioPin.PB1;

                case "p10":
                    return SC13048.GpioPin.PA6;

                case "p11":
                    return SC13048.GpioPin.PB6;

                case "p12":
                    return SC13048.GpioPin.PA10;

                case "p13":
                    return SC13048.GpioPin.PB3;

                case "p14":
                    return SC13048.GpioPin.PB4;

                case "p15":
                    return SC13048.GpioPin.PB5;

                case "p16":
                    return SC13048.GpioPin.PB12;
            }

            return -1;

        }

        public static bool IsPulse() => BrainPad.Type.IsPulse == true ? true : false;

        public static void Wait(double seconds) {
            var t = (int)(seconds * 1000);
            Thread.Sleep(t);
        }
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

        public static void Print(string text) {
            if (display == null)
                display = new Display();

            display.Print(text);

        }

        public static void Clear() {
            if (display == null)
                display = new Display();

            display.Clear();

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
            else if (module is Display display) {
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
            }
        }
    }

}
