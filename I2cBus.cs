using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class I2cBus : IOModule {


        const int registerNum = 0xA1;

        private I2cDevice i2cDevice;
        private I2cController Controller;

        GpioPin sclPin;
        GpioPin sdaPin;
        public I2cBus(int address) {

            this.Initialize(address, -1, -1);
        }

        public I2cBus(int address, BrainPad.Pin sclPin, BrainPad.Pin sdaPin) {
            var scl = BrainPad.GetGpioFromBpPin(sclPin);
            var sda = BrainPad.GetGpioFromBpPin(sdaPin);

            this.Initialize(address, scl, sda);
        }

        public I2cBus(int address, string sclPin, string sdaPin) {

            var scl = BrainPad.GetGpioFromBpPin(sclPin);
            var sda = BrainPad.GetGpioFromBpPin(sdaPin);

            this.Initialize(address, scl, sda);
        }

        void Initialize(int address, int scl, int sda) {
            var addr = (byte)(address & 0xFF);

            BrainPad.UnRegisterObject(registerNum);

            if (scl != -1 && sda != -1) {
                this.sclPin = GpioController.GetDefault().OpenPin(scl);
                this.sdaPin = GpioController.GetDefault().OpenPin(sda);
                this.Controller = I2cController.FromName(SC13048.I2cBus.Software, this.sdaPin, this.sclPin);
            }
            else {
                this.Controller = I2cController.FromName(SC13048.I2cBus.I2c4);
            }

            var settings = new I2cConnectionSettings(addr, 100_000);
            this.i2cDevice = this.Controller.GetDevice(settings);

            BrainPad.RegisterObject(this, registerNum);
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
        public override double OutIn(double[] data, double[] result) {

            var bData = new byte[data.Length];
            var bResult = new byte[result.Length];

            for (var i = 0; i < bData.Length; i++) {
                bData[i] = (byte)(data[i]);
            }

            this.i2cDevice.WriteRead(bData, bResult);

            for (var i = 0; i < result.Length; i++) {
                result[i] = bResult[i];
            }

            return result.Length;
        }
    }
}