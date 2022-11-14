using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace sqsClient
{
    public partial class SaveMessages : UserControl
    {
        private DataControllerService _DataControllerService = DataControllerService.GetInstance();

        
        
        public SaveMessages()
        {
            InitializeComponent();
            
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Title = "Save Messages";
            saveFileDialog.DefaultExt = "json";
            saveFileDialog.Filter = "Json File (*.json)|*.json|Any (*.*)|*.*";
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.ShowDialog();
            if(saveFileDialog.FileName != "") {      
                var fullPath = saveFileDialog.FileName;

                var messages = _DataControllerService.GetCachedMessages();
                var messageBodies = messages.Select(msg => JsonSerializer.Deserialize<JsonElement>(msg.Body)).ToList();
                var jsonData = JsonSerializer.Serialize(messages);
                File.WriteAllText(fullPath, jsonData);
            } 
        }
    }
}