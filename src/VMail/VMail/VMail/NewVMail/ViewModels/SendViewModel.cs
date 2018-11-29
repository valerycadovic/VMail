using System.ComponentModel;
using System.Windows.Input;
using VMail.NewVMail.Models;
using Xamarin.Forms;

namespace VMail.NewVMail.ViewModels
{
    /// <summary>
    /// MVVM ViewModel implementation
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class SendViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public ICommand SendCommand { get; protected set; }
        private readonly MessageSender _sender;
        private readonly Page _page;

        public SendViewModel(Page page)
        {
            _sender = new MessageSender();
            SendCommand = new Command(Send);
            this._page = page;
        }
        
        private async void Send()
        {
            await _sender.Send(from, text, to.Replace(" ", "").Split(','));
            await _page.DisplayAlert(string.Empty, "Паведамленне адпраўлена", "Добра");
        }
        
        private string text;
        public string Text
        {
            get => text;
            set
            {
                if(text != value)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        private string from;
        public string From
        {
            get => from;
            set
            {
                if(from != value)
                {
                    from = value;
                    OnPropertyChanged(nameof(From));
                }
            }
        }

        private string to;
        public string To
        {
            get => to;
            set
            {
                if(to != value)
                {
                    to = value;
                    OnPropertyChanged(nameof(To));
                }

            }
        }

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
