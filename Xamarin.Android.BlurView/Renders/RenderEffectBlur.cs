using Android.Graphics;
using System.Runtime.Versioning;
using Xamarin.Android.BlurView.Interfaces;

namespace Xamarin.Android.BlurView.Renders
{
    [SupportedOSPlatform("android31.0")]
    public class RenderEffectBlur : IBlurAlgorithm
    {
        private readonly RenderNode _renderNode = new("BlurViewNode");

        private int _height, _width;

        public bool CanModifyBitmap => true;

        public Bitmap.Config SupportedBitmapConfig => Bitmap.Config.Argb8888;

        public float ScaleFactor => IBlurController.DEFAULT_SCALE_FACTOR;

        public Bitmap Blur(Bitmap bitmap, float blurRadius)
        {
            if (bitmap.Height != _height || bitmap.Width != _width)
            {
                _height = bitmap.Height;
                _width = bitmap.Width;
                _renderNode.SetPosition(0, 0, _width, _height);
            }

            Canvas canvas = _renderNode.BeginRecording();
            canvas.DrawBitmap(bitmap, 0, 0, null);
            _renderNode.EndRecording();
            _renderNode.SetRenderEffect(RenderEffect.CreateBlurEffect(blurRadius, blurRadius, Shader.TileMode.Mirror));

            return bitmap;
        }

        public void Destroy()
        {
            _renderNode.DiscardDisplayList();
        }

        public void Render(Canvas canvas, Bitmap bitmap)
        {
            canvas.DrawRenderNode(_renderNode);
        }
    }
}
