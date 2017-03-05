using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Support.Design.Widget;
using static Android.App.KeyguardManager;
using Android.Content.PM;

namespace AppMapDroid
{
    [Activity(Label = "", Theme = "@style/Theme.AppCompat.Translucent")]
    public class FindTaxiActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(GetType().FullName, "FindTaxiActivity - OnCreate");
            Window.RequestFeature(WindowFeatures.NoTitle); // Ce sera Masquer la barre de titre
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_findTaxi);

            FloatingActionButton cancel_booking_find = FindViewById<FloatingActionButton>(Resource.Id.cancel_booking_find);
            cancel_booking_find.Click += (o, e) =>
            {
                View anchor = o as View;

                Snackbar.Make(anchor, "Etes vous sur de vouloir annuler le taxi ?", Snackbar.LengthLong)
                        .SetAction("OUI", v =>
                        {
                        //Do something here
                        Intent intent = new Intent(cancel_booking_find.Context, typeof(PickActivity));
                            StartActivity(intent);
                            FinishAfterTransition();
                        })
                        .Show();
            };

        }

        // ALERTE DIALOG QUAND LE CLIENT CLICK SUR LE BACK
        public override void OnBackPressed()
        {
            FloatingActionButton cancel_booking_find = FindViewById<FloatingActionButton>(Resource.Id.cancel_booking_find);
            //set alert for executing the task
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Message Mob1Taxi");
            alert.SetMessage("Etes vous sur de vouloir annuler le taxi ?");
            alert.SetPositiveButton("OUI", (senderAlert, args) => {
                Toast.MakeText(this, "Votre taxi à bien été annuler", ToastLength.Short).Show();
                Intent intent = new Intent(cancel_booking_find.Context, typeof(PickActivity));
                StartActivity(intent);
                Finish();
            });
            alert.SetNegativeButton("NON", (senderAlert, args) => {
                Toast.MakeText(this, "Il vous reste x secondes pour accepter la course", ToastLength.Short).Show();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        protected override void OnRestart()
        {
            Log.Debug(GetType().FullName, " FindTaxiActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnStart()
        {
            Log.Debug(GetType().FullName, " FindTaxiActivity - OnStart");
            base.OnStart();
        }

        protected override void OnResume()
        {
            Log.Debug(GetType().FullName, " FindTaxiActivity - OnResume");
            base.OnResume();
            RequestedOrientation = ScreenOrientation.Portrait;
        }

        // Called implicitly when device is about to sleep or application is backgrounded
        protected override void OnPause()
        {
            Log.Debug(GetType().FullName, " FindTaxiActivity - OnPause");
            base.OnPause();

        }
        // Called implicitly when device is about to wake up or foregrounded
        protected override void OnStop()
        {
            Log.Debug(GetType().FullName, " FindTaxiActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Log.Debug(GetType().FullName, " FindTaxiActivity - OnDestroy");
            base.OnDestroy();
        }

    }

}