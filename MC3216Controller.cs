using GHIElectronics.TinyCLR.Devices.I2c;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    internal class MC3216Controller {
#if !DEBUG
        const int MC3216_Mode = 0x07;
        const int MC3216_Opstat = 0x04;
        const int MC3216_Outcfg = 0x20;
        const int MC3216_XOut = 0x0D;
        const int MC3216_YOut = 0x0F;
        const int MC3216_ZOut = 0x11;
        const int MC3216_SRTFR = 0x08;
        private readonly I2cDevice i2cDevice;
#endif
        public sbyte X => this.GetX();
        public sbyte Y => this.GetY();
        public sbyte Z => this.GetZ();

        public MC3216Controller(I2cController i2cController) {
#if !DEBUG
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
#endif
        }
#if !DEBUG
        bool disposed = false;
#endif
        internal void Dispose() {
#if !DEBUG
            if (!this.disposed)
                this.i2cDevice.Dispose();

            this.disposed = true;
#endif
        }

        private void WriteToRegister(byte reg, byte[] value) {
#if !DEBUG
            var count = value.Length + 1;

            var write = new byte[count];

            write[0] = reg;

            Array.Copy(value, 0, write, 1, value.Length);

            this.i2cDevice.Write(write, 0, write.Length);
#endif
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "<Pending>")]
        private byte[] ReadFromRegister(byte reg, int count) {
#if !DEBUG
            var writeData = new byte[1] { reg };
            var readData = new byte[count];

            this.i2cDevice.WriteRead(writeData, readData);

            return readData;
#else
            return null;
#endif
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "<Pending>")]
        private sbyte GetX() {
#if !DEBUG
            var reg = this.ReadFromRegister(MC3216_XOut, 1);
            return (sbyte)reg[0];
#else
            return 0;
#endif
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "<Pending>")]
        private sbyte GetY() {
#if !DEBUG
            var reg = this.ReadFromRegister(MC3216_YOut, 1);
            return (sbyte)reg[0];
#else
            return 0;
#endif
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Use expression body for methods", Justification = "<Pending>")]
        private sbyte GetZ() {
#if !DEBUG
            var reg = this.ReadFromRegister(MC3216_ZOut, 1);
            return (sbyte)reg[0];
#else
            return 0;
#endif
        }
    }
}
