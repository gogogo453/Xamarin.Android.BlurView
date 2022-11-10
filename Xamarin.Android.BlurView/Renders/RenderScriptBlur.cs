using Android.Content;
using Android.Graphics;
using Android.Renderscripts;
using Xamarin.Android.BlurView.Interfaces;

namespace Xamarin.Android.BlurView.Renders
{
    [Obsolete]
    public class RenderScriptBlur : IBlurAlgorithm
    {
        private readonly Paint _paint = new(PaintFlags.FilterBitmap);
        private readonly RenderScript _renderScript;
        private readonly ScriptIntrinsicBlur _blurScript;
        
        private Allocation _outAllocation;

        private int _lastBitmapWidth = -1;
        private int _lastBitmapHeight = -1;

        public RenderScriptBlur(Context context)
        {
            _renderScript = RenderScript.Create(context);
            _blurScript = ScriptIntrinsicBlur.Create(_renderScript, Element.U8_4(_renderScript));
        }

        private bool CanReuseAllocation(Bitmap bitmap)
        {
            return bitmap.Height != _lastBitmapHeight && bitmap.Width != _lastBitmapWidth;
        }

        public bool CanModifyBitmap => true;

        public Bitmap.Config SupportedBitmapConfig => Bitmap.Config.Argb8888;

        public float ScaleFactor => IBlurController.DEFAULT_SCALE_FACTOR;

        public Bitmap Blur(Bitmap bitmap, float blurRadius)
        {
            Allocation inAllocation = Allocation.CreateFromBitmap(_renderScript, bitmap);

            if (CanReuseAllocation(bitmap))
            {
                if (_outAllocation != null)
                {
                    _outAllocation.Destroy();
                }

                _outAllocation = Allocation.CreateTyped(_renderScript, inAllocation.Type);

                _lastBitmapWidth = bitmap.Width;
                _lastBitmapHeight = bitmap.Height;
            }

            _blurScript.SetRadius(blurRadius);
            _blurScript.SetInput(inAllocation);
            //do not use inAllocation in forEach. it will cause visual artifacts on blurred Bitmap
            _blurScript.ForEach(_outAllocation);
            _outAllocation.CopyTo(bitmap);

            inAllocation.Destroy();
            return bitmap;
        }

        public void Destroy()
        {
            _blurScript.Destroy();
            _renderScript.Destroy();
            
            if (_outAllocation != null)
            {
                _outAllocation.Destroy();
            }
        }

        public void Render(Canvas canvas, Bitmap bitmap)
        {
            canvas.DrawBitmap(bitmap, 0f, 0f, _paint);
        }
    }
}
