# Xamarin.Android.BlurView

Port of [Dimezis/BlurView](https://github.com/Dimezis/BlurView) for Xamarin.Android .NET 6

Dynamic iOS-like blur for Android Views. Includes library and small example project.

BlurView can be used as a regular FrameLayout. It blurs its underlying content and draws it as a
background for its children. The children of the BlurView are not blurred. BlurView redraws its
blurred content when changes in view hierarchy are detected (draw() called). It honors its position
and size changes, including view animation and property animation.

## How to use

```xml
<Xamarin.Android.BlurView.BlurView
  android:id="@+id/blurView"
  android:layout_width="match_parent"
  android:layout_height="wrap_content"
  app:blurOverlayColor="@color/colorOverlay">

   <!--Any child View here, TabLayout for example-->

</Xamarin.Android.BlurView.BlurView
```

```csharp
float radius = 20;
BlurView blurView = FindViewById<BlurView>(Resource.Id.blurView);
IBlurAlgorithm BlurAlgorithm = Build.VERSION.SdkInt >= BuildVersionCodes.S
            ? new RenderEffectBlur()
            : new RenderScriptBlur(this);

//Activity's root View. Can also be root View of your layout (preferably)
ViewGroup rootView = decorView.FindViewById<ViewGroup>(Android.Resource.Id.content);
//set background, if your root layout doesn't have one
Drawable windowBackground = Window.DecorView.Background;

blurView.SetupWith(rootView, BlurAlgorithm)       
       .SetFrameClearDrawable(windowBackground)
       .SetBlurRadius(radius);
```

Always try to choose the closest possible root layout to BlurView. This will greatly reduce the amount of work needed for creating View hierarchy snapshot.

