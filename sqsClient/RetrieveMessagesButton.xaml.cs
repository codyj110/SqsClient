using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace sqsClient
{
    public partial class RetrieveMessagesButton : UserControl
    {
        private DataControllerService _DataControllerService = DataControllerService.GetInstance();

        public bool RegionValid { get; set; }
        public bool AccountValid { get; set; }
        public bool QueueNameValid { get; set; } 
        public bool MaxMessagesValid { get; set; }
        
        public RetrieveMessagesButton()
        {
            InitializeComponent();
            _DataControllerService.EntryChangeEvent += EntryChangeHandler;
        }
        
        private void EntryChangeHandler(object sender, string name, bool valid, string value)
        {
            if (name.ToLower() == "region")
            {
                RegionValid = valid;
            }
            
            if (name.ToLower() == "account")
            {
                AccountValid = valid;
            }
            
            if (name.ToLower() == "queuename")
            {
                QueueNameValid = valid;
            }

            if (name.ToLower() == "maxmessages")
            {
                MaxMessagesValid = valid;
            }

            RetrieveButton.IsEnabled = EntriesValid();
        }

        private async void LoadMessages_OnClick(object sender, RoutedEventArgs e)
        {
            if (!EntriesValid())
            {
                // DisplayError("Entry Invalid");
                return;
            }

            await _DataControllerService.GetMessages();

            // try
            // {
            //     topWindow.IsEnabled = false;
            //     var region = Region.Text;
            //     var account = Account.Text;
            //     var queName = QueName.Text;
            //     var maxMessages = int.Parse(MaxMessages.Text);
            //
            //     var messages = await DataControllerService
            //         .GetMessages(region, account, queName, maxMessages);
            //
            //     ClearDisplay();
            //     DisplayMessages(messages);
            //     topWindow.IsEnabled = true;
            // }
            // catch (Exception exception)
            // {
            //     DisplayError(exception.Message);
            // }
        }

        private bool EntriesValid()
        {
            return AccountValid & RegionValid & QueueNameValid & MaxMessagesValid;
        }
    }
}