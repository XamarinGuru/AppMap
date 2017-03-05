
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Content.PM;

namespace AppMapDroid
{
    [Activity(Label = "RegisterActivity", Theme = "@style/Theme.DesignDemo")]

    public class RegisterActivity : AppCompatActivity
    {
        Spinner countrycode_spin, howdid_spin;
        EditText firstname_txt, lastname_txt, mobile_txt, email_txt, password_txt, confirmpassword_txt;
        Button login_but, register_but, facebook_login_but;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.NoTitle); // Ce sera Masquer la barre de titre
            base.OnCreate(savedInstanceState);
            RequestedOrientation = ScreenOrientation.Portrait;

            SetContentView(Resource.Layout.activity_register);

            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);   //keyboard disabled

            countrycode_spin = FindViewById<Spinner>(Resource.Id.countrycode_spin);
            howdid_spin = FindViewById<Spinner>(Resource.Id.howdid_spin);

            firstname_txt = FindViewById<EditText>(Resource.Id.firstname_txt);
            lastname_txt = FindViewById<EditText>(Resource.Id.lastname_txt);
            mobile_txt = FindViewById<EditText>(Resource.Id.mobile_txt);
            email_txt = FindViewById<EditText>(Resource.Id.email_txt);
            password_txt = FindViewById<EditText>(Resource.Id.password_txt);
            confirmpassword_txt = FindViewById<EditText>(Resource.Id.confirmpassword_txt);

            countrycode_spin.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(countrycode_spinner_ItemSelected);
            var countrycode_adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.countrycode_array, Android.Resource.Layout.SimpleSpinnerItem);
            countrycode_adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            countrycode_spin.Adapter = countrycode_adapter;

            howdid_spin.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(howdid_spinner_ItemSelected);
            var howdid_adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.how_did_you_hear_about_us_array, Android.Resource.Layout.SimpleSpinnerItem);
            howdid_adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            howdid_spin.Adapter = howdid_adapter;

            login_but = FindViewById<Button>(Resource.Id.login_but);
            login_but.Click += delegate
            {

                StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
                Finish();
            };
            register_but = FindViewById<Button>(Resource.Id.app_register_but);
            register_but.Click += delegate
            {
                Toast.MakeText(this, "Coming soon!", ToastLength.Long).Show();
            };
            facebook_login_but = FindViewById<Button>(Resource.Id.facebook_login_but);
            facebook_login_but.Click += delegate
            {
                Toast.MakeText(this, "Coming soon!", ToastLength.Long).Show();
            };
        }

        private void countrycode_spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;

            string toast = string.Format("The country code is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }
        private void howdid_spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;

            //string toast = string.Format("{0}", spinner.GetItemAtPosition(e.Position));
            //Toast.MakeText(this, toast, ToastLength.Long).Show();
        }
    }
}
