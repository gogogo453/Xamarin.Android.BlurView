using Android.Graphics;

namespace Xamarin.Android.BlurView.Interfaces
{
    public interface IBlurController : IBlurViewFacade
    {
        const float DEFAULT_SCALE_FACTOR = 6f;
        const float DEFAULT_BLUR_RADIUS = 16f;

        bool Draw(Canvas canvas);

        void UpdateBlurViewSize();

        void Destroy();
    }
}
