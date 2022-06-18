using System;

namespace PW.Drawing
{

  //internal enum CompressionBounds : long { Min = 1, Max = 100 }

  public struct ImageCompression
  {
    public const long Min = 1;
    public const long Max = 100;
    public const long Default = 95;

    public ImageCompression() => Value = Default;

    public ImageCompression(long value)
    {
      if (value is < Min or > Max) 
        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} is {value}. Must be between {Min} and {Max}.");
      
      Value = value;
    }

    public long Value { get; }

    public static implicit operator long(ImageCompression imageCompression) => imageCompression.Value; 

  }


}
