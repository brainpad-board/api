using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.Infrared;

namespace BrainPad {
    public class Infrared : IOModule {

        private GpioPin gpioPin;

        private NecIRDecoder infrared;

        private byte[] values;

        private int index = 0;

        const int MaxValueCount = 10;

        const int PRESS_CODE_NONE = 100;

        object locker;

        static readonly byte[] keyMapTable = { 10, 12, 11, 0xFF, 14, 16, 15, 0xFF, 17, 13, 18, 0xFF, 19, 0, 20, 0xFF, 1, 2, 3, 0xFF, 4, 5, 6, 0xFF, 7, 8, 9 };
        public Infrared(string pinBp) {
            var pinNum = Controller.GetGpioFromString(pinBp);

            this.gpioPin = Controller.Gpio.OpenPin(pinNum);

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
                    return keyMapTable[this.values[--this.index]];
            }

            return PRESS_CODE_NONE;
        }
       
        public override void Dispose() {
            this.gpioPin?.Dispose();

            this.gpioPin = null;
        }
    }
}
