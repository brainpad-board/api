using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class BrainPadType {
        public bool IsPulse { get; } = false;
        public BrainPadType() {
            var pb15 = GpioController.GetDefault().OpenPin(SC13048.GpioPin.PB15);

            pb15.SetDriveMode(GpioPinDriveMode.InputPullDown);

            if (pb15.Read() == GpioPinValue.High)
                this.IsPulse = true;

            pb15.Dispose();
        }
    }
}
