using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Support.Design.Widget;
using Android.Content.PM;

namespace AppMapDroid
{
    [Activity(Label = "", Theme = "@style/Theme.AppCompat.Translucent")]
    public class SearchTaxiActivity : AppCompatActivity
    {

        private TextView _adress_search_taxi;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(GetType().FullName, "SearchTaxiActivity - OnCreate");
            Window.RequestFeature(WindowFeatures.NoTitle); // Ce sera Masquer la barre de titre
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_searchTaxi);




            // POUR RECUPERER ADRESSE DE de la commande venant de l'activity PickActivity
            TextView adress_search_taxi = FindViewById<TextView>(Resource.Id.adress_search_taxi);
            _adress_search_taxi = FindViewById<TextView>(Resource.Id.adress_search_taxi);
            _adress_search_taxi.Text = Intent.GetStringExtra("PickActivity") ?? "";


            FloatingActionButton cancel_booking_search = FindViewById<FloatingActionButton>(Resource.Id.cancel_booking_search);
            cancel_booking_search.Click += (o, e) =>
            {
                View anchor = o as View;

                Snackbar.Make(anchor, "Etes vous sur de vouloir abandonner la recherche d'un taxi ?", Snackbar.LengthLong)
                        .SetAction("OUI", v =>
                        {
                            //Do something here
                            Intent intent = new Intent(cancel_booking_search.Context, typeof(PickActivity));
                            StartActivity(intent);
                            FinishAfterTransition();
                        })
                        .Show();
            };

        }

        // ALERTE DIALOG QUAND LE CLIENT CLICK SUR LE BACK
        public override void OnBackPressed()
        {
            FloatingActionButton cancel_booking_search = FindViewById<FloatingActionButton>(Resource.Id.cancel_booking_search);
            //set alert for executing the task
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Message Mob1Taxi");
            alert.SetMessage("Etes vous sur de vouloir abandonner la recherche d'un taxi ?");
            alert.SetPositiveButton("OUI", (senderAlert, args) => {
                Toast.MakeText(this, "La recherche d'un taxi à été abandonner", ToastLength.Short).Show();
                Intent intent = new Intent(cancel_booking_search.Context, typeof(FindTaxiActivity));
                StartActivity(intent);
                Finish();

                //Intent intent = new Intent(cancel_booking_search.Context, typeof(PickActivity));
                //            StartActivity(intent);
                //            Finish();
            });
            alert.SetNegativeButton("NON", (senderAlert, args) => {
                Toast.MakeText(this, "La recherche d'un taxi continue", ToastLength.Short).Show();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }


        protected override void OnRestart()
        {
            Log.Debug(GetType().FullName, "PickDetailActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnStart()
        {
            Log.Debug(GetType().FullName, "SearchTaxiActivity - OnStart");
            base.OnStart();
        }

        protected override void OnResume()
        {
            Log.Debug(GetType().FullName, "SearchTaxiActivity - OnResume");
            base.OnResume();
            RequestedOrientation = ScreenOrientation.Portrait;
        }

        protected override void OnPause()
        {
            Log.Debug(GetType().FullName, "SearchTaxiActivity - OnPause");
            base.OnPause();
        }

        protected override void OnStop()
        {
            Log.Debug(GetType().FullName, "SearchTaxiActivity - OnStop");
            base.OnStop();
        }


        protected override void OnDestroy()
        {
            Log.Debug(GetType().FullName, "SearchTaxiActivity - OnDestroy");
            base.OnDestroy();
        }




    }

}