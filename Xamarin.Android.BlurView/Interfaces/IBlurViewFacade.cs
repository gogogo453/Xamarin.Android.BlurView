using Android.Graphics.Drawables;

namespace Xamarin.Android.BlurView.Interfaces
{
    public interface IBlurViewFacade
    {
        IBlurViewFacade SetBlurEnabled(bool enabled);

        IBlurViewFacade SetBlurAutoUpdate(bool enabled);

        IBlurViewFacade SetFrameClearDrawable(Drawable frameClearDrawable);

        IBlurViewFacade SetBlurRadius(float radius);

        IBlurViewFacade SetOverlayColor(int overlayColor);
    }
}
