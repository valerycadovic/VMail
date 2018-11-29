using Email.Imap;
using System.ComponentModel;
using Xamarin.Forms;

namespace VMail.NewVMail.ViewModels
{
    /// <summary>
    /// MVVM ViewModel implementation
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class MessageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ImapMessageInfo Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageViewModel"/> class.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public MessageViewModel(ImapMessageInfo msg)
        {
            Message = msg;
        }

        private ListViewModel vm;
        /// <summary>
        /// Gets or sets the ListView model.
        /// </summary>
        /// <value>
        /// The ListView model.
        /// </value>
        public ListViewModel ListViewModel
        {
            get => vm;
            set
            {
                if (vm != value)
                {
                    vm = value;
                    OnPropertyChanged(nameof(ListViewModel));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is seen.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is seen; otherwise, <c>false</c>.
        /// </value>
        public bool IsSeen => Message.IsRead;

        /// <summary>
        /// Gets the text HTML.
        /// </summary>
        /// <value>
        /// The text HTML.
        /// </value>
        public string TextHtml => Message.Message.TextHtml;

        /// <summary>
        /// Gets the text plain.
        /// </summary>
        /// <value>
        /// The text plain.
        /// </value>
        public string TextPlain => Message.Message.TextPlain;

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public string Subject => Message.Message.Subject;

        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string From => Message.Message.From;

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>
        /// To.
        /// </value>
        public string To => string.Join(", ", Message.Message.To);

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public string Date => Message.Message.Date.ToString();

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
