using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VMail.NewVMail.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewMaster : ContentPage
    {
        public ListView ListView;

        public ListViewMaster()
        {
            InitializeComponent();
            
        }

        private void Send_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SendMessageView());
        }

        class ListViewMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<ListViewMenuItem> MenuItems { get; set; }
            
            public ListViewMasterViewModel()
            {
                MenuItems = new ObservableCollection<ListViewMenuItem>(new[]
                {
                    new ListViewMenuItem { Id = 0, Title = "Page 1" },
                    new ListViewMenuItem { Id = 1, Title = "Page 2" },
                    new ListViewMenuItem { Id = 2, Title = "Page 3" },
                    new ListViewMenuItem { Id = 3, Title = "Page 4" },
                    new ListViewMenuItem { Id = 4, Title = "Page 5" },
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}