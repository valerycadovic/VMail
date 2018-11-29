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
	public partial class MessageView : ContentPage
	{
        public MessageViewModel Mvm { get; protected set; }
        
		public MessageView (MessageViewModel vm)
		{
			InitializeComponent ();
            Mvm = vm;
            BindingContext = Mvm;
		}
	}
}