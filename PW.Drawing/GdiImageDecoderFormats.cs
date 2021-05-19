using PW.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;

namespace PW.Drawing.Imaging
{
  /// <summary>
  /// A list of file extensions which represent image files, as supported by GDI.
  /// </summary>
  public static class GdiImageDecoderFormats
  {

    /// <summary>
    /// List of the file extensions supported by installed GDI+ decoders.
    /// </summary>
    public static IReadOnlyList<string> FileExtensions { get; }

    /// <summary>
    /// List of descriptions for installed GDI+ decoders.
    /// </summary>
    public static IEnumerable<string> Descriptions { get; }

    /// <summary>
    /// Static ctor for all instances.
    /// </summary>
    static GdiImageDecoderFormats()
    {
      var decoders = ImageCodecInfo.GetImageDecoders();

      // Added for .NET 5 upgrade -- ImageCodecInfo.GetImageDecoders now nullable.

      static string[] ParseFilenameExtensions(ImageCodecInfo x)
      {
        var ext = x.FilenameExtension;
        return ext is not null ? ext.RemoveAll('*').Split(';') : Array.Empty<string>();
      }

      if (decoders is not null)
      {
        FileExtensions = decoders.SelectMany(x => ParseFilenameExtensions(x))
          .OrderBy(x => x)
          .Distinct()
          .ToArray();

        Descriptions = decoders.Select(x => x.FormatDescription ?? "<No Description>").OrderBy(x => x);
      }
      else
      {
        FileExtensions = Array.Empty<string>();
        Descriptions = Array.Empty<string>();
      }


    }



    /// <summary>
    /// Returns true is <paramref name="fileExtension"/> is supported by gdi
    /// </summary>
    /// <param name="fileExtension"></param>
    /// <returns></returns>
    public static bool IsSupported(string fileExtension) => FileExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Returns a comma-separated string containing all extensions in the list
    /// </summary>
    /// <returns></returns>
    public static string All() => string.Join(",", FileExtensions);

  }
}