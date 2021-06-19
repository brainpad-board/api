using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class I2cBus : IOModule {


        const int RegisterNum = 0xA1;

        private I2cDevice i2cDevice;
        private I2cController controller;

        GpioPin sclPin;
        GpioPin sdaPin;
        public I2cBus(int address) => this.Initialize(address, -1, -1);
        public I2cBus(int address, string sclPin, string sdaPin) {

            var scl = BrainPad.GetGpioFromString(sclPin);
            var sda = BrainPad.GetGpioFromString(sdaPin);

            this.Initialize(address, scl, sda);
        }

        void Initialize(int address, int scl, int sda) {
            var addr = (byte)(address & 0xFF);

            BrainPad.UnRegisterObject(RegisterNum);

            if (scl != -1 && sda != -1) {
                this.sclPin = GpioController.GetDefault().OpenPin(scl);
                this.sdaPin = GpioController.GetDefault().OpenPin(sda);
                this.controller = I2cController.FromName(SC13048.I2cBus.Software, this.sdaPin, this.sclPin);
            }
            else {
                this.controller = I2cController.FromName(SC13048.I2cBus.I2c4);
            }

            var settings = new I2cConnectionSettings(addr, 100_000);
            this.i2cDevice = this.controller.GetDevice(settings);

            BrainPad.RegisterObject(this, RegisterNum);
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


        public override void Dispose(bool disposing) {
            if (disposing) {
                this.i2cDevice?.Dispose();
                this.sdaPin?.Dispose();
                this.sdaPin?.Dispose();
            }

            this.i2cDevice = null;
            this.sdaPin = null;
            this.sdaPin = null;



        }

        public override double OutIn(byte[] data, byte[] result) {
            this.i2cDevice.WriteRead(data, result);

            return 0;
        }
    }
}
