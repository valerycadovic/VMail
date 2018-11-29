using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMail.Models;
using VMail.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VMail.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ReadMessagePage : ContentPage
	{
        public ReadMessageViewModel ViewModel { get; private set; }

		public ReadMessagePage (ReadMessageViewModel vm)
        {
            InitializeComponent();
            ViewModel = vm;
            BindingContext = ViewModel;
		}
	}
}