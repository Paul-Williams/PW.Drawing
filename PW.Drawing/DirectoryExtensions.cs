namespace PW.IO;

public static class DirectoryExtensions
{

  /// <summary>
  /// Returns a list of files from the directory with image file extensions.
  /// </summary>
  public static IEnumerable<FileInfo> EnumerateGdiSupportedImages(this DirectoryInfo directory, SearchOption searchOption)
  {
    if (directory is null)
    {
      throw new ArgumentNullException(nameof(directory));
    }

    // TODO: Use - Enumerable.SelectMany(directory.EnumerateAuthorizedDirectories(true), x =>
    return directory.EnumerateFiles("*.*", searchOption).Where(fileInfo => GdiImageDecoderFormats.IsSupported(fileInfo.Extension));
  }

  /// <summary>
  /// Enumerates files from the directory with image file extensions.
  /// </summary>
  public static IEnumerable<FileInfo> EnumerateGdiSupportedImages(this DirectoryInfo directory)
    => EnumerateGdiSupportedImages(directory, SearchOption.TopDirectoryOnly);

  /// <summary>
  /// Returns a list of files from the directory with image file extensions.
  /// </summary>
  public static IEnumerable<FilePath> EnumerateGdiSupportedImages(this DirectoryPath directory, System.IO.SearchOption searchOption)
  {
    if (directory is null) throw new ArgumentNullException(nameof(directory));
    if (!directory.Exists) throw new DirectoryNotFoundException("Directory not found: " + directory.ToString());


    // TODO: Use - Enumerable.SelectMany(directory.EnumerateAuthorizedDirectories(true), x =>
    return directory.EnumerateFiles("*.*", searchOption).Where(file => GdiImageDecoderFormats.IsSupported(file.Extension.ToString()));
  }
}
