using System;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Pins;

namespace BrainPad {
    public class Accel : IOModule {
        MC3216Controller accel;

        bool doX = false;
        bool doY = false;
        bool doZ = false;
        //public Accel(string xyz) {
        //    xyz = xyz.ToLower();
        //    switch (xyz) {
        //        case "x":
        //            this.doX = true;
        //            break;
        //        case "y":
        //            this.doY = true;
        //            break;
        //        case "z":
        //            this.doZ = true;
        //            break;

        //    }

        //    if (this.doX == false && this.doY == false && this.doZ == false)
        //        throw new ArgumentException("Argument must be X or Y or Z");

        //    if (Controller.IsPulse) {
        //        var i2ccon = I2cController.FromName(SC13048.I2cBus.I2c2);
        //        this.accel = new MC3216Controller(i2ccon);
        //    }
        //}

        public Accel(double xyz) {            
            switch ((int)xyz) {
                case Controller.X:
                    this.doX = true;
                    break;
                case Controller.Y:
                    this.doY = true;
                    break;
                case Controller.Z:
                    this.doZ = true;
                    break;

            }

            if (this.doX == false && this.doY == false && this.doZ == false)
                throw new ArgumentException("Argument must be X or Y or Z");

            if (Controller.IsPulse) {
                var i2ccon = I2cController.FromName(SC13048.I2cBus.I2c2);
                this.accel = new MC3216Controller(i2ccon);
            }
        }

        private double GetX() {
            if (Controller.IsPulse) {
                // Default 2g, range [-200..200], 8 bit ( 7 bit data + 1 bit sign) 
                // 1.56 = 200 / 128
                var d = (int)(this.accel.X * 1.56); 

                return d;
            }
            return 0;
        }
        private double GetY() {
            if (Controller.IsPulse) {                
                var d = (int)(this.accel.Y * 1.56);

                return d;
            }
            return 0;
        }
        private double GetZ() {
            if (Controller.IsPulse) {
                var d = (int)(this.accel.Z * 1.56);

                return d;
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

        public override void Dispose() {
            this.accel?.Dispose();
            this.accel = null;
        }

    }
}
