using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    public static class BrainPadType {
        public static bool IsPulse { get; } = false;
        static BrainPadType() {
            using (var pb15 = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB15)) {
                pb15.SetDriveMode(GpioPinDriveMode.InputPullDown);
                IsPulse = pb15.Read() == GpioPinValue.High;
            }
        }
    }
}
