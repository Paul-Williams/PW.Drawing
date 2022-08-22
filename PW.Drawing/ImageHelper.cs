namespace PW.Drawing;

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
  public static ImageCodecInfo? GetEncoderInfo(string mimeType) =>
    ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.MimeType == mimeType);

  public static ImageCodecInfo JpegImageCodecInfo { get; } =
    GetEncoderInfo("image/jpeg") ?? throw new Exception("GetEncoderInfo(\"image/jpeg\") failed.");

  /// <summary>
  /// Loads an image from file. Unlike <see cref="Image.FromFile(string)"/> it does not hold a lock on the file until the <see cref="Image"/> is disposed.
  /// This does not support WebP images, only GDI+ supported. It may be slower than <see cref="Image.FromFile(string)"/>.
  /// </summary>
  public static Image OpenWithoutFileLock(FilePath path)
  {
    // Purpose:
    // This method is a work-around for a common problem where loading an Image into a PictureBox
    // results in the file on disk staying locked even after the Image is removed from the PictureBox
    // and disposed. The picture file cannot then be deleted or moved until the application is closed.
    // Not sure if the fault is with Dispose() or whether the PictureBox has anything to do with it.

    // See: https://stackoverflow.com/questions/4803935/free-file-locked-by-new-bitmapfilepath/14837330#14837330

    // NB: Stream must not be disposed here. The returned image *may* require it later.
    // As such it is left to the GC to dispose the stream after the image is disposed.

    return Image.FromStream(new MemoryStream(File.ReadAllBytes(path.Path)));

  }


  /// <summary>
  /// Return either an <see cref="Image"/> or an <see cref="Exception"/>. See: <see cref="OpenWithoutFileLock(FilePath)"/>
  /// </summary>
  public static OneOf<Image, Exception> TryOpenImage(FilePath path)
  {
    try
    {
      return OpenWithoutFileLock(path);
    }
    catch (Exception ex)
    {
      return ex;
    }
  }

}
