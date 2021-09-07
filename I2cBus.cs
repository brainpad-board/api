using System;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    public class I2cBus : IOModule {

        private I2cDevice i2cDevice;
        private I2cController controller;

        public I2cBus(int address) => this.Initialize(address);

        void Initialize(int address) {
            var addr = (byte)(address & 0xFF);

            this.controller = I2cController.FromName(SC13048.I2cBus.I2c4);

            var settings = new I2cConnectionSettings(addr, 100_000);
            this.i2cDevice = this.controller.GetDevice(settings);
        }

        public override void Out(double data) => this.i2cDevice.Write(new byte[] { (byte)data });

        public override void Out(double[] data) {
            var bData = new byte[data.Length];

            for (var i = 0; i < bData.Length; i++) {
                bData[i] = (byte)(data[i]);
            }
            this.i2cDevice.Write(bData);
        }

        public override double In() {
            var readBuffer = new byte[1];
            this.i2cDevice.Read(readBuffer);
            return (int)readBuffer[0];
        }


        public override void Dispose() {
            this.i2cDevice?.Dispose();

            this.i2cDevice = null;
        }

        public override double OutIn(byte[] write, byte[] read) {
            if (write != null && write.Length != 0 && read !=null && read.Length != 0)    
                this.i2cDevice.WriteRead(write, read);
            else if (write != null && write.Length != 0)
                this.i2cDevice.Write(write);
            else
                this.i2cDevice.Read(read);

            return 0;
        }
    }
}
