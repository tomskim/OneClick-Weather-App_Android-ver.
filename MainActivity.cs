using Android.App;
using Android.Widget;
using Android.OS;
using System.Net;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Json;
using Android.Graphics;
using Android.Views.InputMethods;
using Android.Content;
using Org.Apache.Http;
using System.Collections.Generic;

namespace Android_CityWeather
{
    [Activity(Label = "CityWeather", MainLauncher = true, Icon = "@drawable/Icon")]
    public class MainActivity : Activity
    {
        int casetype = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);
            LinearLayout myScreen = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            myScreen.SetBackgroundColor(Color.LightBlue);

            EditText city = FindViewById<EditText>(Resource.Id.city);
            EditText country = FindViewById<EditText>(Resource.Id.country);
            Button button = FindViewById<Button>(Resource.Id.find);
            CheckBox CheckBox1 = FindViewById<CheckBox>(Resource.Id.checkBox1);
            CheckBox CheckBox2 = FindViewById<CheckBox>(Resource.Id.checkBox2);
            CheckBox CheckBox3 = FindViewById<CheckBox>(Resource.Id.checkBox3);

            button.Click += async (sender, e) => {

                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);

                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);

                int errorfree = 1;

                string url1 = "http://api.openweathermap.org/data/2.5/weather?APPID=3e9539d1f7a9e3882e6998d0222fed52&q=" + city.Text + "," + country.Text;

                if (string.IsNullOrWhiteSpace(city.Text) || string.IsNullOrWhiteSpace(country.Text))
                {
                    errorfree = 0;
                    new AlertDialog.Builder(this).SetMessage("One or more text fields are empty!").Show();
                }

                try
                {
                    JsonValue json1 = await FetchAsync(url1);
                    JsonValue coord = json1["coord"];
                    string url2 = "http://api.timezonedb.com/?format=json&key=04WHGJMVG1LY&lat=" + coord["lat"].ToString() + "&lng=" + coord["lon"].ToString();
                    JsonValue json2 = await FetchAsync(url2);
                }
                catch (WebException)
                {
                    errorfree = 0;
                    new AlertDialog.Builder(this).SetMessage("Please check your internet connection!").Show();
                }
                catch (KeyNotFoundException)
                {
                    errorfree = 0;
                    new AlertDialog.Builder(this).SetMessage("Please enter valid city/country names!").Show();
                }

                if (errorfree == 1)
                {
                   
                    JsonValue json1 = await FetchAsync(url1);
                    JsonValue coord = json1["coord"];
                    string url2 = "http://api.timezonedb.com/?format=json&key=04WHGJMVG1LY&lat=" + coord["lat"].ToString() + "&lng=" + coord["lon"].ToString();
                    JsonValue json2 = await FetchAsync(url2);
                    ParseAndDisplay(json1, json2);
                }
            };

            CheckBox1.CheckedChange += (sender, e) => {
                if (CheckBox1.Checked == true)
                {
                    if (casetype == 0)
                    {
                        casetype = 1;
                    }
                    else if (casetype == 1)
                    {
                        casetype = 0;
                    }
                    else
                    {
                        new AlertDialog.Builder(this).SetMessage("You cannot assign more than one unit!").Show();
                        CheckBox1.Checked = false;
                    }
                }
                else if (casetype == 1)
                {
                    casetype = 0;
                }
                else if ((casetype == 2) || (casetype == 3))
                {
                    new AlertDialog.Builder(this).SetMessage("You cannot assign more than one unit!").Show();
                    CheckBox1.Checked = false;
                }
            };

            CheckBox2.CheckedChange += (sender, e) => {
                if (CheckBox2.Checked == true)
                {
                    if (casetype == 0)
                    {
                        casetype = 2;
                    }
                    else if (casetype == 2)
                    {
                        casetype = 0;
                    }
                    else
                    {
                        new AlertDialog.Builder(this).SetMessage("You cannot assign more than one unit!").Show();
                        CheckBox2.Checked = false;
                    }
                }
                else if (casetype == 2)
                {
                    casetype = 0;
                }
                else if ((casetype == 1) || (casetype == 3))
                {
                    new AlertDialog.Builder(this).SetMessage("You cannot assign more than one unit!").Show();
                    CheckBox2.Checked = false;
                }
            };

            CheckBox3.CheckedChange += (sender, e) => {
                if (CheckBox3.Checked == true)
                {
                    if (casetype == 0)
                    {
                        casetype = 3;
                    }
                    else if (casetype == 3)
                    {
                        casetype = 0;
                    }
                    else
                    {
                        new AlertDialog.Builder(this).SetMessage("You cannot assign more than one unit!").Show();
                        CheckBox3.Checked = false;
                    }
                }
                else if (casetype == 3)
                {
                    casetype = 0;
                }
                else if ((casetype == 1) || (casetype == 2))
                {
                    new AlertDialog.Builder(this).SetMessage("You cannot assign more than one unit!").Show();
                    CheckBox3.Checked = false;
                }
            };

        }

        private async Task<JsonValue> FetchAsync(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));

            request.ContentType = "application/json";

            request.Method = "GET";

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    JsonValue jsondoc = await Task.Run(() => JsonObject.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsondoc.ToString());

                    return jsondoc;
                }
            }
        }

        private void ParseAndDisplay(JsonValue json1, JsonValue json2)
        {
            TextView time = FindViewById<TextView>(Resource.Id.timetext);
            TextView temperature = FindViewById<TextView>(Resource.Id.temptext);
            TextView humidity = FindViewById<TextView>(Resource.Id.humtext);
            TextView condition = FindViewById<TextView>(Resource.Id.condtext);
            ImageView image = FindViewById<ImageView>(Resource.Id.image);
            
            if (casetype == 0)
            {
                new AlertDialog.Builder(this).SetMessage("Please select a specific unit!").Show();
            }
            else
            {
                int timeResult = json2["timestamp"];

                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                dateTime = dateTime.AddSeconds(timeResult);

                JsonValue tempResult = json1["main"];

                double temp = tempResult["temp"];

                if (casetype == 1)
                {
                    temperature.Text = Math.Round((temp - 273.15), 1).ToString() + "°C";
                }
                else if (casetype == 2)
                {
                    temperature.Text = Math.Round(((temp * 9) / 5 - 459.67), 1).ToString() + "°F";
                }
                else if (casetype == 3)
                {
                    temperature.Text = Math.Round(temp, 1).ToString() + "K";
                }

                time.Text = string.Concat(dateTime.Hour.ToString("00"), ":", dateTime.Minute.ToString("00"));

                JsonValue condResult = json1["weather"];

                string iconval = condResult[0]["icon"];

                var imageBitmap = GetImageBitmapFromUrl("http://openweathermap.org/img/w/" + iconval + ".png");

                image.SetImageBitmap(imageBitmap);

                humidity.Text = tempResult["humidity"].ToString() + "%";

                condition.Text = condResult[0]["main"];
            }
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}


