using Android.Graphics;

namespace Xamarin.Android.BlurView.Interfaces
{
    public interface IBlurAlgorithm
    {
        Bitmap Blur(Bitmap bitmap, float blurRadius);
        void Destroy();
        bool CanModifyBitmap { get; }

        Bitmap.Config SupportedBitmapConfig { get; }

        float ScaleFactor { get; }

        void Render(Canvas canvas, Bitmap bitmap);
    }
}
