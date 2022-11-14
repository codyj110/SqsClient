using System.Windows;
using System.Windows.Controls;

namespace sqsClient
{
    public partial class RegionEntry : UserControl
    {
        private DataControllerService _DataControllerService = DataControllerService.GetInstance();
        private string EntryName = "Region";
        
        
        public RegionEntry()
        {
            InitializeComponent();
            
        }

        private void TextBoxBase_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var notEmpty =  Region.Text != string.Empty;
            
            _DataControllerService.PushEntryChange(EntryName, notEmpty, Region.Text);
            
        }
    }
}