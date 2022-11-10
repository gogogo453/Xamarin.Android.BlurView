using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using AndroidX.ViewPager.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Tabs;
using AndroidX.Fragment.App;
using Xamarin.Android.BlurView;
using Xamarin.Android.BlurView.Renders;
using Xamarin.Android.BlurView.Interfaces;

// Ported from:
// https://github.com/Dimezis/BlurView/blob/master/app/src/main/java/com/eightbitlab/blurview_sample/MainActivity.java
namespace SampleApp
{
    [Activity(
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        Label = "@string/app_name")]
    public class MainActivity : AppCompatActivity
    {
        ViewGroup root;
        ViewPager viewPager;
        SeekBar radiusSeekBar;
        BlurView topBlurView;
        BlurView bottomBlurView;
        TabLayout tabLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            InitView();
            SetupBlurView();
            SetupViewPager();
        }

        private void InitView()
        {
            viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            bottomBlurView = FindViewById<BlurView>(Resource.Id.bottomBlurView);
            topBlurView = FindViewById<BlurView>(Resource.Id.topBlurView);
            radiusSeekBar = FindViewById<SeekBar>(Resource.Id.radiusSeekBar);
            root = FindViewById<ViewGroup>(Resource.Id.root);
        }

        void SetupViewPager()
        {
            viewPager.OffscreenPageLimit = 2;
            viewPager.Adapter = new ViewPagerAdapter(SupportFragmentManager);
            tabLayout.SetupWithViewPager(viewPager);
        }

        void SetupBlurView()
        {
            float radius = 25f;
            float minBlurRadius = 4f;
            float step = 4f;

            //set background, if your root layout doesn't have one
            Drawable windowBackground = Window.DecorView.Background;
            
            var topViewSettings = topBlurView.SetupWith(root, BlurAlgorithm)
                .SetFrameClearDrawable(windowBackground)
                .SetBlurRadius(radius);

            var bottomViewSettings = bottomBlurView.SetupWith(root, BlurAlgorithm)
                .SetFrameClearDrawable(windowBackground)
                .SetBlurRadius(radius);

            int initialProgress = (int)(radius * step);
            radiusSeekBar.Progress = initialProgress;

            radiusSeekBar.ProgressChanged += (sender, args) =>
            {
                float blurRadius = args.Progress / step;
                blurRadius = Java.Lang.Math.Max(blurRadius, minBlurRadius);
                topViewSettings.SetBlurRadius(blurRadius);
                bottomViewSettings.SetBlurRadius(blurRadius);
            };
        }

        private IBlurAlgorithm BlurAlgorithm =>
            Build.VERSION.SdkInt >= BuildVersionCodes.S
            ? new RenderEffectBlur()
            : new RenderScriptBlur(this);
    }

    class ViewPagerAdapter : FragmentPagerAdapter
    {
        List<BaseFragment> pages;

        public ViewPagerAdapter(AndroidX.Fragment.App.FragmentManager fragmentManager)
            : base(fragmentManager, BehaviorResumeOnlyCurrentFragment)
        {
            pages = new List<BaseFragment>
            {
                new ScrollFragment(),
                new ListFragment(),
                new ImageFragment()
            };
        }

        public override int Count => pages.Count;

        public override AndroidX.Fragment.App.Fragment GetItem(int position)
        {
            return pages[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(pages[position].Title);
        }
    }
}

