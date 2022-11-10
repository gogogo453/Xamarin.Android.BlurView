using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Xamarin.Android.BlurView.Interfaces;
using static Android.Views.ViewTreeObserver;

namespace Xamarin.Android.BlurView
{
    public class PreDrawBlurController : Java.Lang.Object, IBlurController, IOnPreDrawListener
    {
        const int TRANSPARENT = 0;

        private float _blurRadius = IBlurController.DEFAULT_BLUR_RADIUS;

        private readonly IBlurAlgorithm _blurAlgorithm;
        private BlurViewCanvas _internalCanvas;
        private Bitmap _internalBitmap;

        private readonly BlurView _blurView;
        private int _overlayColor;
        private readonly ViewGroup _rootView;

        private readonly int[] _rootLocation = new int[2];
        private readonly int[] _blurViewLocation = new int[2];

        private bool _blurEnabled = true;
        private bool _isInitialized;

        private Drawable _frameClearDrawable;

        public PreDrawBlurController(BlurView blurView, ViewGroup rootView, int overlayColor, IBlurAlgorithm blurAlgorithm)
        {
            _blurView = blurView;
            _rootView = rootView;
            _overlayColor = overlayColor;
            _blurAlgorithm = blurAlgorithm;

            int measuredWidth = blurView.MeasuredWidth;
            int measuredHeight = blurView.MeasuredHeight;

            Init(measuredWidth, measuredHeight);
        }

        private void Init(int measuredWidth, int measuredHeight)
        {
            SetBlurAutoUpdate(true);
            SizeScaler sizeScaler = new SizeScaler(_blurAlgorithm.ScaleFactor);
            if (sizeScaler.IsZeroSized(measuredWidth, measuredHeight))
            {
                _blurView.SetWillNotDraw(true);
                return;
            }

            _blurView.SetWillNotDraw(false);
            var bitmapSize = sizeScaler.Scale(measuredWidth, measuredHeight);
            _internalBitmap = Bitmap.CreateBitmap(bitmapSize.Width, bitmapSize.Height, _blurAlgorithm.SupportedBitmapConfig);
            _internalCanvas = new BlurViewCanvas(_internalBitmap);
            _isInitialized = true;
            // Usually it's not needed, because `onPreDraw` updates the blur anyway.
            // But it handles cases when the PreDraw listener is attached to a different Window, for example
            // when the BlurView is in a Dialog window, but the root is in the Activity.
            // Previously it was done in `draw`, but it was causing potential side effects and Jetpack Compose crashes
            UpdateBlur();
        }

        void UpdateBlur()
        {
            if (!_blurEnabled || !_isInitialized)
            {
                return;
            }

            if (_frameClearDrawable == null)
            {
                _internalBitmap.EraseColor(Color.Transparent);
            }
            else
            {
                _frameClearDrawable.Draw(_internalCanvas);
            }

            _internalCanvas.Save();
            SetupInternalCanvasMatrix();
            _rootView.Draw(_internalCanvas);
            _internalCanvas.Restore();

            BlurAndSave();
        }

        private void SetupInternalCanvasMatrix()
        {
            _rootView.GetLocationOnScreen(_rootLocation);
            _blurView.GetLocationOnScreen(_blurViewLocation);

            int left = _blurViewLocation[0] - _rootLocation[0];
            int top = _blurViewLocation[1] - _rootLocation[1];

            float scaleFactorH = (float)_blurView.Height / _internalBitmap.Height;
            float scaleFactorW = (float)_blurView.Width / _internalBitmap.Width;

            float scaledLeftPosition = -left / scaleFactorW;
            float scaledTopPosition = -top / scaleFactorH;

            _internalCanvas.Translate(scaledLeftPosition, scaledTopPosition);
            _internalCanvas.Scale(1 / scaleFactorW, 1 / scaleFactorH);
        }

        public bool Draw(Canvas canvas)
        {
            if (!_blurEnabled || !_isInitialized)
            {
                return true;
            }

            if (canvas is BlurViewCanvas)
            {
                return false;
            }

            float scaleFactorH = (float)_blurView.Height / _internalBitmap.Height;
            float scaleFactorW = (float)_blurView.Width / _internalBitmap.Width;

            canvas.Save();
            canvas.Scale(scaleFactorW, scaleFactorH);
            _blurAlgorithm.Render(canvas, _internalBitmap);
            canvas.Restore();
            if (_overlayColor != TRANSPARENT)
            {
                Color color = new Color(_overlayColor);
                canvas.DrawColor(color);
            }

            return true;
        }

        private void BlurAndSave()
        {
            _internalBitmap = _blurAlgorithm.Blur(_internalBitmap, _blurRadius);
            if (!_blurAlgorithm.CanModifyBitmap)
            {
                _internalCanvas.SetBitmap(_internalBitmap);
            }
        }

        public void UpdateBlurViewSize()
        {
            int measuredWidth = _blurView.MeasuredWidth;
            int measuredHeight = _blurView.MeasuredHeight;

            Init(measuredWidth, measuredHeight);
        }

        public void Destroy()
        {
            SetBlurAutoUpdate(false);
            _blurAlgorithm.Destroy();
            _isInitialized = false;
        }

        public IBlurViewFacade SetBlurRadius(float radius)
        {
            _blurRadius = radius;
            return this;
        }

        public IBlurViewFacade SetFrameClearDrawable(Drawable frameClearDrawable)
        {
            _frameClearDrawable = frameClearDrawable;
            return this;
        }

        public IBlurViewFacade SetBlurEnabled(bool enabled)
        {
            _blurEnabled = enabled;
            SetBlurAutoUpdate(enabled);
            _blurView.Invalidate();
            return this;
        }

        public IBlurViewFacade SetBlurAutoUpdate(bool enabled)
        {
            _rootView.ViewTreeObserver.RemoveOnPreDrawListener(this);
            if (enabled)
            {
                _rootView.ViewTreeObserver.AddOnPreDrawListener(this);
            }
            return this;
        }

        public IBlurViewFacade SetOverlayColor(int overlayColor)
        {
            if (_overlayColor != overlayColor)
            {
                _overlayColor = overlayColor;
                _blurView.Invalidate();
            }
            return this;
        }

        public bool OnPreDraw()
        {
            UpdateBlur();
            return true;
        }
    }
}
