using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace sqsClient
{
    public partial class DeleteMessages : UserControl
    {
        public DataControllerService DataControllerService = DataControllerService.GetInstance();
        
        public DeleteMessages()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await DataControllerService.DeleteMessages();
        }
    }
}