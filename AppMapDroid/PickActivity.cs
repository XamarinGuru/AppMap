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
using Android.Locations;
using System;
using Android.Util;
using Android.Gms.Common;
using Firebase.Iid;
using Android.Content.PM;
using Android;
using Android.Runtime;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using System.Collections.Generic;
using Taxi;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AppMapDroid
{
    [Activity(Label = "", Theme = "@style/Theme.DesignDemo")]
    public class PickActivity : AppCompatActivity, IOnMapReadyCallback, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener 

    {

        public const long UPDATE_INTERVAL_IN_MILLISECONDS = 20000;
        public const long FASTEST_UPDATE_INTERVAL_IN_MILLISECONDS = UPDATE_INTERVAL_IN_MILLISECONDS / 2;
        protected const string REQUESTING_LOCATION_UPDATES_KEY = "requesting-location-updates-key";
        protected const string LOCATION_KEY = "location-key";
        protected const string LAST_UPDATED_TIME_STRING_KEY = "last-updated-time-string-key";

        protected GoogleApiClient mGoogleApiClient;
        protected LocationRequest mLocationRequest;
        protected Location mCurrentLocation;
        protected bool mRequestingLocationUpdates;
        protected string mLastUpdateTime;

        protected const string TAG = "main-activity";
        protected const string ADDRESS_REQUESTED_KEY = "address-request-pending";
        protected const string LOCATION_ADDRESS_KEY = "location-address";
        protected Location mLastLocation;
        protected bool mAddressRequested;
        protected string mAddressOutput;
        private AddressResultReceiver mResultReceiver;
        protected TextView search_txt;

        static readonly string TAGE = typeof(PickActivity).FullName;

        private LocationManager _locationManager;
        private string _locationProvider;
        LocationManager locationManager;
        private string provider;
        const int RequestLocationId = 0;

        //FloatingActionButton myLocationButton;
        Button menu_but, request_ride_but, destination_but;

        private DrawerLayout mDrawerLayout;
        private GoogleMap mMap;
        private TextView _search_txt;

        bool _isGooglePlayServicesInstalled;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.RequestFeature(WindowFeatures.NoTitle); // Ce sera Masquer la barre de titre
            Log.Debug(GetType().FullName, "activity_pick - OnCreate");

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_pick);


            Log.Debug(TAG, "1-google app id =: " + Resource.String.google_app_id);
            if (!GetString(Resource.String.google_app_id).Equals("1:647985738537:android:618097eb35a52170"))
                // throw new Java.Lang.Exception("Invalaid json file");


                IsPlayServicesAvailable();
            SetUpMap();
            InitializeLocationManager();

            mResultReceiver = new AddressResultReceiver(new Handler());
            mResultReceiver.OnReceiveResultImpl = (resultCode, resultData) =>
            {
                mAddressOutput = resultData.GetString(Constants.ResultDataKey);
                DisplayAddressOutput();

                if (resultCode == 0)
                {
                    ShowToast(GetString(Resource.String.address_found));
                }
                mAddressRequested = false;
            };



            _search_txt = FindViewById<TextView>(Resource.Id.search_txt);
            //FloatingActionButton myLocationButton = FindViewById<FloatingActionButton>(Resource.Id.myLocationButton);
            //FloatingActionButton FAB = FindViewById<FloatingActionButton>(Resource.Id.myLocationButton);
            destination_but = FindViewById<Button>(Resource.Id.destination_but);

            destination_but.Click += localisation_Click;
            destination_but.Click += FetchAddressButtonHandler;
            destination_but.Click += StartUpdatesButtonHandler;

            mRequestingLocationUpdates = false;
            mAddressOutput = string.Empty;
            UpdateValuesFromBundle(savedInstanceState);
            BuildGoogleApiClient();

            locationManager = (LocationManager)GetSystemService(LocationService);
            provider = locationManager.GetBestProvider(new Criteria(), false);


            ImageView pin_marker = (ImageView)FindViewById(Resource.Id.pin_marker);

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
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            menu_but = FindViewById<Button>(Resource.Id.menu_but);
            menu_but.Click += (sender, e) =>
            {
                mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
            };

            // LE CLICK FAIT LA TRANSMISSION DE DE L'ADRESSE A L'ACTIVITY PickDetailActivity
            TextView search_txt = FindViewById<TextView>(Resource.Id.search_txt);
            search_txt = FindViewById<TextView>(Resource.Id.search_txt);
            search_txt.Click += delegate
            {
                if (string.IsNullOrWhiteSpace(search_txt.Text))
                {
                    StartActivity(new Intent(Application.Context, typeof(PickDetailActivity)));
                    Finish();
                }
                else
                {
                    var PickDetailActivity = new Intent(this, typeof(PickDetailActivity));
                    PickDetailActivity.PutExtra("PickActivity", search_txt.Text);
                    StartActivity(PickDetailActivity);
                    Finish();
                };
            };

            // LE CLICK FAIT LA TRANSMISSION DE DE L'ADRESSE A L'ACTIVITY SearchTaxiActivity
            request_ride_but = FindViewById<Button>(Resource.Id.request_ride_but);
            request_ride_but.Click += delegate
            {
                if (string.IsNullOrWhiteSpace(search_txt.Text))
                {
                    Toast.MakeText(this, "adresse incorrect merci de verifier", ToastLength.Long).Show();
                }
                else
                {
                    Log.Debug(TAG, "4 -logTokenButton - request_ride_but =: " + Resource.Id.request_ride_but + " " + "TOKEN PUSH DU BOUTON DE COMMANDE" + " - " + FirebaseInstanceId.Instance.Token);
                    var SearchTaxiActivity = new Intent(this, typeof(SearchTaxiActivity));
                    SearchTaxiActivity.PutExtra("PickActivity", search_txt.Text);
                    StartActivity(SearchTaxiActivity);
                };
            };
        }

        // FIN MAIN////////////////////////////////////////////////////////////////////////////FIN MAIN

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

        private void InitializeLocationManager()
        {
            // Obtenir Location Manager et vérifier pour GPS et réseau des services de localisation 
            _locationManager = (LocationManager)GetSystemService(LocationService);
            if (!_locationManager.IsProviderEnabled(LocationManager.GpsProvider) || !_locationManager.IsProviderEnabled(LocationManager.NetworkProvider))

            {
                // Construire la boîte de dialogue d' alerte
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Message Mob1Taxi");
                alert.SetMessage("Vous devez activez votre GPS pour la géolocalisation ! Voulez vous activez le GPS ?");
                alert.SetPositiveButton("OUI", (senderAlert, args) =>

                {
                    // Afficher les paramètres de localisation lorsque l'utilisateur reconnaît la boîte de dialogue d' alerte 
                    var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);

                    StartActivity(intent);
                });
                alert.SetNegativeButton("NON", (senderAlert, args) =>
                {
                    // FinishAffinity TUE TOUTE LES ACTIVITY QUI YA EN DESSOUS
                    //SA EVITE LA PILE DES ACTIVITY SI YA TROP DE VAS ET VIEN ENTRE LES ACTIVITY
                    FinishAffinity();
                });
                Dialog dialog = alert.Create();
                dialog.Show();
                Log.Debug(TAG, "InitializeLocationManager" + " " + LocationManager.GpsProvider + " " + LocationManager.NetworkProvider);
            }
        }

        private void localisation_Click(object sender, EventArgs e)
        {          
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(mMap.CameraPosition.Target);
            CameraPosition cameraPosition = builder.Build();
            //CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            Toast.MakeText(this, "Géolocalisation en cours Merci de patienter !", ToastLength.Long).Show();

            if (mMap != null)
            {
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);                
            }

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

                    //if (item.ItemId == Resource.Id.nav_home_1)
                    //{
                    //    StartActivity (typeof (RegisterActivity));
                    //    return true;

                    //}
                    return base.OnOptionsItemSelected(item);
            }
        }

        // ALERTE DIALOG QUAND LE CLIENT CLICK SUR LE BACK
        public override void OnBackPressed()
        {
            //set alert for executing the task           
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetTitle("Message Mob1Taxi");
            alert.SetMessage("Etes vous sur de vouloir quitter l'application ?");
            alert.SetPositiveButton("OUI", (senderAlert, args) =>
            {
                // FinishAffinity TUE TOUTE LES ACTIVITY QUI YA EN DESSOUS
                //SA EVITE LA PILE DES ACTIVITY SI YA TROP DE VAS ET VIEN ENTRE LES ACTIVITY
                FinishAffinity();
                Log.Debug(GetType().FullName, "back_Activity_Pick -  FinishAffinity();");
            });
            alert.SetNegativeButton("NON", (senderAlert, args) =>
            {
                Toast.MakeText(this, "Géolocalisation en cours Merci de patienter !", ToastLength.Long).Show();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            LatLng paris = new LatLng(48.857708, 2.346353);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(paris);
            builder.Zoom(7);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            mMap = googleMap;

            if (mMap != null)
            {
                mMap.MoveCamera(cameraUpdate);
                mMap.MyLocationEnabled = false;//ne pas afficher le point bleu
                mMap.UiSettings.MapToolbarEnabled = false; //ne pas afficher la redirection vers goggle map et navigation
                mMap.UiSettings.MyLocationButtonEnabled = false;// AFFICHER LE BOUTON GPS
            }

            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted)
            {
                mMap.MyLocationEnabled = false;//ne pas afficher le point bleu
            }
            else
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation }, RequestLocationId);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case RequestLocationId:
                    if (grantResults[0] == Permission.Granted)

                    {
                        if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
                        {

                            mMap.MyLocationEnabled = false;
                            Toast.MakeText(this, "Location permission is GOOD", ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Location permission is denied", ToastLength.Long).Show();
                    }
                    break;
            }
        }

        protected override void OnRestart()
        {
            Log.Debug(GetType().FullName, "Activity_Pick - OnRestart");
            base.OnRestart();
            mGoogleApiClient.Connect();
        }

        protected override void OnStart()
        {
            Log.Debug(GetType().FullName, "Activity_Pick - OnStart");
            base.OnStart();
            mGoogleApiClient.Connect();             
        }

        protected override async void OnResume()
        {
            Log.Debug(GetType().FullName, "Activity_Pick - On Resume");           
            base.OnResume();
            RequestedOrientation = ScreenOrientation.Portrait;
            if (mGoogleApiClient.IsConnected && mRequestingLocationUpdates)
            {                 
                await StartLocationUpdates();
                StartIntentService();
            }
             
            //locationManager.RequestLocationUpdates(provider, 400, 1, this);
            //DetectNetwork();
        }

        protected override async void OnPause()
        {
            base.OnPause();
            Log.Debug(GetType().FullName, "Activity_Pick - On Pause");
            if (mGoogleApiClient.IsConnected)
            {
                await StopLocationUpdates();
            }
            // locationManager.RemoveUpdates(this);
        }

        void Android.Gms.Location.ILocationListener.OnLocationChanged(Location location)
        {
            double lat, lng;
            lat = location.Latitude;
            lng = location.Longitude;
            TextView search_txt = FindViewById<TextView>(Resource.Id.search_txt);
            mCurrentLocation = location;
            UpdateUI();
            mMap.Clear();
            LatLng mGoogleApiClient = new LatLng(lat, lng);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(mGoogleApiClient);
            builder.Zoom(14);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            if (mMap != null)
            {
                mMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }
            Log.Debug(GetType().FullName, "OnLocationChanged = - Adresse" + " " + search_txt.Text + " " + lat + " " + lng);

            CircleOptions circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(new LatLng(lat, lng));
            circleOptions.InvokeRadius(200);// RAYON DU CERCLE
            circleOptions.InvokeStrokeWidth(4);// EPAISSEUR DU TRIT DU CERCLE
            circleOptions.InvokeStrokeColor(Android.Graphics.Color.ParseColor("#75C043")); // Cercle Couleur  
            Circle newCircle = mMap.AddCircle(circleOptions);
            newCircle.Visible = true;

            string URL_GetTaxis = "GetTaxis";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("lat", lat.ToString().Replace(',', '.'));
            parameters.Add("longi", lng.ToString().Replace(',', '.'));
            var JsonRetour = taxisController.POST(URL_GetTaxis, parameters);
            var retour = JsonConvert.DeserializeObject<ReturnData>(JsonRetour);

            for (int i = 0; i < retour.Data.taxiData.Length; i++)
            {
                var marker = new MarkerOptions()
                .SetPosition(new LatLng((double)retour.Data.taxiData[i].Position.Lat, (double)retour.Data.taxiData[i].Position.Lon))
                .Anchor(0.5f, 0.5f)
                .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Mipmap.marker_taxi))
                .SetTitle(search_txt.Text);
                mMap.AddMarker(marker);
            }
        }
  
        void UpdateValuesFromBundle(Bundle savedInstanceState)
        {
            if (savedInstanceState != null)
            {
                if (savedInstanceState.KeySet().Contains(ADDRESS_REQUESTED_KEY))
                {
                    mAddressRequested = savedInstanceState.GetBoolean(ADDRESS_REQUESTED_KEY);
                }
                if (savedInstanceState.KeySet().Contains(LOCATION_ADDRESS_KEY))
                {
                    mAddressOutput = savedInstanceState.GetString(LOCATION_ADDRESS_KEY);
                    DisplayAddressOutput();
                }

                if (savedInstanceState.KeySet().Contains(REQUESTING_LOCATION_UPDATES_KEY))
                {
                    mRequestingLocationUpdates = savedInstanceState.GetBoolean(REQUESTING_LOCATION_UPDATES_KEY);
                }

                if (savedInstanceState.KeySet().Contains(LOCATION_KEY))
                {
                    mCurrentLocation = (Location)savedInstanceState.GetParcelable(LOCATION_KEY);
                }
                if (savedInstanceState.KeySet().Contains(LAST_UPDATED_TIME_STRING_KEY))
                {
                    mLastUpdateTime = savedInstanceState.GetString(LAST_UPDATED_TIME_STRING_KEY);
                }
                UpdateUI();
            }
        }

        protected void BuildGoogleApiClient()
        {
            mGoogleApiClient = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(LocationServices.API)
                .Build();
            CreateLocationRequest();
        }

        protected void CreateLocationRequest()
        {
            mLocationRequest = new LocationRequest();
            mLocationRequest.SetInterval(UPDATE_INTERVAL_IN_MILLISECONDS);
            mLocationRequest.SetFastestInterval(FASTEST_UPDATE_INTERVAL_IN_MILLISECONDS);
            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
        }

        public async void StartUpdatesButtonHandler(object sender, EventArgs e)
        {
            if (!mRequestingLocationUpdates)
            {
                mRequestingLocationUpdates = true;
                await StartLocationUpdates();
            }
        }

        protected async Task StartLocationUpdates()
        {
            await LocationServices.FusedLocationApi.RequestLocationUpdates(mGoogleApiClient, mLocationRequest, this);
            StartIntentService();
             
        }

        void UpdateUI()
        {
            if (mCurrentLocation != null)
            {
                StartIntentService();
                 
            }
        }

        protected async Task StopLocationUpdates()
        {
            await LocationServices.FusedLocationApi.RemoveLocationUpdates(mGoogleApiClient, this);
        }

        public void FetchAddressButtonHandler(object sender, EventArgs e)
        {
            if (mGoogleApiClient.IsConnected && mLastLocation != null)
            {
                StartIntentService();
            }
            mAddressRequested = true;
        }

        protected override void OnStop()
        {
            Log.Debug(GetType().FullName, "Activity_Pick - OnStop");
            base.OnStop();
            if (mGoogleApiClient.IsConnected)
            {
                mGoogleApiClient.Disconnect();
            }
        }

        protected override void OnDestroy()
        {
            Log.Debug(GetType().FullName, "Activity_Pick - On Destroy");
            base.OnDestroy();
        }

        public async void OnConnected(Bundle connectionHint)
        {
            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
            if (mLastLocation != null)
            {
                if (!Geocoder.IsPresent)
                {
                    Toast.MakeText(this, Resource.String.no_geocoder_available, ToastLength.Long).Show();
                    return;
                }
                if (mAddressRequested)
                {
                    StartIntentService();
                    Log.Debug(GetType().FullName, "OnConnected mAddressRequested = - Adresse" + mAddressRequested);
                }
                }
                if (mCurrentLocation == null)
                {
                    mCurrentLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
                    UpdateUI();
                }

                if (mRequestingLocationUpdates)
                {
                    await StartLocationUpdates();
                }
            }

        public void OnConnectionSuspended(int cause)
        {
            mGoogleApiClient.Connect();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Info(TAG, "Connection failed: ConnectionResult.getErrorCode() = " + result.ErrorCode);
        }

        protected void StartIntentService()
        {
            var intent = new Intent(this, typeof(FetchAddressIntentService));
            intent.PutExtra(Constants.Receiver, mResultReceiver);
            intent.PutExtra(Constants.LocationDataExtra, mLastLocation);
            StartService(intent);
            Log.Debug(GetType().FullName, "StartIntentService = - Adresse" + " " + mResultReceiver + " " + mLastLocation);

        }

        protected void DisplayAddressOutput()
        {
            _search_txt.Text = mAddressOutput;
        }

        protected void ShowToast(string text)
        {
            Toast.MakeText(this, text, ToastLength.Short).Show();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(ADDRESS_REQUESTED_KEY, mAddressRequested);
            outState.PutString(LOCATION_ADDRESS_KEY, mAddressOutput);

            outState.PutBoolean(REQUESTING_LOCATION_UPDATES_KEY, mRequestingLocationUpdates);
            outState.PutParcelable(LOCATION_KEY, mCurrentLocation);
            base.OnSaveInstanceState(outState);
        }

        class AddressResultReceiver : ResultReceiver
        {
            public Action<int, Bundle> OnReceiveResultImpl { get; set; }
            public AddressResultReceiver(Handler handler)
                : base(handler)
            {
            }

            protected override void OnReceiveResult(int resultCode, Bundle resultData)
            {
                OnReceiveResultImpl(resultCode, resultData);
            }
        }

        public class ReturnData
        {
            public TaxiData Data;
        }

        public class TaxiData : Error
        {
            public taxiData[] taxiData;
        }

        public class taxiData : Error
        {
            public AdsDescriptor Ads { get; set; }
            public double? CrowflyDistance { get; set; }
            public DriverDescriptor Driver { get; set; }
            public string Id { get; set; }
            public string LastUpdate { get; set; }
            public string Operator { get; set; }
            public CoordinatesDescriptor Position { get; set; }
            public VehicleDescriptor Vehicle { get; set; }
        }

        public class Error
        {
            public string ErrorMessage { get; set; }
            public string Message { get; set; }
        }

        public class AdsDescriptor
        {
            public string Insee { get; set; }
            public string Numero { get; set; }
        }

        public class DriverDescriptor
        {
            public string Departement { get; set; }
            public string ProfessionalLicence { get; set; }
        }

        public class CoordinatesDescriptor
        {
            public double? Lat { get; set; }
            public double? Lon { get; set; }
        }

        public class VehicleDescriptor
        {
            public List<string> Characteristics { get; set; }
            public string Color { get; set; }
            public string Constructor { get; set; }
            public string LicencePlate { get; set; }
            public string Model { get; set; }
            public int? NbSeats { get; set; }
            public string Type { get; set; }
        }

    }
}








