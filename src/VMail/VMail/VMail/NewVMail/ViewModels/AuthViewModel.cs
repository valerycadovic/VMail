using Email.Imap;
using System.ComponentModel;
using System.Windows.Input;
using VMail.Models;
using Xamarin.Forms;

namespace VMail.NewVMail.ViewModels
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public User User { get; set; }
        private readonly ImapClient _imap;
        private readonly Page _page;

        public ICommand SignInCommand { get; protected set; }
        public INavigation Navigation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthViewModel"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public AuthViewModel(Page page)
        {
            User = new User();
            _imap = new ImapClient(1);
            SignInCommand = new Command(SignIn);
            _page = page;
        }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address
        {
            get => User.Address;
            set
            {
                if (User.Address != value)
                {
                    User.Address = value;
                    OnPropertyChanged(nameof(User.Address));
                }
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get => User.Password;
            set
            {
                if (User.Password != value)
                {
                    User.Password = value;
                    OnPropertyChanged(nameof(User.Password));
                }
            }
        }

        /// <summary>
        /// Signs the in.
        /// </summary>
        private async void SignIn()
        {
            try
            {
                await _imap.ConnectAsync(Address, Password);
                await _imap.Quit();
                await Navigation.PushAsync(new Views.ListView());
            }
            catch
            {
                await _page.DisplayAlert("Памылка!", "Няправільны логін ці пароль", "Добра");
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
