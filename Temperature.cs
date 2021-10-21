using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Adc;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    class Temperature : IOModule {
        private uint unit;
        private AdcChannel adcChannel;
        public Temperature(double unit) {
            if (unit != Controller.Celsius && unit != Controller.Fahrenheit) {
                throw new ArgumentException("unit must be Celsius or Fahrenheit.");
            }

            this.Initialize(unit);
        }

        private void Initialize(double mode) {

            var enable_reg = (IntPtr)(0x40000000U + 0x08000000U + 0x08040300U + 8);
            var enable_val = Marshal.ReadInt32(enable_reg);
            enable_val |= (1 << 23);

            Marshal.WriteInt32(enable_reg, enable_val);

            var controller = AdcController.FromName(SC13048.Adc.Controller1.Id);
            this.adcChannel = controller.OpenChannel(SC13048.Adc.Controller1.InternalTemperatureSensor);

            this.unit = (uint)mode;
        }

        public override double In() {

            var v = this.adcChannel.ReadValue() * 1.0;

            var ts1 = Marshal.ReadInt32((IntPtr)0x1FFF75A8);
            var ts2 = Marshal.ReadInt32((IntPtr)0x1FFF75CA);

            ts1 &= 0xFFFF;
            ts2 &= 0xFFFF;
            v *= 1.1;

            var t1 = (110 - 30) * 1.0;
            var t2 = (ts2 - ts1) * 1.0;
            var t3 = (v - ts1) * 1.0;

            var temperature = t1 / t2 * t3 + 30;

            if (this.unit == Controller.Celsius)
                return temperature;

            return (temperature * 18) / 10 + 32;
        }

        public override void Dispose() {
            this.adcChannel?.Dispose();
            this.adcChannel = null;
        }
    }
}
