using System.Runtime.InteropServices;

namespace PW.Drawing;

// See: https://stackoverflow.com/questions/13660976/get-the-active-color-of-windows-8-automatic-color-theme
//      https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmgetcolorizationcolor

/// <summary>
/// Gives access to the Windows Glass accent color.
/// </summary>
public static class DesktopManagerColors
{
  private static class NativeMethods
  {
    [DllImport("dwmapi.dll", EntryPoint = "#127")]
    internal static extern void DwmGetColorizationParameters(ref DWMCOLORIZATIONPARAMS param);
  }

  private struct DWMCOLORIZATIONPARAMS
  {
    public uint ColorizationColor,
        ColorizationAfterglow,
        ColorizationColorBalance,
        ColorizationAfterglowBalance,
        ColorizationBlurBalance,
        ColorizationGlassReflectionIntensity,
        ColorizationOpaqueBlend;
  }

  /// <summary>
  /// Retrieves the current color used for Desktop Window Manager (DWM) glass composition. 
  /// </summary>
  /// <param name="opaque">true to ignore any transparency value.</param>
  /// <returns></returns>
  public static Color GetAccentColor(bool opaque)
  {

    var param = new DWMCOLORIZATIONPARAMS();
    NativeMethods.DwmGetColorizationParameters(ref param);

    return Color.FromArgb(
      (byte)(opaque ? 255 : param.ColorizationColor >> 24),
      (byte)(param.ColorizationColor >> 16),
      (byte)(param.ColorizationColor >> 8),
      (byte)param.ColorizationColor
      );
  }
}
