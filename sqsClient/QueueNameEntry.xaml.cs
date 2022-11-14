using System.Windows;
using System.Windows.Controls;

namespace sqsClient
{
    public partial class QueueNameEntry : UserControl
    {
        private DataControllerService _DataControllerService = DataControllerService.GetInstance();
        private string QueueEntryName { get; set; } = "QueueName";
        
        public QueueNameEntry()
        {
            InitializeComponent();
        }

        private void QueName_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var notEmpty =  QueueName.Text != string.Empty;
            
            _DataControllerService.PushEntryChange(QueueEntryName, notEmpty, QueueName.Text);
        }
    }
}