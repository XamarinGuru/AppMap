using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Gms.Common;
using Firebase.Iid;
using Android.Util;
using Android.Gms.Location;
using System;
using Android.Content.PM;

namespace AppMapDroid
{
    [Activity(Label = "LoginActivity", Theme = "@style/Theme.DesignDemo")]
    public class LoginActivity : AppCompatActivity
    {
        static readonly string TAG = typeof(LoginActivity).FullName;
        Button login_but, register_but, facebook_login_but;
        bool _isGooglePlayServicesInstalled;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.NoTitle); // Ce sera Masquer la barre de titre
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_login);

            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);   //keyboard disabled

            _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();

            if (_isGooglePlayServicesInstalled)
            {

                Log.Info(" LoginActivity", "Google Play Services is installed on this device." + FirebaseInstanceId.Instance.Token);
            }
            else
            {
                Log.Error("OnCreate", "Google Play Services is not installed");
                Toast.MakeText(this, "Google Play Services is not installed", ToastLength.Long).Show();
                Finish();
            }

            register_but = FindViewById<Button>(Resource.Id.register_but);
            register_but.Click += delegate
            {

                StartActivity(new Intent(Application.Context, typeof(RegisterActivity)));
                Finish();
            };

            login_but = FindViewById<Button>(Resource.Id.app_login_but);
            login_but.Click += delegate
            {

                StartActivity(new Intent(Application.Context, typeof(MapActivity)));
                Finish();

            };

            facebook_login_but = FindViewById<Button>(Resource.Id.facebook_login_but);
            facebook_login_but.Click += delegate
            {
                StartActivity(new Intent(Application.Context, typeof(MyRideActivity)));
                Finish();
            };


        }

        bool IsGooglePlayServicesInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(" LoginActivity", "Google Play Services is installed on this device." + FirebaseInstanceId.Instance.Token);

                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(" LoginActivity", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);

                // Show error dialog to let user debug google play services
            }
            return false;
        }

        public bool IsPlayServicesAvailable()
        {
            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                }
            }
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))

                    Log.Debug(TAG, "GoogleApiAvailability " + GoogleApiAvailability.Instance.GetErrorString(resultCode));
                else
                {
                    Log.Debug(TAG, "This device is not supported " + FirebaseInstanceId.Instance.Token);
                    Finish();
                }
                return false;
            }
            else
            {
                Log.Debug(TAG, "2 - Google Play Services is available. " + FirebaseInstanceId.Instance.Token);
                return true;
            }
        }
        protected override void OnRestart()
        {
            Log.Debug(GetType().FullName, "LoginActivity - OnRestart");
            base.OnRestart();

        }

        protected override void OnStart()
        {
            Log.Debug(GetType().FullName, "LoginActivity - OnStart");
            base.OnStart();

        }

        protected override void OnResume()
        {
            Log.Debug(GetType().FullName, "LoginActivity - On Resume");
            base.OnResume();
            RequestedOrientation = ScreenOrientation.Portrait;

        }

        protected override void OnPause()
        {
            base.OnPause();
            Log.Debug(GetType().FullName, "LoginActivity - On Pause");
        }

        protected override void OnStop()
        {
            Log.Debug(GetType().FullName, "LoginActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Log.Debug(GetType().FullName, "LoginActivity - On Destroy");
            base.OnDestroy();
        }
    }
}
//// Obtenir Location Manager et vérifier pour GPS et réseau des services de localisation 
//LocationManager lm = (LocationManager)GetSystemService(LocationService);
//            if (!lm.IsProviderEnabled(LocationManager.GpsProvider) || !lm.IsProviderEnabled(LocationManager.NetworkProvider))
//            {
//                // Construire la boîte de dialogue d' alerte
//                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
//alert.SetTitle("Message Mob1Taxi");
//                alert.SetMessage("Vous devez activez votre GPS pour la géolocalisation ! Voulez vous activez le GPS ?");
//                alert.SetPositiveButton("OUI", (senderAlert, args) =>
//                {
//                    // Afficher les paramètres de localisation lorsque l'utilisateur reconnaît la boîte de dialogue d' alerte 
//                    var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
//                    StartActivity(intent);
//                });
//                alert.SetNegativeButton("NON", (senderAlert, args) =>
//                {
//                    // FinishAffinity TUE TOUTE LES ACTIVITY QUI YA EN DESSOUS
//                    //SA EVITE LA PILE DES ACTIVITY SI YA TROP DE VAS ET VIEN ENTRE LES ACTIVITY
//                    FinishAffinity();
//                });
//                Dialog dialog = alert.Create();
//dialog.Show();