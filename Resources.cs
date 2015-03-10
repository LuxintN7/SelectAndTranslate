using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace SelectAndTranslate
{
    public static class Resources
    {
        static Resources()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SelectAndTranslate.img.logo32.png"))
            {
                Logo32Bitmap = new Bitmap(stream);

                stream.Seek(0, SeekOrigin.Begin);

                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                Logo32BitmapSource = image;
            }
        }

        public static readonly Bitmap Logo32Bitmap;
        public static readonly BitmapSource Logo32BitmapSource;
    }
}
