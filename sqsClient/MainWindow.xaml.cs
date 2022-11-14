using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using sqsClient.Exceptions;
using sqsClient.Models;

// TODO as a user i need mechanism to prevent or warn me about closing window when deletion is in progress

namespace sqsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, SqsClientMessage> LoadedMessages = new Dictionary<string, SqsClientMessage>();
        public DataControllerService DataControllerService = DataControllerService.GetInstance();

        public MainWindow()
        {
            InitializeComponent();
            DataControllerService.MessagesUpdatedEvent += UpdateMessages;
            DataControllerService.MessagesDeletedEvent += DeletedMessages;
            DataControllerService.CallingApiEvent += CallingApiHandler;
            DataControllerService.RetrievedMessagesEvent += RetrievedMessagesHandler;
        }

        private void RetrievedMessagesHandler(object sender, List<SqsClientMessage> messages)
        {
            ClearDisplay();
            DisplayMessages(messages);
            topWindow.IsEnabled = true;
        }

        private void CallingApiHandler(object sender)
        {
            topWindow.IsEnabled = false;
        }

        private void UpdateMessages(object sender, List<SqsClientMessage> messages)
        {
            ClearDisplay();
            DisplayMessages(messages);
            topWindow.IsEnabled = true;
        }

        private void DeletedMessages(object sender)
        {
            ClearDisplay();
            MessageDisplay.AppendText("Messages Deleted");
            topWindow.IsEnabled = true;
        }

        private void ClearDisplay()
        {
            LoadedMessages.Clear();
            MessageBox.Items.Clear();
        }

        private void DisplayMessages(List<SqsClientMessage> messages)
        {
            try
            {
                NoMessagesGuard(messages);
                
                foreach (var sqsClientMessage in messages)
                {
                    LoadedMessages.Add(sqsClientMessage.Id, sqsClientMessage);
                    MessageBox.Items.Add(sqsClientMessage.Id);
                }
            }
            catch (NoMessagesException e)
            {
                DisplayError(e.Message);
            }
        }

        private void NoMessagesGuard(List<SqsClientMessage> messages)
        {
            if (messages.Count == 0)
            {
                throw new NoMessagesException();
            }
        }

        private void DisplayError(string message)
        {
            MessageDisplay.Document.Blocks.Clear();
            MessageDisplay.AppendText(message);
        }
        
        private void MessageBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = e.AddedItems.Cast<string>();
            MessageDisplay.Document.Blocks.Clear();
            if (LoadedMessages.Count > 0)
            {
                MessageDisplay.AppendText( FormatBody(LoadedMessages[items.First()].Body));
            }
        }

        private string FormatBody(string rawBody)
        {
            var formattedOuterBody = rawBody.Replace(@",\n", Environment.NewLine);
            var innerRegx = ",\\\"";
            var formattedInnerBody = formattedOuterBody.Replace(innerRegx, $",{Environment.NewLine}        \\\"");
            return formattedInnerBody;
        }



        private string GetAppDatapath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }
}