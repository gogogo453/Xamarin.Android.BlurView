using System.Drawing;

namespace Xamarin.Android.BlurView
{
    public class SizeScaler
    {
        private const int ROUNDING_VALUE = 64;
        private readonly float _scaleFactor;

        public SizeScaler(float scaleFactor)
        {
            _scaleFactor = scaleFactor;
        }

        public Size Scale(int width, int height)
        {
            int nonRoundedScaledWidth = GetDownscaleSize(width);
            int scaleWidth = RoundSize(nonRoundedScaledWidth);

            float roundingScaleFactor = (float)width / scaleWidth;
            int scaledHeight = (int)Java.Lang.Math.Ceil(height / roundingScaleFactor);

            return new Size((int)scaleWidth, scaledHeight);
        }

        public bool IsZeroSized(int measuredWidth, int measuredHeight) =>
            GetDownscaleSize(measuredHeight) == 0 || GetDownscaleSize(measuredWidth) == 0;

        private int GetDownscaleSize(float value) =>
            (int)Java.Lang.Math.Ceil(value / _scaleFactor);

        private static int RoundSize(int value)
        {
            return value % ROUNDING_VALUE != 0
                ? value - (value % ROUNDING_VALUE) + ROUNDING_VALUE
                : value;
        }
    }
}
