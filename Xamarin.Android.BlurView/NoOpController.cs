using Android.Graphics;
using Android.Graphics.Drawables;
using Xamarin.Android.BlurView.Interfaces;

namespace Xamarin.Android.BlurView
{
    //Used in edit mode and in case if no BlurController was set
    public class NoOpController : IBlurController
    {
        public bool Draw(Canvas canvas)
            => true;

        public IBlurViewFacade SetBlurAutoUpdate(bool enabled)
            => this;

        public IBlurViewFacade SetBlurEnabled(bool enabled)
            => this;

        public IBlurViewFacade SetBlurRadius(float frameFillDrawable)
            => this;

        public IBlurViewFacade SetFrameClearDrawable(Drawable frameClearDrawable)
            => this;

        public IBlurViewFacade SetOverlayColor(int overlayColor)
            => this;

        public void UpdateBlurViewSize()
        { }

        public void Destroy()
        { }
    }
}
