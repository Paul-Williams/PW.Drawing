using PW.IO.FileSystemObjects;
using System.Drawing;

namespace PW.Drawing.Tests;

[TestClass]
public class ImageHelperTests
{
  [TestMethod]
  public void Image_properties_match_Image_FromFile()
  {
    var file = new FilePath(  @"");

    var img1 = Image.FromFile(file.ToString());
    var img2 = ImageHelper.OpenWithoutFileLock(file);

    Assert.AreEqual(img1.HorizontalResolution, img2.HorizontalResolution);
    Assert.AreEqual(img1.VerticalResolution, img2.VerticalResolution);  
    Assert.AreEqual(img1.Width, img2.Width);  
    Assert.AreEqual(img1.Height, img2.Height);  
    Assert.AreEqual(img1.PixelFormat, img2.PixelFormat);

  }
}