using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using VMail.ViewModels;

namespace VMail.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthorizationPage : ContentPage
    {

        public AuthorizationPage()
        {
            InitializeComponent();

            BindingContext = new AuthorizationViewModel()
            {
                Navigation = this.Navigation
            };
        }
    }
}