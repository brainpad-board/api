using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace GHIElectronics.TinyCLR.Drivers.BrainPadController {   
    public class Image {
        public enum Transform {
            None,
            FlipHorizontal,
            FlipVertical,
            Rotate90,
            Rotate180,
            Rotate270,
        }
        public int Height { get; internal set; }
        public int Width { get; internal set; }
        public byte[] Data { get; internal set; }

        public Image(string img, int width, int height) : this(img, width, height, 1, 1, Transform.None) { }
        public Image(string img, int width, int height, int hScale, int vScale, Transform transform) {
            var data = Encoding.UTF8.GetBytes(img);

            for (var x = 0; x < data.Length; x++) {
                if (data[x] == ' ') {
                    data[x] = 0;
                }
            }

            this.CreateImage(data, width, height, hScale, vScale, transform);
        }
        public Image(byte[] data, int width, int height) : this(data, width, height, 1, 1, Transform.None) { }

        public Image(byte[] data, int width, int height, int hScale, int vScale, Transform transform) => this.CreateImage(data, width, height, hScale, vScale, transform);

        private void CreateImage(byte[] data, int width, int height, int hScale, int vScale, Transform transform) {

            if (width * height != data.Length) throw new Exception("Incorrect image data size");

            this.Height = height * vScale;
            this.Width = width * hScale;

            this.Data = new byte[this.Width * this.Height];

            for (var x = 0; x < this.Width; x++) {
                for (var y = 0; y < this.Height; y++) {
                    switch (transform) {
                        case Transform.None:
                            this.Data[y * this.Width + x] = data[y / vScale * width + x / hScale];
                            break;
                        case Transform.FlipHorizontal:
                            this.Data[y * this.Width + (this.Width - x - 1)] = data[y / vScale * width + x / hScale];
                            break;
                        case Transform.FlipVertical:
                            this.Data[(this.Height - y - 1) * this.Width + x] = data[y / vScale * width + x / hScale];
                            break;
                        case Transform.Rotate90:
                            this.Data[x * this.Height + this.Height - y - 1] = data[y / vScale * width + x / hScale];
                            break;
                        case Transform.Rotate180:
                            this.Data[(this.Height - y - 1) * this.Width + (this.Width - x - 1)] = data[y / vScale * width + x / hScale];
                            break;
                        case Transform.Rotate270:

                            this.Data[(this.Width - x - 1) * this.Height + y] = data[y / vScale * width + x / hScale];
                            break;
                    }
                }
            }
            if (transform == Transform.Rotate90 || transform == Transform.Rotate270) {
                var temp = this.Width;
                this.Width = this.Height;
                this.Height = temp;
            }
        }
    }
}
