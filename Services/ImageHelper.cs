using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GantsPlace.Services
{
    public static class ImageHelper
    {
        /// <summary>
        /// Charge une image depuis Images/ à côté de l'exe.
        /// Essaie .jpg puis .png si le nom n'a pas d'extension.
        /// Retourne null si introuvable.
        /// </summary>
        public static ImageBrush? LoadImageBrush(string? fileName,
            Stretch stretch = Stretch.UniformToFill)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            // Si pas d'extension, essayer jpg puis png
            var candidates = Path.HasExtension(fileName)
                ? new[] { fileName }
                : new[] { fileName + ".jpg", fileName + ".png" };

            foreach (var name in candidates)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", name);
                if (!File.Exists(path)) continue;
                try
                {
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource      = new Uri(path, UriKind.Absolute);
                    bmp.CacheOption    = BitmapCacheOption.OnLoad;
                    bmp.DecodePixelWidth = 400; // Limite mémoire
                    bmp.EndInit();
                    bmp.Freeze();
                    return new ImageBrush(bmp) { Stretch = stretch };
                }
                catch { /* Image corrompue → continuer */ }
            }
            return null;
        }

        /// <summary>Retourne une BitmapImage (pour ImageSource) plutôt qu'un ImageBrush.</summary>
        public static BitmapImage? LoadBitmap(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
            if (!File.Exists(path)) return null;
            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource   = new Uri(path, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch { return null; }
        }
    }
}
