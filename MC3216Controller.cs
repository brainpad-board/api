using System;
using GHIElectronics.TinyCLR.Devices.I2c;

namespace BrainPad {
    internal class MC3216Controller {
        const int MC3216_Mode = 0x07;
        const int MC3216_Opstat = 0x04;
        const int MC3216_Outcfg = 0x20;
        const int MC3216_XOut = 0x0D;
        const int MC3216_YOut = 0x0F;
        const int MC3216_ZOut = 0x11;
        const int MC3216_SRTFR = 0x08;
        private readonly I2cDevice i2cDevice;
        public sbyte X => this.GetX();
        public sbyte Y => this.GetY();
        public sbyte Z => this.GetZ();

        public MC3216Controller(I2cController i2cController) {
            var setting = new I2cConnectionSettings(0x4C) {
                BusSpeed = 400_000,
                AddressFormat = I2cAddressFormat.SevenBit,
            };

            this.i2cDevice = i2cController.GetDevice(setting);

            this.WriteToRegister(MC3216_Outcfg, new byte[1] { 2 });//8bit - 2g range
            //this.WriteToRegister(MC3216_SRTFR, new byte[1] { 5 });// 256hz

            this.WriteToRegister(MC3216_Mode, new byte[1] { 1 });// wake


            var id = this.ReadFromRegister(MC3216_Opstat, 1);//is it awake?

            if ((id[0] & 0x01) != 0x01) {
                throw new InvalidOperationException("Unexpected init!");
            }
        }
        bool disposed = false;
        internal void Dispose() {
            if (!this.disposed)
                this.i2cDevice.Dispose();

            this.disposed = true;
        }

        private void WriteToRegister(byte reg, byte[] value) {
            var count = value.Length + 1;

            var write = new byte[count];

            write[0] = reg;

            Array.Copy(value, 0, write, 1, value.Length);

            this.i2cDevice.Write(write, 0, write.Length);
        }

        private byte[] ReadFromRegister(byte reg, int count) {
            var writeData = new byte[1] { reg };
            var readData = new byte[count];

            this.i2cDevice.WriteRead(writeData, readData);

            return readData;
        }

        private sbyte GetX() {
            var reg = this.ReadFromRegister(MC3216_XOut, 1);
            return (sbyte)reg[0];
        }

        private sbyte GetY() {
            var reg = this.ReadFromRegister(MC3216_YOut, 1);
            return (sbyte)reg[0];
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "<Pending>")]
        private sbyte GetZ() {
            var reg = this.ReadFromRegister(MC3216_ZOut, 1);
            return (sbyte)reg[0];
        }
    }
}
