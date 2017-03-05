using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Messaging;
using Android.Graphics;

namespace AppMapDroid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";


        public override void OnMessageReceived(RemoteMessage message)

        {

            Log.Debug(TAG, "From: " + message.From);
            Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
            // Forward the received message in a local notification


            SendNotification(message.GetNotification().Body);
        }

        void SendNotification(string messageBody)
        {
            // Set up an intent so that tapping the notifications returns to this app:
            var intent = new Intent(this, typeof(FindTaxiActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            // Instantiate the builder and set notification elements, including pending intent:
            var notificationBuilder = new Notification.Builder(this)

                // Set the intent that will fire when the user taps the notification.
                .SetContentIntent(pendingIntent)

                //icon appears in device notification bar and right hand corner of notification
                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)

                // Large icon appears on the left of the notification
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_stat_ic_notification))

                // Content title, which appears in large type at the top of the notification
                .SetContentTitle("Mob1Taxi")

                // Content text, which appears in smaller text below the title
                .SetContentText("Le taxi a accepter votre commmande !")

                // The subtext, which appears under the text on newer devices.
                // This will show-up in the devices with Android 4.2 and above only
                //.SetSubText("teste du subtext")
                //.SetColor(-16711681)
                .SetAutoCancel(true)
                //.SetProgress(20, 1, true)
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                //.SetCategory(Notification.CategoryCall)
                .SetCategory(Notification.CategoryAlarm);
            //.SetProgress(0, 20, false)

            // .SetUsesChronometer(true)
            // .SetChronometerCountDown = (true)
            // .SetWhen(20000);  // the time stamp, you will probably use System.currentTimeMillis() for most scenarios

            // Définit l'indicateur de progression à une valeur maximum, l'achèvement en cours
            // pourcentage, et l' état "déterminée" mBuilder . setProgress ( 100 , incr , false );
            // Délivre les notifications mNotifyManager . notifier ( 0 , mBuilder . construire ());
            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}