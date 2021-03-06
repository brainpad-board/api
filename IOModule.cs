using System;

namespace BrainPad {
    public abstract class IOModule : IDisposable {
        public virtual double In() => 0;
        public virtual void Out(double data) { }
        public virtual void Out(double[] data) { }
        public virtual double OutIn(byte[] data, byte[] result) => 0;
        public abstract void Dispose();
    }
}
