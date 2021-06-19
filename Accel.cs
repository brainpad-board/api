using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public class Accel : IOModule {
        MC3216Controller accel;

        const int RegisterNum = 0x80;

        bool doX = false;
        bool doY = false;
        bool doZ = false;
        public Accel(string xyz) {
            switch (xyz) {
                case "X":
                case "x":
                    this.doX = true;
                    break;
                case "Y":
                case "y":
                    this.doY = true;
                    break;
                case "Z":
                case "z":
                    this.doZ = true;
                    break;

            }

            if (this.doX == false && this.doY == false && this.doZ == false)
                throw new ArgumentException("Argument must be X or Y or Z");

            BrainPad.UnRegisterObject(RegisterNum);

            if (BrainPad.Type.IsPulse) {
                var i2ccon = I2cController.FromName(SC13048.I2cBus.I2c2);
                this.accel = new MC3216Controller(i2ccon);
            }

            BrainPad.RegisterObject(this, RegisterNum);
        }

        private double GetX() {
            if (BrainPad.Type.IsPulse) {
                var x = (double)this.accel.X;

                return (x + 128) / 256;
            }
            return 0;
        }
        private double GetY() {
            if (BrainPad.Type.IsPulse) {
                var y = (double)this.accel.Y;

                return (y + 128) / 256;
            }
            return 0;
        }
        private double GetZ() {
            if (BrainPad.Type.IsPulse) {
                var z = (double)this.accel.Z;

                return (z + 128) / 256;
            }
            return 0;
        }

        public override double In() {
            if (this.doX)
                return this.GetX();
            if (this.doY)
                return this.GetY();

            return this.GetZ();
        }

        public override void Dispose(bool disposing) {
            if (disposing)
                this.accel?.Dispose();

            this.accel = null;
        }

    }
}
