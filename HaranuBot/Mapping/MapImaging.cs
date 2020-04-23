using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace HaranuBot.Mapping
{
    public class MapImaging
    {
        public static readonly string imageFile = "resources/image.png";

        public static bool GetMap(string map)
        {
            try
            {
                using (Image<Rgba32> image = Image.Load("resources/" + map + ".png"))
                {
                    double scaleFactor = 0.4;
                    int width = (int)(image.Width * scaleFactor);
                    int height = (int)(image.Height * scaleFactor);
                    image.Mutate(ctx => ctx.Resize(width, height));
                    image.Save(imageFile);
                }

                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public static void CreateMap(Location location)
        {
            using (Image<Rgba32> image = Image.Load("resources/" + location.Map + ".png"))
            {
                image.Mutate(c => c.Crop(new Rectangle(location.X - (location.Width / 2), location.Y - (location.Height / 2), location.Width, location.Height)));
                image.Save(imageFile);
            }
        }
    }
}
