using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMail.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VMail.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SendPage : ContentPage
	{
        public SendPageViewModel ViewModel { get; private set; }

		public SendPage (SendPageViewModel vm)
		{
			InitializeComponent ();
            this.ViewModel = vm;
            BindingContext = ViewModel;
		}
	}
}