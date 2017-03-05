using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using static Android.Resource;
using Android.Support.V4.Widget;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Locations;
using System;
using Android.Gms.Common;

namespace AppMapDroid
{
    [Activity(Label = "", Theme = "@style/Theme.DesignDemo")]
    public class MyRideActivity : AppCompatActivity, IOnMapReadyCallback, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        Button menu_but;
        TextView name_txt, address_txt, time_txt;
        private DrawerLayout mDrawerLayout;
        private GoogleMap mMap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.NoTitle); // Ce sera Masquer la barre de titre

            base.OnCreate(savedInstanceState);
            RequestedOrientation = ScreenOrientation.Portrait;
            SetContentView(Resource.Layout.activity_myride);
            SetUpMap();

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            name_txt = FindViewById<TextView>(Resource.Id.name_txt);
            address_txt = FindViewById<TextView>(Resource.Id.address_txt);
            time_txt = FindViewById<TextView>(Resource.Id.time_txt);

            SupportToolbar toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Mipmap.menu_but);
            ab.SetDisplayHomeAsUpEnabled(false);

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            if (navigationView != null)
            {
                SetUpDrawerContent(navigationView);
            }

            menu_but = FindViewById<Button>(Resource.Id.menu_but);
            menu_but.Click += (sender, e) =>
            {
                mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
            };

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (o, e) =>
            {
                View anchor = o as View;

                Snackbar.Make(anchor, "Voir le info du taxi!", Snackbar.LengthLong)
                        .SetAction("Voir", v =>
                        {
                            //Do something here
                            Intent intent = new Intent(fab.Context, typeof(BottomSheetActivity));
                            StartActivity(intent);
                        })
                        .Show();
            };

        } // FIN MAIN


        private void SetUpMap()
        {
            if (mMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
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

        // OUVERTURE DU MENU AU CLIC SUR LE MENU
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
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

        public void OnLocationChanged(Location location)
        {
            throw new NotImplementedException();
        }

        public void OnConnected(Bundle connectionHint)
        {
            throw new NotImplementedException();
        }

        public void OnConnectionSuspended(int cause)
        {
            throw new NotImplementedException();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }
    }
}
