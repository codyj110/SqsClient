using System.Windows;
using System.Windows.Controls;

namespace sqsClient
{
    public partial class AccountEntry : UserControl
    {
        private DataControllerService _DataControllerService = DataControllerService.GetInstance();
        private string AccountEntryName { get; set; } = "Account";
        private string DefaultValue = "208792733595";
        
        public AccountEntry()
        {
            InitializeComponent();
            Account.Text = DefaultValue;
        }

        private void Account_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var notEmpty =  Account.Text != string.Empty;
            _DataControllerService.PushEntryChange(AccountEntryName, notEmpty, Account.Text);
        }
    }
}