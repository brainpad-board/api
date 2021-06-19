using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {
    public abstract class IOModule : IDisposable{
        public virtual double In() => 0;
        public virtual void Out(double data) {

        }

        public virtual void Out(double[] data) {

        }
        public virtual void Out(byte[] data) {

        }
        public virtual double OutIn(double[] data, double[] result) => 0;

        public virtual double OutIn(byte[] data, byte[] result) => 0;

        public virtual double In(byte[] result) => 0.0;
        public abstract void Dispose(bool disposing);

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
