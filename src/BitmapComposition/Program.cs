using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BitmapComposition {
    class Program {
        static void Main(string[] args) {
            var path = "images";
            var files = Directory.GetFiles(path, "*.jpg").Select(Bitmap.FromFile).Select(x => (Bitmap)x).ToArray();

            files[0] = Processor.SetImageOpacity(files[0], 0.4f);

            var output = Processor.CreateCompositeBitmap(files);
            output.Save(Path.Combine(path, "output.png"));
        }
    }
}
