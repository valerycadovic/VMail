using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using VMail.Models;

namespace VMail.ViewModels
{
    public class ReadMessageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private MainViewModel vm;

        public Message Message { get; set; }

        public ReadMessageViewModel(Message message)
        {
            Message = message;
        }

        public MainViewModel ListViewModel
        {
            get => vm;
            set
            {
                if(vm != value)
                {
                    vm = value;
                    OnPropertyChanged(nameof(ListViewModel));
                }
            }
        }

        public string Body
        {
            get => Message.Body;
            set
            {
                if(Message.Body != value)
                {
                    Message.Body = value;
                    OnPropertyChanged(nameof(Body));
                }
            }
        }

        public string Subject
        {
            get => Message.Subject;
            set
            {
                if (Message.Subject != value)
                {
                    Message.Subject = value;
                    OnPropertyChanged(nameof(Subject));
                }
            }
        }

        public string From
        {
            get => Message.From;
            set
            {
                if (Message.From != value)
                {
                    Message.From = value;
                    OnPropertyChanged(nameof(From));
                }
            }
        }

        public string To
        {
            get => Message.To;
            set
            {
                if (Message.To != value)
                {
                    Message.To = value;
                    OnPropertyChanged(nameof(To));
                }
            }
        }

        public string Date
        {
            get => Message.Date;
            set
            {
                if (Message.Date != value)
                {
                    Message.Date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public string MailBox
        {
            get => Message.MailBox;
            set
            {
                if (Message.MailBox != value)
                {
                    Message.MailBox = value;
                    OnPropertyChanged(nameof(MailBox));
                }
            }
        }

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
