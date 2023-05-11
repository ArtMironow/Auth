using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Auth.Features
{
    public class CropImageFeature
    {
        public static string? CropImage(string? imageAsBase64, int size)
        {
            if (imageAsBase64 != null && imageAsBase64 != "")
            //if (String.IsNullOrEmpty(imageAsBase64))
            {
                var bytes = Convert.FromBase64String((imageAsBase64).Split(",")[1]);

                Image image;

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.Load(ms);
                }

                var format = Image.DetectFormat(bytes);

                int width, height;

                if (image != null)
                {
                    if (image.Width < size && image.Height < size)
                    {
                        return imageAsBase64;
                    }

                    if (image.Width > image.Height)
                    {
                        width = size;
                        height = Convert.ToInt32(image.Height * size / (double)image.Width);
                    }
                    else
                    {
                        width = Convert.ToInt32(image.Width * size / (double)image.Height);
                        height = size;
                    }

                    var clone = image.Clone(
                        i => i.Resize(width, height)
                            .Crop(new Rectangle(0, 0, width, height)));

                    if (clone != null)
                    {
                        imageAsBase64 = clone.ToBase64String(format);
                    }
                }

                return imageAsBase64;
            }

            return null;
        }        
    }
}