//public override void OnLowMemory()
//{

//    base.OnLowMemory();
//    Log.Debug(GetType().FullName, "Activity_Pick - OnLowMemory");
//}

//public void onBackPressed()
//{
//    FinishAndRemoveTask();
//}


// pour dectecter quelle est la connection utiliser wifi, itinerance
//private void DetectNetwork()
//{
//    ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
//    NetworkInfo info = connectivityManager.ActiveNetworkInfo;
//    bool isOnline = info.IsConnected;

//    Log.Debug(TAG, "IsOnline = {0}", isOnline);
//    Toast.MakeText(this, "IsOnline.", ToastLength.Long).Show();

//    if (isOnline)
//    {
//        // Display the type of connectionn
//        NetworkInfo.State activeState = info.GetState();

//        // Check for a WiFi connection
//        bool isWifi = info.Type == ConnectivityType.Wifi;
//        if (isWifi)
//        {
//            Log.Debug(TAG, "Wifi connected.");
//            Toast.MakeText(this, "Wifi connected.", ToastLength.Long).Show();
//        }
//        else
//        {
//            Log.Debug(TAG, "Wifi disconnected.");
//            Toast.MakeText(this, "Wifi disconnected", ToastLength.Long).Show();
//        }

//        // Check if roaming
//        if (info.IsRoaming)
//        {
//            Log.Debug(TAG, "Roaming.");
//            Toast.MakeText(this, "Roaming.", ToastLength.Long).Show();
//        }
//        else
//        {
//            Log.Debug(TAG, "Not roaming.");
//            Toast.MakeText(this, "Not roaming.", ToastLength.Long).Show();
//        }
//    }
//    else
//    {
//        Toast.MakeText(this, "iNDETERMINER.", ToastLength.Long).Show();
//    }
//}

