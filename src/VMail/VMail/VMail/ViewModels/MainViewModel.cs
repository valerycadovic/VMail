using EmailProtocols.Imap.Realization;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using VMail.Models;
using VMail.Views;
using Xamarin.Forms;

namespace VMail.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<ReadMessageViewModel> Messages { get; set; }
        public INavigation Navigation { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<ReadMessageViewModel>();
            GetMessages();
        }
        
        private ReadMessageViewModel selectedMessage;
        public ReadMessageViewModel SelectedMessageVM
        { 
            get => selectedMessage;
            set
            {
                if (selectedMessage != value)
                {
                    //ReadMessageViewModel temp = value;
                    selectedMessage = value;
                    OnPropertyChanged(nameof(SelectedMessageVM));
                   // selectedMessage.Body = GetBody(selectedMessage.Message.Uid);
                    Navigation.PushAsync(new ReadMessagePage(selectedMessage));
                    
                }
            }
        }

        private async Task<string> GetBodyAsync(string uid)
        {
            using(ImapClient imap = new ImapClient())
            {
                bool b = await imap.Login("valerachad04@gmail.com", "testaddress1999");
                bool a = await imap.SelectFolder("inbox");

                try
                {
                    return await imap.GetBody(uid);
                }
                catch(Exception e)
                {
                    string str = e.InnerException.Message;
                    string str1 = e.StackTrace;
                    throw;
                }
            }
        }

        private string GetBody(string uid)
        {
            Task<string> a = Task.Run(async() =>
            {
                return await GetBodyAsync(uid);
            });
            a.Wait();
            return a.Result;
        }

        private async void GetMessages()
        {
            using (ImapClient imap = new ImapClient())
            {
                //if (!imap.Login(CrossSettings.Current.GetValueOrDefault("Address", ""), CrossSettings.Current.GetValueOrDefault("Password", "")))
                //   throw new Exception("no authenticated");
                bool b = await imap.Login("valerachad03@gmail.com", "valera1999");
                bool a = await imap.SelectFolder("inbox");
                string[] uids = await imap.GetUids();

                try
                {

                    Regex regexNum = new Regex(@"\d+");
                    for (int i = uids.Length - 20; i < uids.Length; i++)
                    //foreach(var item in uids)
                    {
                        var match = regexNum.Match(/*item*/uids[i]);
                        Message msg = new Message
                        {
                            Uid = match.Value
                        };

                        msg.Body = await imap.GetBody(msg.Uid);
                        msg.Subject = await imap.GetSubject(msg.Uid);
                        msg.Date = await imap.GetDate(msg.Uid);
                        msg.From = await imap.GetFrom(msg.Uid);
                        msg.To = await imap.GetTo(msg.Uid);
                        msg.MailBox = "inbox";

                        Messages.Add(new ReadMessageViewModel(msg));
                    }

                }
                catch (Exception e)
                {
                    string str = e.InnerException.Message;
                    string str1 = e.StackTrace;
                }

                await imap.Logout();
            }
        }

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
