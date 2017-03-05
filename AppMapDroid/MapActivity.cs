using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Gms.Maps;
using static Android.Resource;
using Android.Support.V4.App;
using Android.Widget;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Gms.Common.Apis;
using Android.Gms.Location.Places.UI;
using Android.Gms.Location.Places;
using System;
using Android.Gms.Location;
using Android.Support.Compat;
using Android.Util;
using Android.Gms.Common;
using static Android.Gms.Common.Apis.GoogleApiClient;
using Android.Locations;
using Android.Content.PM;


namespace AppMapDroid
{
    [Activity(Label = "", Theme = "@style/Theme.DesignDemo")]
    public class MapActivity : AppCompatActivity, IConnectionCallbacks, IOnMapReadyCallback,
        IPlaceSelectionListener, IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {

        //teste de commite
        Button menu_but, pick_location_but;
        EditText search_txt;
        //TextView result_txt;
        private DrawerLayout mDrawerLayout;
        private GoogleMap mMap;
        GoogleApiClient apiClient;
        LocationRequest locRequest;
        bool _isGooglePlayServicesInstalled;

        public object Map { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.NoTitle); // Ce sera Masquer la barre de titre
            base.OnCreate(savedInstanceState);
            RequestedOrientation = ScreenOrientation.Portrait;
            SetContentView(Resource.Layout.activity_map);
            SetUpMap();
            var mGoogleApiClient = new Builder(this)
                .AddApi(PlacesClass.GEO_DATA_API)
                .AddApi(PlacesClass.PLACE_DETECTION_API)
                .EnableAutoManage(this, this)
                .Build();

            PlaceAutocompleteFragment autocompleteFragment = (PlaceAutocompleteFragment)base.FragmentManager.FindFragmentById(Resource.Id.place_autocomplete_fragment);

            autocompleteFragment.SetOnPlaceSelectedListener(this);


            AutocompleteFilter filter = new AutocompleteFilter.Builder()
         .SetTypeFilter(AutocompleteFilter.TypeFilterAddress)
         .Build();


            //search_txt = FindViewById<EditText>(Resource.Id.search_txt);
            //result_txt = FindViewById<TextView>(Resource.Id.result_txt);

            SupportToolbar toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Mipmap.menu_but);
            ab.SetDisplayHomeAsUpEnabled(false);


            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

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

            pick_location_but = FindViewById<Button>(Resource.Id.pick_location_but);
            pick_location_but.Click += delegate
            {
                StartActivity(new Intent(Application.Context, typeof(PickActivity)));
                Finish();
            };

            //search_but = FindViewById<Button>(Resource.Id.search_but);
            //search_but.Click += delegate
            //{
            //    if (search_txt.Text.Length != 0)
            //    {
            //        result_txt.Text = search_txt.Text;
            //    }
            //};

            _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled();

            if (_isGooglePlayServicesInstalled)
            {
                // pass in the Context, ConnectionListener and ConnectionFailedListener
                apiClient = new Builder(this, this, this)
                    .AddApi(LocationServices.API).Build();

                // generate a location request that we will pass into a call for location updates
                locRequest = new LocationRequest();

            }
            else
            {
                Log.Error("OnCreate", "Google Play Services is not installed");
                Toast.MakeText(this, "Google Play Services is not installed", ToastLength.Long).Show();
                Finish();
            }

        }



        public void onBackPressed()
        {
            MoveTaskToBack(true);
        }



        bool IsGooglePlayServicesInstalled()
        {
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MapActivity", "Google Play Services is installed on this device.");
                Toast.MakeText(this, "Google Play Services is installed", ToastLength.Long).Show();
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MapActivity", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);

                // Show error dialog to let user debug google play services
            }
            return false;
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug("OnResume", "OnResume called, connecting to client...");
            apiClient.Connect();
        }

        private void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override async void OnPause()
        {
            base.OnPause();
            Log.Debug("OnPause", "OnPause called, stopping location updates");
            if (apiClient.IsConnected)
            {
                // stop location updates, passing in the LocationListener
                await LocationServices.FusedLocationApi.RemoveLocationUpdates(apiClient, this);
                apiClient.Disconnect();
            }
        }


        ////Interface methods

        public void OnConnected(Bundle bundle)
        {
            // This method is called when we connect to the LocationClient. We can start location updated directly form
            // here if desired, or we can do it in a lifecycle method, as shown above 

            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now connected to client");
        }

        public void OnDisconnected()
        {
            // This method is called when we disconnect from the LocationClient.

            // You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
            Log.Info("LocationClient", "Now disconnected from client");
        }

        //public void OnConnectionFailed(ConnectionResult bundle)
        //{
        //    // This method is used to handle connection issues with the Google Play Services Client (LocationClient). 
        //    // You can check if the connection has a resolution (bundle.HasResolution) and attempt to resolve it

        //    // You must implement this to implement the IGooglePlayServicesClientOnConnectionFailedListener Interface
        //    Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
        //}

        public void OnLocationChanged(Location location)
        {
            // This method returns changes in the user's location if they've been requested

            // You must implement this to implement the Android.Gms.Locations.ILocationListener Interface
            Log.Debug("LocationClient", "Location updated");

        }

        public void OnConnectionSuspended(int i)
        {
            Log.Debug("LocationClient", "Connection Suspended");
        }

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
                builder.Zoom(15);
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
                //if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted)
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

        public void OnError(Statuses status)
        {
            Log.Debug(BuildConfig.BuildType, "onError: Status = " + status.ToString());
        }

        public void OnPlaceSelected(IPlace place)
        {
            if (mMap != null)
            {
                mMap.Clear();
                MarkerOptions markerOpt1 = new MarkerOptions();
                markerOpt1.SetPosition(new LatLng(place.LatLng.Latitude, place.LatLng.Longitude));
                markerOpt1.SetTitle("Vimy Ridge");
                mMap.AddMarker(markerOpt1);
                LatLng location = new LatLng(place.LatLng.Latitude, place.LatLng.Longitude);

                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(location);
                builder.Zoom(19);
                //builder.Bearing(155);
                //builder.Tilt(65);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                mMap.MoveCamera(cameraUpdate);
            }
            else
            {
                return;
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
        }
    }
}
