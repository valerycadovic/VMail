using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Xamarin.Forms;
using VMail.Views;
using Plugin.Settings;
using VMail.NewVMail.Views;

namespace VMail
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //if (CrossSettings.Current.GetValueOrDefault("login", string.Empty) == string.Empty)
            //{
                MainPage = new NavigationPage(new AuthView());
            /*}
            else
            {
                MainPage = new NavigationPage(new ListMessagesView());
            }
            */
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
