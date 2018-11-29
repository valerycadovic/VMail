using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Email.Imap;
using VMail.NewVMail.Models;
using VMail.NewVMail.Views;
using Xamarin.Forms;
using Xamarin.Forms.Extended;

namespace VMail.NewVMail.ViewModels
{
    public class ListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MessageLoader loader = new MessageLoader();
        public INavigation Navigation { get; set; }
        private Page page;

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public InfiniteScrollCollection<MessageViewModel> Messages { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewModel"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public ListViewModel(Page page)
        {
            this.page = page;
            Messages = new InfiniteScrollCollection<MessageViewModel>
            {
                OnLoadMore = async () =>
                {
                    IsBusy = true;
                    var list = await Load();
                    IsBusy = false;
                    return list;
                },
                OnCanLoadMore = () => loader.MessagesLoaded < loader.MessagesCount
            };
            LoadMessages();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        private async Task<InfiniteScrollCollection<MessageViewModel>> Load()
        {
            var list = new InfiniteScrollCollection<MessageViewModel>();
            List<ImapMessageInfo> msgs = await loader.LoadMessages();

            foreach (var i in msgs)
            {
                list.Add(new MessageViewModel(i));
            }
            IsBusy = false;
            return list;
        }

        private bool isUpdating = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is updating.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is updating; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpdating
        {
            get => isUpdating;
            set
            {
                isUpdating = value;
                OnPropertyChanged(nameof(IsUpdating));
            }
        }

        private bool isBusy;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        /// <summary>
        /// Gets the update messages command.
        /// </summary>
        /// <value>
        /// The update messages command.
        /// </value>
        public ICommand UpdateMessagesCommand => new Command(async () =>
        {
            IsUpdating = true;
            List<ImapMessageInfo> msgs = await loader.UpdateMessages();
            Messages.Clear();
            foreach (var i in msgs)
            {
                Messages.Add(new MessageViewModel(i));
            }
            IsUpdating = false;
        });

        /// <summary>
        /// Gets the delete message command.
        /// </summary>
        /// <value>
        /// The delete message command.
        /// </value>
        public ICommand DeleteMessageCommand => new Command(async () =>
        {
            bool response = await page.DisplayAlert("", "Выдаліць паведамленне?", "Так", "Не");
            if (response == true)
            {
                await loader.DeleteMessage(selectedMessage.Message);
                Messages.Remove(selectedMessage);
            }
        });

        private MessageViewModel selectedMessage;
        /// <summary>
        /// Gets or sets the selected message.
        /// </summary>
        /// <value>
        /// The selected message.
        /// </value>
        public MessageViewModel SelectedMessage
        {
            get => selectedMessage;
            set
            {
                if (selectedMessage != value)
                {
                    selectedMessage = value;
                    OnPropertyChanged(nameof(SelectedMessage));
                    Navigation.PushAsync(new MessageView(selectedMessage));
                }
            }
        }

        /// <summary>
        /// Loads the messages.
        /// </summary>
        private async void LoadMessages()
        {
            await loader.Login();
            List<ImapMessageInfo> msgs = await loader.LoadMessages();
            Messages.Clear();
            try
            {
                foreach (var i in msgs)
                {
                    Messages.Add(new MessageViewModel(i));
                }
            }
            catch
            {
                await page.DisplayAlert("Памылка!", "Магчыма, дрэннае інтэрнэт-злучэнне", "Добра");
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
