using Microsoft.Win32;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Commands;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Data;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Helpers;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Services;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Telemetry;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels
{
    public class AiChatViewModel : ObservableObject
    {
        public event Action LogoutRequested;

        private readonly DatabaseService _databaseService;
        private readonly ChatRepository _chatRepository;
        private readonly GeminiService _geminiService;

        private readonly AudioRecordingService _audioRecordingService;
        private CancellationTokenSource _requestCancellationTokenSource;
        private readonly GeminiTranscriptionService _geminiTranscriptionService;

        private int _chatId;
            private string _title;
            private DateTime _createdDate;
            private string _messageText;

            private bool _isSending;
            private string _errorMessage;
            private bool _hasError;
            private string _lastFailedMessage;
            private string _lastFailedAttachmentPath;



        // ATTACHMENT


        private string _attachedFilePath;
        private string _attachedFileName;
        private string _attachedFileSize;
        private string _attachedFileExtension;



        #region Properties

        private string _currentUserName;
        public string CurrentUserName
        {
            get { return _currentUserName; }
            set
            {
                if (_currentUserName == value)
                    return;

                _currentUserName = value;
                OnPropertyChanged(nameof(CurrentUserName));
            }
        }


        private string _currentUserProfilePicture;
        public string CurrentUserProfilePicture
        {
            get { return _currentUserProfilePicture; }
            set
            {
                if (_currentUserProfilePicture == value)
                    return;

                _currentUserProfilePicture = value;
                OnPropertyChanged(nameof(CurrentUserProfilePicture));
            }
        }


        // MicroPhone Voice
        private bool _isListening;

        public bool IsListening
        {
            get => _isListening;
            set
            {
                if (_isListening == value)
                    return;

                _isListening = value;
                OnPropertyChanged();
            }
        }




        public string AttachedFilePath
        {
            get => _attachedFilePath;
            private set
            {
                if (_attachedFilePath == value)
                    return;

                _attachedFilePath = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasAttachment));
            }
        }

        public string AttachedFileName
        {
            get => _attachedFileName;
            private set
            {
                if (_attachedFileName == value)
                    return;

                _attachedFileName = value;
                OnPropertyChanged();
            }
        }

        public string AttachedFileSize
        {
            get => _attachedFileSize;
            private set
            {
                if (_attachedFileSize == value)
                    return;

                _attachedFileSize = value;
                OnPropertyChanged();
            }
        }

        public string AttachedFileExtension
        {
            get => _attachedFileExtension;
            private set
            {
                if (_attachedFileExtension == value)
                    return;

                _attachedFileExtension = value;
                OnPropertyChanged();
            }
        }

        public bool HasAttachment =>
            !string.IsNullOrWhiteSpace(AttachedFilePath);

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            private set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasError
        {
            get
            {
                return _hasError;
            }
            private set
            {
                if (_hasError != value)
                {
                    _hasError = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedChatTitle;

        public string SelectedChatTitle
        {
            get
            {
                return _selectedChatTitle;
            }
            set
            {
                if (_selectedChatTitle != value)
                {
                    _selectedChatTitle = value;
                    OnPropertyChanged();
                }
            }
        }
        public int ChatId
            {
                get
                {
                    return _chatId;
                }
                set
                {
                    if (_chatId != value)
                    {
                        _chatId = value;
                        OnPropertyChanged();
                    }
                }
            }


            public string Title
            {
                get
                {
                    return _title;
                }
                set
                {
                    if (_title != value)
                    {
                        _title = value;
                        OnPropertyChanged();
                    }
                }
            }

     


        public DateTime CreatedDate
            {
                get
                {
                    return _createdDate;
                }
                set
                {
                    if (_createdDate != value)
                    {
                        _createdDate = value;
                        OnPropertyChanged();
                    }
                }
            }


          
            public string MessageText
            {
                get
                {
                    return _messageText;
                }
                set
                {
                    if (_messageText != value)
                    {
                        _messageText = value;
                        OnPropertyChanged();

                        CommandManager.InvalidateRequerySuggested();
                        OnPropertyChanged(nameof(CanSendMessage));
                }
                }
            }



        private bool _isAiTyping = false;

        public bool IsAiTyping
        {
            get
            {
                return _isAiTyping;
            }
            set
            {
                if (_isAiTyping == value)
                    return;

                _isAiTyping = value;
                OnPropertyChanged(nameof(IsAiTyping));
                OnPropertyChanged(nameof(CanSendMessage));
            }
        }


        public bool IsEmptyChat
        {
            get
            {
                return Messages == null || Messages.Count == 0;
            }
        }

     
        public bool IsSending
        {
            get { return _isSending; }
            set
            {
                if (_isSending != value)
                {
                    _isSending = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanSendMessage));

                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }


        public ObservableCollection<MessageViewModel> Messages
            {
                get;
            } = new ObservableCollection<MessageViewModel>();

            public ObservableCollection<Chat> Chats
            {
                get;
            } = new ObservableCollection<Chat>();

            

   
        private void Messages_CollectionChanged(
        object sender,
        NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsEmptyChat));
        }

        #endregion


        #region Commands

        public ICommand SendMessageCommand
            {
                get;
            }

            public ICommand OpenChatCommand
            {
                get;
            }

            public ICommand NewChatCommand { get; }

            public ICommand RetryCommand { get; }

            

            public ICommand MinimizeWindowCommand { get; }
            public ICommand CloseWindowCommand { get; }

            public ICommand RenameChatCommand { get; private set; }
            public ICommand FinishRenameCommand { get; private set; }
            public ICommand DeleteChatCommand { get; private set; }
            public bool IsTimeoutError { get; private set; }
            public bool IsRetryVisible { get; private set; }

            public ICommand AttachFileCommand { get; }
            public ICommand RemoveAttachmentCommand { get; }
            public ICommand StartVoiceCommand { get; }
            public ICommand SettingsCommand { get; }
            public ICommand LogoutCommand { get; }






        #endregion


        #region Constructor

        public AiChatViewModel()
        {


            _databaseService = new DatabaseService();

            _databaseService.InitializeDatabase();

            _chatRepository = new ChatRepository(_databaseService);


            _audioRecordingService = new AudioRecordingService();

            _geminiTranscriptionService =
       new GeminiTranscriptionService();



            _geminiService = new GeminiService();


            Messages.CollectionChanged += Messages_CollectionChanged;


            ChatId = 0;
            Title = string.Empty;
            SelectedChatTitle = string.Empty;
            CreatedDate = DateTime.Now;

            MessageText = string.Empty;

            IsSending = false;



            SendMessageCommand = new RelayCommand(
                    async parameter => await SendMessageAsync(),
                    parameter => CanSendMessage() && !IsSending
                );


            OpenChatCommand = new RelayCommand(
                parameter => OpenExistingChat(parameter as Chat)
                );

            NewChatCommand = new RelayCommand(
            parameter => CreateNewChat());


            MinimizeWindowCommand = new RelayCommand(MinimizeWindow);
            CloseWindowCommand = new RelayCommand(CloseWindow);

            RenameChatCommand = new RelayCommand(
             parameter => RenameChat(parameter as Chat),
             parameter => parameter is Chat);

            FinishRenameCommand = new RelayCommand(
            parameter => FinishRename(parameter as Chat),
            parameter => parameter is Chat);

            DeleteChatCommand = new RelayCommand(
                parameter => DeleteChat(parameter as Chat),
                parameter => parameter is Chat);

                RetryCommand = new RelayCommand(
            parameter => RetryMessage(),
            parameter => CanRetryMessage());

            AttachFileCommand = new RelayCommand(
    parameter => AttachFile());

            RemoveAttachmentCommand = new RelayCommand(
                parameter => RemoveAttachment());

            StartVoiceCommand = new RelayCommand(ToggleVoiceInput);

            SettingsCommand = new RelayCommand(OpenSettings);

            LogoutCommand =
     new RelayCommand(_ => ExecuteLogout());

            LoadCurrentUserProfile();

            LoadChatHistory();
        }


        private void LoadCurrentUserProfile()
        {
            if (!CurrentUserSession.IsAuthenticated)
                return;

            CurrentUserName = CurrentUserSession.Name;
            CurrentUserProfilePicture =
                CurrentUserSession.ProfilePicture;
        }


        private void ExecuteLogout()
        {
            TelemetryService.Instance.TrackEvent(
            TelemetryEventName.UserLoggedOut,
            TelemetryEventCategory.UserAction,
            new Dictionary<string, object>
            {
                { "SourceTrigger", "AiChatView" },
                { "ButtonType", "Button" }
            });

            MessageBoxResult result = MessageBox.Show(
            "Are you sure you want to logout?",
            "Confirm Logout",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            CurrentUserSession.Clear();
            LogoutRequested?.Invoke();
        }
        public void OpenSettings(object parameter)
        {
            TelemetryService.Instance.TrackEvent(
            TelemetryEventName.SettingsOpened,
            TelemetryEventCategory.UserAction,
            new Dictionary<string, object>
            {
                { "SourceTrigger", "AiChatView" }
            });

            var settingsView = new SettingsView();
            
        }


        private string RestoreAttachmentFile(
        byte[] attachmentData,
        string attachmentFileName,
        string attachmentExtension)
        {
            if (attachmentData == null ||
                attachmentData.Length == 0)
            {
                return null;
            }

            string extension =
                attachmentExtension;

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".bin";
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string attachmentFolder =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "RestoredAttachments");

            Directory.CreateDirectory(
                attachmentFolder);

            string fileName =
                Guid.NewGuid().ToString("N")
                + extension;

            string filePath =
                Path.Combine(
                    attachmentFolder,
                    fileName);

            File.WriteAllBytes(
                filePath,
                attachmentData);

            return filePath;
        }


        private async void ToggleVoiceInput(
      object parameter)
        {
            TelemetryService.Instance.TrackEvent(
            TelemetryEventName.MicrophoneButtonClicked,
            TelemetryEventCategory.UserAction,
            new Dictionary<string, object>
            {
                { "SourceTrigger", "AiChatView" }
            });

            try
            {
                // ==========================================
                // STOP RECORDING
                // ==========================================

                if (IsListening)
                {
                    IsListening = false;

                    byte[] audioData =
                        await _audioRecordingService
                            .StopRecordingAsync();

                    if (audioData == null ||
                        audioData.Length == 0)
                    {
                        return;
                    }

                    // ==========================================
                    // SPEECH → TEXT
                    // ==========================================

                    string transcription =
                        await _geminiTranscriptionService
                            .TranscribeAsync(audioData);

                    // ==========================================
                    // DISPLAY TEXT IN EXISTING TEXTBOX
                    // ==========================================

                    if (!string.IsNullOrWhiteSpace(
                            transcription))
                    {
                        MessageText =
                            transcription.Trim();
                    }

                    return;
                }

                // ==========================================
                // START RECORDING
                // ==========================================

                _audioRecordingService
                    .StartRecording();

                IsListening = true;
            }
            catch (Exception ex)
            {
                IsListening = false;

                MessageBox.Show(
                    "Unable to process microphone input.\n\n" +
                    ex.Message,
                    "Voice Input Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                CommandManager
                    .InvalidateRequerySuggested();
            }
        }

        private void AttachFile()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Attach a file",
                Multiselect = false,

                Filter =
                    "Supported Files|" +
                    "*.pdf;*.doc;*.docx;*.txt;*.csv;" +
                    "*.xls;*.xlsx;" +
                    "*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp|" +

                    "Documents|" +
                    "*.pdf;*.doc;*.docx;*.txt;*.csv;*.xls;*.xlsx|" +

                    "Images|" +
                    "*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.webp|" +

                    "All Files|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                FileInfo fileInfo = new FileInfo(dialog.FileName);

                // Maximum file size = 20 MB
                const long maxFileSize = 20 * 1024 * 1024;

                if (fileInfo.Length > maxFileSize)
                {
                    MessageBox.Show(
                        "The selected file is larger than 20 MB.",
                        "File Too Large",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                AttachedFilePath = fileInfo.FullName;
                AttachedFileName = fileInfo.Name;
                AttachedFileSize = FormatFileSize(fileInfo.Length);
                AttachedFileExtension =
                    fileInfo.Extension.ToLowerInvariant();

                TelemetryService.Instance.TrackEvent(
                TelemetryEventName.FileAttachmentAdded,
                TelemetryEventCategory.UserAction,
                new Dictionary<string, object>
                {
                    { "FileExtension", fileInfo.Extension.ToLowerInvariant() },
                    { "FileSizeBytes", fileInfo.Length },
                    { "SourceTrigger", "AiChatView" }
                });

                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to attach the selected file.\n\n" +
                    ex.Message,
                    "Attachment Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} B";
            }

            if (bytes < 1024 * 1024)
            {
                return $"{bytes / 1024.0:F1} KB";
            }

            if (bytes < 1024 * 1024 * 1024)
            {
                return $"{bytes / (1024.0 * 1024.0):F1} MB";
            }

            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }


        private void RemoveAttachment()
        {
            AttachedFilePath = null;
            AttachedFileName = null;
            AttachedFileSize = null;
            AttachedFileExtension = null;

            CommandManager.InvalidateRequerySuggested();
        }


        #endregion
        private bool CanSendMessage()
        {
            return !IsSending &&
                   (
                       !string.IsNullOrWhiteSpace(MessageText) ||
                       HasAttachment
                   );
        }


        private bool CanRetryMessage()
        {
            return HasError &&
                   !IsSending &&
                   (
                   !string.IsNullOrWhiteSpace(_lastFailedMessage) ||
                   !string.IsNullOrWhiteSpace(_lastFailedAttachmentPath)
                   );
        }


        private void RenameChat(Chat chat)
        {
            if (chat == null)
                return;

            chat.IsRenaming = true;
        }

        public void DeleteChat(Chat chat)
        {
            if (chat == null)
                return;

            if (chat.ChatId <= 0)
                return;

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete this chat history?",
                "Delete Chat",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                
                // 1. DELETE CHAT FROM SQLITE
                

                bool deleted = _chatRepository.DeleteChat(chat.ChatId, CurrentUserSession.UserId);

                if (!deleted)
                {
                    MessageBox.Show(
                        "Unable to delete the chat.",
                        "Delete Chat",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                TelemetryService.Instance.TrackEvent(
                TelemetryEventName.ChatDeleted,
                TelemetryEventCategory.UserAction,
                new Dictionary<string, object>
                {
                    { "SourceTrigger", "AiChatView" }
                });


                // 2. REMOVE CHAT FROM CHAT HISTORY


                Chats.Remove(chat);

                
                // 3. IF DELETED CHAT IS CURRENTLY OPEN
                

                if (ChatId == chat.ChatId)
                {
                    ChatId = 0;
                    Title = string.Empty;
                    SelectedChatTitle = string.Empty;
                    Messages.Clear();
                    MessageText = string.Empty;

                    IsAiTyping = false;
                    IsSending = false;
                }

                
                // 4. IF NO CHAT HISTORY REMAINS
                //    NAVIGATE TO MAINWINDOW HOME SCREEN
                

                if (Chats.Count == 0)
                {
                    NavigateToHome();
                }

                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to delete the chat.\n\n" + ex.Message,
                    "Delete Chat",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void FinishRename(Chat chat)
        {
            if (chat == null)
                return;

            string newTitle = chat.Title == null
                ? string.Empty
                : chat.Title.Trim();

            if (string.IsNullOrWhiteSpace(newTitle))
            {
                return;
            }

            // Update UI model
            chat.Title = newTitle;

            // Update SQLite Chats table
            if (chat.ChatId > 0)
            {
                bool updated = _chatRepository.UpdateChatTitle(
                    chat.ChatId,
                    CurrentUserSession.UserId,
                    newTitle);

                if (!updated)
                {
                    return;
                }
            }

            TelemetryService.Instance.TrackEvent(
            TelemetryEventName.ChatRenamed,
            TelemetryEventCategory.UserAction,
            new Dictionary<string, object>
            {
                { "NameLength", newTitle.Length },
                { "SourceTrigger", "AiChatView" }
            });


            // Update main chat header
            if (ChatId == chat.ChatId)
            {
                Title = newTitle;
                SelectedChatTitle = newTitle;
            }

            // TextBox -> TextBlock
            chat.IsRenaming = false;
        }


        private void NavigateToHome()
        {
            if (Application.Current?.MainWindow?.DataContext is MainViewModel mainViewModel)
            {
                // Remove the current AiChatView from ContentControl
                mainViewModel.CurrentView = null;

                // Clear MainWindow input
                mainViewModel.MessageText = string.Empty;

                // Clear header title
                mainViewModel.SelectedChatTitle = string.Empty;


                // Refresh command states
                CommandManager.InvalidateRequerySuggested();
            }
        }




        private void MinimizeWindow(object parameter)
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.WindowState =
                    WindowState.Minimized;
            }
        }

        private void CloseWindow(object parameter)
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Close();
            }
        }


        private void LoadChatHistory()
        {
            Chats.Clear();

            List<Chat> chats = _chatRepository.GetChats(CurrentUserSession.UserId);

            foreach (Chat chat in chats)
            {
                Chats.Add(chat);
            }
        }


        #region Send Message




        private async Task SendMessageAsync()
        {
            if (IsSending)
                return;

            if (!CanSendMessage())
                return;

            // Capture BOTH before clearing anything.
            string message = MessageText?.Trim() ?? string.Empty;
            string attachmentPath = AttachedFilePath;

            // Nothing to send.
            if (string.IsNullOrWhiteSpace(message) &&
                string.IsNullOrWhiteSpace(attachmentPath))
            {
                return;
            }

            TelemetryService.Instance.TrackEvent(
           TelemetryEventName.ChatMessageSent,
           TelemetryEventCategory.UserAction,
           new Dictionary<string, object>
           {
                { "MessageLength", message.Length },
                { "HasAttachment", !string.IsNullOrWhiteSpace(attachmentPath) },
                { "SourceTrigger", "AiChatView" }
           });

            // Attachment-only message.
            if (string.IsNullOrWhiteSpace(message) &&
                !string.IsNullOrWhiteSpace(attachmentPath))
            {
                message = "Please analyze the attached file.";
            }

            // Save BOTH for Retry.
            _lastFailedMessage = message;
            _lastFailedAttachmentPath = attachmentPath;

            // Clear only the TextBox.
            MessageText = string.Empty;

            IsSending = true;
            IsAiTyping = true;

            try
            {
                await RequestAiResponseAsync(
                    message,
                    true,
                    attachmentPath);
            }
            finally
            {
                IsAiTyping = false;
                IsSending = false;

                CommandManager.InvalidateRequerySuggested();
            }
        }


        private async void RetryMessage()
        {
            if (IsSending)
                return;

            if (!HasError)
                return;

            if (string.IsNullOrWhiteSpace(_lastFailedMessage))
                return;

            string message = _lastFailedMessage;

            string attachmentPath =
    _lastFailedAttachmentPath;

            IsSending = true;
            IsAiTyping = true;

            // Immediately remove the previous timeout UI.
            ClearError();

            IsTimeoutError = false;
            IsRetryVisible = false;

            try
            {
                await RequestAiResponseAsync(
                message,
                false,
                attachmentPath);
            }
            finally
            {
                IsAiTyping = false;
                IsSending = false;

                CommandManager.InvalidateRequerySuggested();
            }
        }



        private async Task RequestAiResponseAsync(
         string message,
         bool addUserMessage,
         string attachmentPath = null)
        {

            if (string.IsNullOrWhiteSpace(message) &&
        string.IsNullOrWhiteSpace(attachmentPath))
            {
                return;
            }

            if (addUserMessage)
            {
                AddUserMessage(
                        message,
                        attachmentPath,
                        AttachedFileName,
                        AttachedFileSize,
                        AttachedFileExtension);
            }

            ClearError();

            IsTimeoutError = false;
            IsRetryVisible = false;

            // Cancel previous request if one exists.
            if (_requestCancellationTokenSource != null)
            {
                _requestCancellationTokenSource.Cancel();
                _requestCancellationTokenSource.Dispose();
                _requestCancellationTokenSource = null;
            }

            // Create a NEW cancellation token for THIS request.
            _requestCancellationTokenSource =
                new CancellationTokenSource();

            CancellationToken cancellationToken =
                _requestCancellationTokenSource.Token;

            try
            {


                string response;

                if (!string.IsNullOrWhiteSpace(attachmentPath))
                {
                    response =
                        await _geminiService.SendMessageWithAttachmentAsync(
                            message,
                            attachmentPath,
                            cancellationToken);
                }
                else
                {
                    response =
                        await _geminiService.SendMessageAsync(
                            message,
                            cancellationToken);
                }

                if (string.IsNullOrWhiteSpace(response))
                {
                    ShowError(
                        "The AI returned an empty response.");

                    return;
                }

                // Show AI response immediately.
                AddAIMessage(response);

                // Save only after successful response.
                SaveSuccessfulConversation(
                    message,
                    response,
                    attachmentPath);

                _lastFailedMessage = null;
                _lastFailedAttachmentPath = null;

                IsTimeoutError = false;
                IsRetryVisible = false;

                ClearError();

                // Close attachment composer ONLY after
                // Gemini successfully receives and processes
                // the message + attachment.
                if (!string.IsNullOrWhiteSpace(attachmentPath))
                {
                    RemoveAttachment();
                }
            }
            catch (TimeoutException)
            {
                // Keep the original user question for Retry.
                _lastFailedMessage = message;

                IsTimeoutError = true;
                IsRetryVisible = true;

                ShowError(
                    "The AI request timed out. Please try again.");
            }
            catch (OperationCanceledException)
            {
                /*
                 * Cancellation caused by the user/application
                 * is NOT treated as a timeout.
                 */
                if (!cancellationToken.IsCancellationRequested)
                {
                    _lastFailedMessage = message;

                    IsTimeoutError = true;
                    IsRetryVisible = true;

                    ShowError(
                        "The AI request timed out. Please try again.");
                }
            }
            catch (GeminiApiException ex)
            {
                HandleHttpError(ex);
            }
            catch (HttpRequestException)
            {
                ShowError(
                    "Unable to connect to the AI service. " +
                    "Please check your internet connection.");
            }
            catch (Exception)
            {
                ShowError(
                    "Something went wrong while generating " +
                    "the AI response.");
            }
            finally
            {
                if (_requestCancellationTokenSource != null)
                {
                    _requestCancellationTokenSource.Dispose();
                    _requestCancellationTokenSource = null;
                }

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage = message;
            HasError = true;

            CommandManager.InvalidateRequerySuggested();
        }

        private void ClearError()
        {
            ErrorMessage = string.Empty;
            HasError = false;

            CommandManager.InvalidateRequerySuggested();
        }

        private void HandleHttpError(
    GeminiApiException exception)
        {
            switch (exception.StatusCode)
            {
                case HttpStatusCode.BadRequest:

                    ShowError(
                        "The AI request was invalid.");

                    break;

                case HttpStatusCode.Unauthorized:

                case HttpStatusCode.Forbidden:

                    ShowError(
                        "Unable to authenticate with the AI service.");

                    break;

                case (HttpStatusCode)429:

                    ShowError(
                        "Too many AI requests. " +
                        "Please wait a moment and try again.");

                    break;

                case HttpStatusCode.InternalServerError:

                case HttpStatusCode.BadGateway:

                case HttpStatusCode.ServiceUnavailable:

                case HttpStatusCode.GatewayTimeout:

                    ShowError(
                        "The AI service is temporarily unavailable. " +
                        "Please try again.");

                    break;

                default:

                    ShowError(
                        "Unable to connect to the AI service. " +
                        "Please check your internet connection.");

                    break;
            }
        }

      

        public void AddAIMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            DateTime timestamp = DateTime.Now;

            // Add AI message to UI
            Messages.Add(
                new MessageViewModel
                {
                    SenderType = "AI",
                    MessageText = message,
                    Timestamp = timestamp
                }); 
        }


        #endregion


        #region Message Methods

        public void AddUserMessage(
             string message,
            string attachmentPath = null,
            string attachmentName = null,
            string attachmentSize = null,
            string attachmentExtension = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            
            // CREATE CHAT ONLY FOR THE FIRST USER MESSAGE
            
            if (Messages.Count == 0 && ChatId == 0)
            {
                string chatTitle = message.Trim();

                // Keep title short
                if (chatTitle.Length > 40)
                {
                    chatTitle = chatTitle.Substring(0, 40) + "...";
                }

                // Set title
                Title = chatTitle;
                SelectedChatTitle = chatTitle;

                CreatedDate = DateTime.Now;                
            }      
            DateTime timestamp = DateTime.Now;

            Messages.Add(
                new MessageViewModel
                {
                    SenderType = "User",
                    MessageText = message,
                    Timestamp = timestamp,

                    AttachmentPath = attachmentPath,
                    AttachmentName = attachmentName,
                    AttachmentSize = attachmentSize,
                    AttachmentExtension = attachmentExtension
                });

        }


        private void SaveSuccessfulConversation(
        string userMessage,
        string aiResponse,
        string attachmentPath = null)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return;

            if (string.IsNullOrWhiteSpace(aiResponse))
                return;

            
            // CREATE CHAT ONLY AFTER AI SUCCESS
            

            if (ChatId == 0)
            {
                string chatTitle =
                    userMessage.Trim();

                if (chatTitle.Length > 40)
                {
                    chatTitle =
                        chatTitle.Substring(0, 40) + "...";
                }

                Title = chatTitle;
                SelectedChatTitle = chatTitle;
                CreatedDate = DateTime.Now;

                
                // CREATE CHAT IN SQLITE
                

                ChatId =
                    _chatRepository.CreateChat(CurrentUserSession.UserId, Title);

                if (ChatId <= 0)
                {
                    throw new Exception(
                        "Unable to create chat in SQLite.");
                }

                
                // CREATE CHAT HISTORY CARD
                

                Chats.Insert(
                    0,
                    new Chat
                    {
                        ChatId = ChatId,
                        Title = chatTitle,
                        CreatedDate = CreatedDate
                    });
            }
            else
            {
                
                // EXISTING CHAT
                

                // Make sure title is available
                if (string.IsNullOrWhiteSpace(Title))
                {
                    Chat existingChat =
                        _chatRepository.GetChatById(ChatId, CurrentUserSession.UserId);

                    if (existingChat != null)
                    {
                        Title = existingChat.Title;
                        SelectedChatTitle = existingChat.Title;
                    }
                }
            }


            // SAVE USER MESSAGE

            byte[] attachmentData = null;

            if (!string.IsNullOrWhiteSpace(attachmentPath) &&
                File.Exists(attachmentPath))
            {
                attachmentData =
                    File.ReadAllBytes(attachmentPath);
            }


            _chatRepository.SaveMessage(
                ChatId,
                CurrentUserSession.UserId,
                "User",
                userMessage,
                AttachedFileName,
                AttachedFileExtension,
                AttachedFileSize,
                attachmentData);


            // SAVE AI RESPONSE


            _chatRepository.SaveMessage(
                ChatId,
                CurrentUserSession.UserId,
                "AI",
                aiResponse);

            
            // UPDATE CHAT HISTORY CARD
            

            Chat historyChat =
                Chats.FirstOrDefault(
                    c => c.ChatId == ChatId);

            if (historyChat != null)
            {
                historyChat.Title = Title;

                OnPropertyChanged(nameof(Chats));
            }

        }
        #endregion


        #region New Chat

        private void CreateNewChat()
        {
            TelemetryService.Instance.TrackEvent(
            TelemetryEventName.NewChatStarted,
            TelemetryEventCategory.UserAction,
            new Dictionary<string, object>
            {
                { "SourceTrigger", "AiChatView" }
            });

            // Stop/reset current chat state
            IsAiTyping = false;
            IsSending = false;

            // Clear all messages from current conversation
            Messages.Clear();

            // Clear message input
            MessageText = string.Empty;

            RemoveAttachment();

            _lastFailedMessage = null;
            _lastFailedAttachmentPath = null;

            // Reset chat information

            ChatId = 0;
            Title = string.Empty;
            SelectedChatTitle = string.Empty;
            CreatedDate = DateTime.Now;

            OnPropertyChanged(nameof(IsEmptyChat));

            // Refresh command states
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion


        #region Public Methods

     
        public void LoadChat(
                int chatId,
                string title,
                DateTime createdDate,
                ObservableCollection<MessageViewModel> messages)
            {
                ChatId = chatId;
                Title = title;
                CreatedDate = createdDate;

                Messages.Clear();

                if (messages != null)
                {
                    foreach (MessageViewModel message in messages)
                    {
                        Messages.Add(message);
                    }
                }

                MessageText = string.Empty;
            }


            /// <summary>
            /// Clears the current conversation.
            /// </summary>
            public void ClearMessages()
            {
                Messages.Clear();

                MessageText = string.Empty;

                IsAiTyping = false;
                IsSending = false;
            }

        private void OpenExistingChat(Chat chat)
        {
            if (chat == null)
            {
                return;
            }

            
            // Current Chat Information
            

            ChatId = chat.ChatId;

            Title = chat.Title;

            SelectedChatTitle = chat.Title;

            CreatedDate = chat.CreatedDate;

            MessageText = string.Empty;

            IsAiTyping = false;

            IsSending = false;

            // Clear current UI messages
            

            Messages.Clear();

            // Load messages from SQLite
            

            List<Message> messages =
                _chatRepository.GetMessages(ChatId, CurrentUserSession.UserId);


            foreach (Message message in messages)
            {
                string attachmentPath = null;

                if (message.AttachmentData != null &&
                    message.AttachmentData.Length > 0)
                {
                    attachmentPath =
                        RestoreAttachmentFile(
                            message.AttachmentData,
                            message.AttachmentFileName,
                            message.AttachmentExtension);
                }

                Messages.Add(
                    new MessageViewModel
                    {
                        MessageId = message.MessageId,

                        ChatId = message.ChatId,

                        SenderType = message.SenderType,

                        MessageText = message.MessageText,

                        Timestamp = message.Timestamp,

                        AttachmentPath = attachmentPath,

                        AttachmentName =
                            message.AttachmentFileName,

                        AttachmentSize =
                            message.AttachmentSize,

                        AttachmentExtension =
                            message.AttachmentExtension
                    });
            }


            CommandManager.InvalidateRequerySuggested();

        }

       


        #endregion
    }
    }

