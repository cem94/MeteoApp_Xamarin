
using Android.App;
using Android.Content.PM;
using Android.OS;
using Acr.UserDialogs;
using System;
using Plugin.FirebasePushNotification;

namespace MeteoApp.Droid
{
    [Activity(Label = "MeteoApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Get the current activity and init
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            //Necessary for init the dialog whit user
            UserDialogs.Init(this);

            FirebasePushNotificationManager.ProcessIntent(this, Intent);
        }

        // Request permission
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Console.WriteLine("Permission request");
        }
    }
}