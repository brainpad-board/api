using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.Infrared;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Infrared : IOModule {
        private GpioPin gpioPin;

        private NecIRDecoder infrared;

        private byte[] values;

        private int index = 0;

        const int MaxValueCount = 10;

        object locker;
        public Infrared(string pinBp) {
            var pinNum = BrainPad.GetGpioFromString(pinBp);

            this.gpioPin = BrainPad.Gpio.OpenPin(pinNum);

            this.infrared = new NecIRDecoder(this.gpioPin);

            this.values = new byte[MaxValueCount];

            this.locker = new object();

            this.infrared.OnDataReceivedEvent += this.Infrared_OnDataReceivedEvent;
        }

        private void Infrared_OnDataReceivedEvent(byte address, byte command) {
            lock (this.locker) {
                if (this.index < MaxValueCount)
                    this.values[this.index++] = command;
            }

        }

        public override double In() {
            lock (this.locker) {
                if (this.index > 0)
                    return this.values[--this.index];
            }

            return 255;
        }
        public override void Dispose() {
            this.gpioPin?.Dispose();

            this.gpioPin = null;
        }
    }
}
