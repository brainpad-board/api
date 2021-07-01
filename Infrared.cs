using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Drivers.Infrared;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Infrared : IOModule {

        private GpioPin gpioPin;

        private NecIRDecoder infrared;

        private byte[] values;

        private int index = 0;

        const int MaxValueCount = 10;

        const int PRESS_CODE_NONE = 100;

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
                    return GetKeyInTableMap(this.values[--this.index]);
            }

            return PRESS_CODE_NONE;
        }

        static int GetKeyInTableMap(byte key) {
            switch (key) {
                case 0: // Power off
                    return 10;

                case 2: // Power on
                    return 11;

                case 1: // Up
                    return 12;

                case 9: // Down
                    return 13;

                case 4: // left
                    return 14;

                case 6: // right
                    return 15;

                case 5: // Center
                    return 16;

                case 8: // Back
                    return 17;

                case 10: // Next
                    return 18;

                case 12: // Plus
                    return 19;

                case 14: // Minus
                    return 20;

                case 13: // 0
                    return 0;

                case 16: // 1
                case 17: // 2
                case 18: // 3
                    return key - 15;

                case 20: // 4
                case 21: // 5
                case 22: // 6
                    return key - 16;

                case 24: // 7
                case 25: // 8
                case 26: // 9
                    return key - 17;
            }

            return PRESS_CODE_NONE;
        }
        public override void Dispose() {
            this.gpioPin?.Dispose();

            this.gpioPin = null;
        }
    }
}
