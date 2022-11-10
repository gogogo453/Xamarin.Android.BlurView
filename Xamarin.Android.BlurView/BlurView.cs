using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Xamarin.Android.BlurView.Interfaces;

namespace Xamarin.Android.BlurView
{
    public class BlurView : FrameLayout
    {
        IBlurController blurController = new NoOpController();

        private int _overlayColor;

        public BlurView(Context context): base(context)
        {
            Init(null, 0);
        }

        public BlurView(Context context, IAttributeSet attrs): base(context, attrs)
        {
            Init(attrs, 0);
        }

        public BlurView(Context context, IAttributeSet attrs, int defStyleAttr): base(context, attrs, defStyleAttr)
        {
            Init(attrs, defStyleAttr);
        }

        private void Init(IAttributeSet attrs, int defStyleAttr)
        {
            var typedAttrs = Context.ObtainStyledAttributes(attrs, Resource.Styleable.BlurView, defStyleAttr, 0);
            _overlayColor = typedAttrs.GetColor(Resource.Styleable.BlurView_blurOverlayColor, 0);
            typedAttrs.Recycle();
        }

        public override void Draw(Canvas canvas)
        {
            bool shouldDraw = blurController.Draw(canvas);
            if (shouldDraw)
            {
                base.Draw(canvas);
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            blurController.UpdateBlurViewSize();
        }

        protected override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            blurController.SetBlurAutoUpdate(false);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            if (!IsHardwareAccelerated)
            {
                Log.Error("BlurView", "BlurView can't be used in not hardware-accelerated window!");
            }
            else
            {
                blurController.SetBlurAutoUpdate(true);
            }
        }

        public IBlurViewFacade SetupWith(ViewGroup rootView, IBlurAlgorithm algorithm)
        {
            blurController.Destroy();
            blurController = new PreDrawBlurController(this, rootView, _overlayColor, algorithm);
            return blurController;
        }

        public IBlurViewFacade SetBlurRadius(float radius)
        {
            return blurController.SetBlurRadius(radius);
        }
        
        public IBlurViewFacade SetOverlayColor(int overlayColor)
        {
            _overlayColor = overlayColor;
            return blurController.SetOverlayColor(overlayColor);
        }

        public IBlurViewFacade SetBlurAutoUpdate(bool enabled)
        {
            return blurController.SetBlurAutoUpdate(enabled);
        }

        public IBlurViewFacade SetBlurEnabled(bool enabled)
        {
            return blurController.SetBlurEnabled(enabled);
        }
    }
}
