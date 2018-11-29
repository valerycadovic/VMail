using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMail.NewVMail.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VMail.NewVMail.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AuthView : ContentPage
	{
		public AuthView ()
		{
			InitializeComponent ();

            BindingContext = new AuthViewModel(this)
            {
                Navigation = this.Navigation
            };
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}