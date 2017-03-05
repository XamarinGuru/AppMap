using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using System;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Content.PM;

namespace AppMapDroid
{
    [Activity(Label = "BottomSheetActivity", Theme = "@style/Theme.DesignDemo")]
    public class BottomSheetActivity : AppCompatActivity, IOnMapReadyCallback
    {
        Button cancel_ride_but, call_but, message_but;
        TextView name_txt, address_txt, time_txt;
        private DrawerLayout mDrawerLayout;
        private GoogleMap mMap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestedOrientation = ScreenOrientation.Portrait;

            SetContentView(Resource.Layout.Activity_BottomSheet);
            SetUpMap();

            name_txt = FindViewById<TextView>(Resource.Id.name_txt);
            address_txt = FindViewById<TextView>(Resource.Id.address_txt);
            time_txt = FindViewById<TextView>(Resource.Id.time_txt);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Mipmap.menu_but);
            ab.SetDisplayHomeAsUpEnabled(true);

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            if (navigationView != null)
            {
                SetUpDrawerContent(navigationView);
            }

            //var menu = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            //menu_but = FindViewById<Button>(Resource.Id.menu_but);
            //menu_but.Click += (sender, e) =>
            //{
            //    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
            //};

            cancel_ride_but = FindViewById<Button>(Resource.Id.cancel_ride_but);
            cancel_ride_but.Click += delegate
            {
                Toast.MakeText(this, "Votre course a bien ete annuler!", ToastLength.Long).Show();
                Finish();
            };

            call_but = FindViewById<Button>(Resource.Id.call_but);
            call_but.Click += delegate
            {
                Toast.MakeText(this, "call button clicked!", ToastLength.Long).Show();
                Finish();
            };

            message_but = FindViewById<Button>(Resource.Id.message_but);
            message_but.Click += delegate
            {
                Toast.MakeText(this, "message button clicked!", ToastLength.Long).Show();
                Finish();
            };

            LinearLayout sheet = FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            BottomSheetBehavior bottomSheetBehavior = BottomSheetBehavior.From(sheet);

            bottomSheetBehavior.PeekHeight = 300;
            bottomSheetBehavior.Hideable = true;

            bottomSheetBehavior.SetBottomSheetCallback(new MyBottomSheetCallBack());

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (o, e) =>
            {
                bottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                Finish();
            };

            // FIN MAIN____________________________________________________________________
        }

        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);

            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        private void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                e.MenuItem.SetChecked(true);
                mDrawerLayout.CloseDrawers();
            };
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
            if (mMap != null)
            {
                mMap.MyLocationEnabled = true;//AFFICHER LA POSITION DU POINT BLEU
                mMap.UiSettings.MyLocationButtonEnabled = true;// AFFICHER LE BOUTON GPS 
                MarkerOptions markerOpt1 = new MarkerOptions();
                markerOpt1.SetPosition(new LatLng(45.7050178, 4.8526129));
                markerOpt1.SetTitle("Vimy Ridge");
                mMap.AddMarker(markerOpt1);

                LatLng location = new LatLng(45.7050178, 4.8526129);
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(location);
                builder.Zoom(13);
                //builder.Bearing(155);
                //builder.Tilt(65);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                mMap.MoveCamera(cameraUpdate);
                mMap.UiSettings.ZoomControlsEnabled = false;// AFFICHER LE PLUS ET LE MOINS DU ZOOM
                mMap.UiSettings.CompassEnabled = false;//AFFICHER LA BOUSOLE
                mMap.UiSettings.MapToolbarEnabled = false; //ne pas afficher la redirection vers goggle map et navigation
                mMap.UiSettings.ZoomGesturesEnabled = true;// EMPECHER DE ZOUMER AVEC LES 2 DOIGT
                mMap.UiSettings.ScrollGesturesEnabled = false; // EMPECHER DE CHANGER DE VILLE SA RESTE BLOQUER
                mMap.UiSettings.RotateGesturesEnabled = true; // EMPECHER DE FAIRE TOURNER LA CARTE
                // if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted)
                mMap.MyLocationEnabled = true;//AFFICHER LA POSITION DU POINT BLEU 
                mMap.UiSettings.MyLocationButtonEnabled = false;// AFFICHER LE BOUTON GPS

                CircleOptions circleOptions = new CircleOptions();
                circleOptions.InvokeCenter(new LatLng(45.7050178, 4.8526129));
                circleOptions.InvokeRadius(200);// RAYON DU CERCLE
                circleOptions.InvokeStrokeWidth(4);// EPAISSEUR DU TRIT DU CERCLE
                circleOptions.InvokeStrokeColor(Android.Graphics.Color.ParseColor("#75C043")); // Cercle Couleur  
                Circle newCircle = mMap.AddCircle(circleOptions);
                newCircle.Visible = true;

            }
        }
    }

    public class MyBottomSheetCallBack : BottomSheetBehavior.BottomSheetCallback
    {
        public override void OnSlide(View bottomSheet, float slideOffset)
        {
            //Sliding
        }

        public override void OnStateChanged(View bottomSheet, int newState)
        {
            //State changed
        }
    }
}