using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using sqsClient.Models;

namespace sqsClient
{
    public partial class LoadMessages : UserControl
    {
        private DataControllerService _DataControllerService = DataControllerService.GetInstance();
        public LoadMessages()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            var fileContent = string.Empty;
            var filePath = string.Empty;

           
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;

                //Read the contents of the file into a stream
                var fileStream = openFileDialog.OpenFile();

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                    var messages = JsonSerializer.Deserialize<List<SqsClientMessage>>(fileContent);

                    _DataControllerService.PushMessages(messages);
                }
            }

        }
    }
}