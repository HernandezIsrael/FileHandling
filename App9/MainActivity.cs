using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Collections;
using Android.Util;
using Java.Lang;
using System.Threading.Tasks;

namespace App9
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {

            string appDirectory;
            string newDirectory;
            string textFilePath;
            string text;
            IEnumerable entries;
            IEnumerable files;

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Button read = FindViewById<Button>(Resource.Id.button1);
            Button write = FindViewById<Button>(Resource.Id.button2);
            Button log = FindViewById<Button>(Resource.Id.button3);
            Button readLog = FindViewById<Button>(Resource.Id.button4);
            EditText editText = FindViewById<EditText>(Resource.Id.editText1);
            EditText textArea = FindViewById<EditText>(Resource.Id.editText2);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += delegate
            {

                //Grab the app storage directory
                // In this case we're going to use the external files directory becasue android devices have limited storage in terms of the app storage area
                appDirectory = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
                entries = Directory.EnumerateDirectories(appDirectory);
                files = Directory.EnumerateFiles(appDirectory);

                //The following method takes an absolute path and appends another item to it which can be a folder (and creates the foñder) or a file (if it ends with a ".'extension'", so it creates a file)
                newDirectory = Path.Combine(appDirectory, "myDirectory");

                Directory.CreateDirectory(newDirectory); //Creates if it doesn't exists

                textFilePath = Path.Combine(appDirectory, "myTXT.txt");
                File.CreateText(textFilePath);

                foreach(var e in entries)
                {
                    Log.Debug("DEBUG", e.ToString());
                }

                foreach (var e in files)
                {
                    Log.Debug("DEBUG", e.ToString());
                }

                //Unless you really, really, relly have to, never use the internal storage

            };

            read.Click += delegate
            {
                appDirectory = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
                textFilePath = Path.Combine(appDirectory, "myTXT.txt");
                text = File.ReadAllText(textFilePath);
                textArea.Text = text;
            };

            write.Click += delegate
            {
                appDirectory = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
                textFilePath = Path.Combine(appDirectory, "myTXT.txt");
                File.AppendAllText(textFilePath, editText.Text);
                editText.Text = "";
            };

            log.Click += delegate
            {
                Toast.MakeText(this, "Creating log...", ToastLength.Short).Show();
                appDirectory = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
                textFilePath = Path.Combine(appDirectory, "log.txt");

                Task.Factory.StartNew(() => { //Let's create a new thread

                    RunOnUiThread(() => {
                        readLog.Enabled = false; //We're trying to acces a button which lies on th UI thread and we're running our whole taks into a separate thread. Let's make this run back on the main thread
                    });
                    
                    for (int i = 0; i < 5; i++)
                    {
                        File.AppendAllText(textFilePath, DateTime.Now.ToString() + "\n");
                        Thread.Sleep(1000);
                    }

                    RunOnUiThread(() => {
                        readLog.Enabled = true;
                    });

                });
            };

            readLog.Click += delegate
            {
                appDirectory = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
                textFilePath = Path.Combine(appDirectory, "log.txt");
                text = File.ReadAllText(textFilePath);
                Log.Debug("FILE", text);
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }
	}
}

