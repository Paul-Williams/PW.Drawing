using OneOf;
using PW.IO.FileSystemObjects;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace PW.Drawing
{
  public static partial class ImageHelper
  {
    /// <summary>
    /// Creates a new <see cref="EncoderParameters"/> with the specified compression.
    /// </summary>
    /// <param name="compression"><see cref="CompressionBounds.Min"/>-<see cref="CompressionBounds.Max"/></param>
    public static EncoderParameters QualityEncoderParameters(long compression)
    {
      var encoderParameters = new EncoderParameters(1);
      encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, compression);
      return encoderParameters;
    }


    // See: https://efundies.com/csharp-save-jpg/
    public static ImageCodecInfo? GetEncoderInfo(String mimeType)
    {
      int j;
      ImageCodecInfo[] encoders;
      encoders = ImageCodecInfo.GetImageEncoders();
      for (j = 0; j < encoders.Length; ++j)
      {
        if (encoders[j].MimeType == mimeType)
          return encoders[j];
      }
      return null;
    }

    public static ImageCodecInfo JpegImageCodecInfo { get; } = GetEncoderInfo("image/jpeg") ?? throw new Exception("GetEncoderInfo(\"image/jpeg\") failed.");

    /// <summary>
    /// Loads an image from file. Unlike <see cref="Image.FromFile(string)"/> it does not hold a lock on the file until the <see cref="Image"/> is disposed.
    /// This does not support Vector images, only Bitmaps.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Image ImageFromFile(FilePath path)
    {
      // Purpose:
      // This method is a work-around for a common problem where loading an Image into a PictureBox
      // results in the file on disk staying locked even after the Image is removed from the PictureBox
      // and disposed. The picture file cannot then be deleted or moved until the application is closed.
      // Not sure if the fault is with Dispose() or whether the PictureBox has anything to do with it.

      // Usage:
      // Have tested the following way of using this method and successfully been able to delete all images
      // without closing the caller application.

      // Example:
      // using (PictureBox.Image) { PictureBox.Image = ImageHelper.ImageFromFile(filePath);


      // Logic:
      // The first bitmap created from file will hold a lock until disposed.
      // A second bitmap is created from the first one, and does not know about the file.
      // In this way the file is not locked for ages!
      using var tmp = new Bitmap(path.Value);
      return new Bitmap(tmp);

    }

    /// <summary>
    /// Return either an bitmap or an exception. See: <see cref="ImageFromFile(FilePath)"/>
    /// </summary>
    public static OneOf<Bitmap, Exception> TryOpenBitmap(FilePath path)
    {
      try
      {
        return (Bitmap)ImageFromFile(path);
      }
      catch (Exception ex)
      {
        return ex;
      }
    }


    /// <summary>
    /// Return either an bitmap or an exception. See: <see cref="ImageFromFile(FilePath)"/>
    /// </summary>
    public static OneOf<Image, Exception> TryOpenImage(FilePath path)
    {
      try
      {
        return ImageFromFile(path);
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

  }
}
