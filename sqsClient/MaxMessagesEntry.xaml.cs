using System;
using System.Windows;
using System.Windows.Controls;

namespace sqsClient
{
    public partial class MaxMessagesEntry : UserControl
    {
        private DataControllerService _DataControllerService = DataControllerService.GetInstance();
        private string MaxMessagesEntryName { get; set; } = "MaxMessages";
        private int DefaultValue = 5;
        
        public MaxMessagesEntry()
        {
            InitializeComponent();
            MaxMessages.Text = DefaultValue.ToString();
        }

        private void MaxMessages_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var notEmpty = MaxMessages.Text != String.Empty;
            var isInt = int.TryParse(MaxMessages.Text, out int intOutParameter);
            var isValid = notEmpty & isInt;
            _DataControllerService.PushEntryChange(MaxMessagesEntryName, isValid, MaxMessages.Text);
        }
    }
}