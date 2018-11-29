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
    public partial class ListViewDetail : ContentPage
    {
        public ListViewDetail()
        {
            InitializeComponent();

            BindingContext = new ListViewModel(this)
            {
                Navigation = this.Navigation
            };
        }

    }
}