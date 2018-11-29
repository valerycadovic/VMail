using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using VMail.Views;
using Xamarin.Forms;
using EmailProtocols.Smtp;
using EmailProtocols.Smtp.Realization;
using Plugin.Settings;
using VMail.Models;

namespace VMail.ViewModels
{
    public class AuthorizationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public INavigation Navigation { get; set; }
        public ICommand AuthCommand { get; protected set; }

        private User user;

        public AuthorizationViewModel()
        {
            AuthCommand = new Command(Auth);
        }

        public string Username
        {
            get => user.Address;
            set
            {
                user.Address = value;
                CrossSettings.Current.AddOrUpdateValue(nameof(user.Address), user.Address);
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => user.Password;
            set
            {
                user.Password = value;
                CrossSettings.Current.AddOrUpdateValue(nameof(user.Password), user.Password);
                OnPropertyChanged(nameof(Password));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Auth()
        {
            await Navigation.PopModalAsync();
        }
    }
}