//IOnCameraMoveListener, 
//IOnCameraMoveCanceledListener, 
//IOnCameraIdleListener


//public void OnMapReady(GoogleMap googleMap)
//mMap.SetOnCameraIdleListener(this);
//mMap.SetOnCameraMoveListener(this);
//mMap.SetOnCameraMoveCanceledListener(this);


//public void onCameraMoveStarted(int reason)
//{

//    if (reason == OnCameraMoveStartedListener.ReasonGesture)
//    {
//        Toast.MakeText(this, "The user gestured on the map.",
//                       ToastLength.Long).Show();

//    }
//    else if (reason == OnCameraMoveStartedListener
//                          .ReasonApiAnimation)
//    {
//        Toast.MakeText(this, "The user tapped something on the map.",
//                      ToastLength.Long).Show();
//    }
//    else if (reason == OnCameraMoveStartedListener
//                          .ReasonDeveloperAnimation)
//    {
//        Toast.MakeText(this, "The app moved the camera.",
//                       ToastLength.Long).Show();
//    }
//}
//public void OnCameraMove()
//{       
//    Log.Debug(GetType().FullName, " OnCameraMove() = - Adresse The app moved the camera.");


//}

//public void OnCameraMoveCanceled()
//{
//    Toast.MakeText(this, "Camera movement canceled.",
//              ToastLength.Long).Show();
//}

//public void OnCameraIdle()
//{
//    Toast.MakeText(this, "The camera has stopped moving.",
//                 ToastLength.Long).Show();
//    StartIntentService();
//}
