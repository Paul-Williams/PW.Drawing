using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PW.Drawing
{
  public static class BitmapExtensions
  {
    /// <summary>
    /// Image is landscape ratio.
    /// </summary>
    public static bool IsLandscape(this Bitmap img) => img.Width > img.Height;

    /// <summary>
    /// Image is portrait ratio.
    /// </summary>
    public static bool IsPortrait(this Bitmap img) => img.Height > img.Width;

    /// <summary>
    /// Image X & Y length match.
    /// </summary>
    public static bool IsSquare(this Bitmap img) => img.Height == img.Width;

    /// <summary>
    /// Returns the bitmap and it's HD size scaled with aspect ratio.
    /// </summary>
    public static (Bitmap Bitmap, Size Size) GetHDSize(this Bitmap image)
    {
      if (image is null) throw new ArgumentNullException(nameof(image));

      if (image.Size == HD) return (image, image.Size);


      // Get the image's original width and height
      int originalWidth = image.Width;
      int originalHeight = image.Height;

      // To preserve the aspect ratio
      float ratioX = (float)HD.Width / originalWidth;
      float ratioY = (float)HD.Height / originalHeight;
      float ratio = Math.Min(ratioX, ratioY);

      // New width and height based on aspect ratio
      int newWidth = (int)(originalWidth * ratio);
      int newHeight = (int)(originalHeight * ratio);

      return (image, new Size(newWidth, newHeight));
    }

    /// <summary>
    /// Resize the image to the specified size.
    /// </summary>
    public static Bitmap ResizeTo(this (Bitmap image, Size size) t) => t.image.Resize(t.size);

    /// <summary>
    /// Resize the image to the specified size.
    /// </summary>
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

    /// <summary>
    /// Saves the image at 95% high quality to the specified path.
    /// </summary>
    public static void SaveHQ(this Bitmap bitmap, string path) => bitmap.SaveHQ(path, 95L);

    /// <summary>
    /// Saves the image at high quality to the specified path with the specified 0-100 (low-high) quality.
    /// </summary>
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

    /// <summary>
    /// HD image size.
    /// </summary>
    public static Size HD { get; } = new Size(1920, 1080);


    /// <summary>
    /// The color-matrix needed to grey-scale an image.
    /// Source: http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
    /// </summary>
    static readonly ColorMatrix _colorMatrix = new(new float[][]
    {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
    });

    /// <summary>
    /// Gets the lightness of the image in 256 sections (16x16)
    /// http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
    /// </summary>
    /// <param name="original">The image to get the lightness for</param>
    /// <returns>A double-array (16x16) containing the lightness of the 256 sections</returns>
    public static Image ToGreyScale(this Image original)
    {
      if (original is null)
      {
        throw new ArgumentNullException(nameof(original));
      }

      Bitmap? newBitmap = null;
      try
      {
        newBitmap = new Bitmap(original.Width, original.Height);
        using (Graphics g = Graphics.FromImage(newBitmap))
        using (ImageAttributes attributes = new())
        {
          attributes.SetColorMatrix(_colorMatrix);
          g.DrawImage(
            original,
            new Rectangle(0, 0, original.Width, original.Height),
            0,
            0,
            original.Width,
            original.Height,
            GraphicsUnit.Pixel,
            attributes
            );
        }
        return newBitmap;
      }
      catch (Exception)
      {
        newBitmap?.Dispose();
        throw;
      }
    }

  }
}
