
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Util;
using System;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;

namespace AppMapDroid
{
    [Activity(Label = "", Theme = "@style/Theme.DesignDemo")]
    public class PickDetailActivity : AppCompatActivity, IOnMapReadyCallback
    {
		GoogleMap map;

        Button back_but, remove_but;
        private EditText _autocompleteTextView;

		LatLng _currentLocation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(GetType().FullName, "PickDetailActivity - OnCreate");
            Window.RequestFeature(WindowFeatures.NoTitle); // Ce sera Masquer la barre de titre
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.content_pickDetail);

            _autocompleteTextView = FindViewById<EditText>(Resource.Id.autocompleteTextView);


            // POUR RECUPERER ADRESSE DE PickActivity
            EditText autocompleteTextView = FindViewById<EditText>(Resource.Id.autocompleteTextView);
            _autocompleteTextView.Text = Intent.GetStringExtra("PickActivity") ?? "";

            back_but = FindViewById<Button>(Resource.Id.back_but);
            back_but.Click += delegate
            {
                StartActivity(new Intent(Application.Context, typeof(PickActivity)));
                Finish();
            };

            remove_but = FindViewById<Button>(Resource.Id.remove_but);
            _autocompleteTextView = FindViewById<EditText>(Resource.Id.autocompleteTextView);
            remove_but.Click += delegate
            {
                Log.Debug(GetType().FullName, "DELETE ADRESSE AUTOCOMPLETE - DELETE ADRESSE AUTOCOMPLETE");
                if (_autocompleteTextView != null)
                {
                    _autocompleteTextView.Text = "";

                }
            };
			FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
        }


		private CameraPositionlHandler _cameraPositionHandler;
		public void OnMapReady(GoogleMap googleMap)
		{
			LatLng paris = new LatLng(48.857708, 2.346353);
			CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
			builder.Target(paris);
			builder.Zoom(7);
			CameraPosition cameraPosition = builder.Build();
			CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
			map = googleMap;
			map.CameraChange += OnCameraChanged;
			_cameraPositionHandler = new CameraPositionlHandler(map, this);

			if (map != null)
			{
				map.MoveCamera(cameraUpdate);
				map.MyLocationEnabled = false;//ne pas afficher le point bleu
				map.UiSettings.MapToolbarEnabled = false; //ne pas afficher la redirection vers goggle map et navigation
				map.UiSettings.MyLocationButtonEnabled = false;// AFFICHER LE BOUTON GPS
			}
		}
		public async void DragMapPinProcess(CameraPosition cameraPos)
		{
			try
			{
				_currentLocation = cameraPos.Target;
				var geo = new Geocoder(this);

				var addresses = await geo.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 1);

				Address addressList = addresses[0];

				string formatedAddress = "";

				for (var i = 0; i < addressList.MaxAddressLineIndex; i++)
				{
					formatedAddress += addressList.GetAddressLine(i) + " ";
				}
				_autocompleteTextView.Text = formatedAddress;
			}
			catch
			{
			}
		}

		private class CameraPositionlHandler : Handler
		{
			private CameraPosition _lastCameraPosition;
			private GoogleMap _googleMap;
			private PickDetailActivity rootVC;

			public CameraPositionlHandler(GoogleMap googleMap, PickDetailActivity rootVC)
			{
				_googleMap = googleMap;
				this.rootVC = rootVC;
			}

			public override void HandleMessage(Message msg)
			{
				if (_googleMap != null)
				{
					if (msg.What == 1)
					{
						_lastCameraPosition = _googleMap.CameraPosition;
					}
					else if (msg.What == 2)
					{
						if (_lastCameraPosition.Equals(_googleMap.CameraPosition))
						{
							rootVC.DragMapPinProcess(_lastCameraPosition);
						}
					}
				}
			}
		}
		void OnCameraChanged(object sender, GoogleMap.CameraChangeEventArgs e)
		{
			_cameraPositionHandler.RemoveMessages(1);
			_cameraPositionHandler.RemoveMessages(2);
			_cameraPositionHandler.SendEmptyMessageDelayed(1, 300);
			_cameraPositionHandler.SendEmptyMessageDelayed(2, 600);
		}

        //protected override void OnRestart()
        //{
        //    Log.Debug(GetType().FullName, "PickDetailActivity - OnRestart");
        //    base.OnRestart();
        //}

        protected override void OnStart()
        {
            Log.Debug(GetType().FullName, "PickDetailActivity - OnStart");
            base.OnStart();
        }

        protected override void OnResume()
        {
            Log.Debug(GetType().FullName, "PickDetailActivity - OnResume");
            base.OnResume();
            RequestedOrientation = ScreenOrientation.Portrait;
        }

        protected override void OnPause()
        {
            Log.Debug(GetType().FullName, "PickDetailActivity - OnPause");
            base.OnPause();
        }

        protected override void OnStop()
        {
            Log.Debug(GetType().FullName, "PickDetailActivity - OnStop");
            base.OnStop();
        }


        protected override void OnDestroy()
        {
            Log.Debug(GetType().FullName, "PickDetailActivity - OnDestroy");
            base.OnDestroy();
        }

	}

}


