using Android.Graphics;

namespace Xamarin.Android.BlurView
{
    // Servers purely as a marker of a Canvas used in BlurView
    // to skip drawing itself and other BlurViews on the View hierarchy snapshot
    public class BlurViewCanvas : Canvas
    {
        public BlurViewCanvas(Bitmap bitmap) : base(bitmap) { }
    }
}
