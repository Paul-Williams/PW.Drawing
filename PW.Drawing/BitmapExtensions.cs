using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PW.Drawing
{
  public static class BitmapExtensions
  {

    public static (Bitmap Bitmap, Size Size) GetHDSize(this Bitmap image)
    {
      if (image is null)
      {
        throw new ArgumentNullException(nameof(image));
      }

      var h = image.Size.Height;
      var w = image.Size.Width;

      // Get the image's original width and height
      int originalWidth = image.Width;
      int originalHeight = image.Height;

      // To preserve the aspect ratio
      float ratioX = (float)HD.Width / originalWidth;
      float ratioY = (float)HD.Height / originalHeight;
      float ratio = Math.Min(ratioX, ratioY);

      float sourceRatio = (float)originalWidth / originalHeight;

      // New width and height based on aspect ratio
      int newWidth = (int)(originalWidth * ratio);
      int newHeight = (int)(originalHeight * ratio);


      return (image, new Size(newWidth, newHeight));

      //// Portrait
      //if (image.Size.Height > image.Size.Width)
      //{
      //  var ratio = h / (decimal)w;

      //  return (image, new Size(1920, Convert.ToInt32(1080 * ratio)));

      //}

      //// Landscape
      //else if (image.Size.Width > image.Size.Height)
      //{
      //  var ratio = w / (decimal)h;
      //  return (image, new Size(Convert.ToInt32(1920 * ratio), 1080));
      //}

      ////Square
      //else
      //{
      //  return (image, new Size(1080, 1080));
      //}


    }

    public static Bitmap ResizeTo(this (Bitmap image, Size size) t) => t.image.Resize(t.size);

    public static Bitmap Resize(this Bitmap image, Size size) => image.Resize(size.Width, size.Height);


    /// <summary>
    /// Resize the image to the specified width and height.
    /// </summary>
    public static Bitmap Resize(this Bitmap image, int width, int height)
    {
      // See: https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp

      var destRect = new Rectangle(0, 0, width, height);
      var destImage = new Bitmap(width, height);

      destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

      using (var graphics = Graphics.FromImage(destImage))
      {
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using var wrapMode = new ImageAttributes();
        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
      }

      return destImage;
    }

    public static void SaveHQ(this Bitmap bitmap, string path) => bitmap.SaveHQ(path, 95L);

    public static void SaveHQ(this Bitmap bitmap, string path, long compression)
    {
      if (bitmap is null) throw new ArgumentNullException(nameof(bitmap));
      if (string.IsNullOrEmpty(path)) throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));


      var imageCodecInfo = ImageHelper.GetEncoderInfo("image/jpeg");

      if (imageCodecInfo is null) throw new Exception("Unable to get imageCodecInfo for: image/jpeg");

      var encoderParameters = new EncoderParameters(1);
      encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, compression);

      bitmap.Save(path, imageCodecInfo, encoderParameters);
    }


    public static bool IsBiggerThanHD(this Image image) =>
      image is not null ? image.Size.Height > HD.Height || image.Size.Width > HD.Width
      : throw new ArgumentNullException(nameof(image));


    public static Size HD { get; } = new Size(1920, 1080);

  }
}
