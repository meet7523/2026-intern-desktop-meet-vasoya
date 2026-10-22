using POC16_AICHATAPPLICATION_FIGMATOWPF.Commands;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Converters;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Data;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Helpers;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Services;
using POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels;
using POC16_AICHATAPPLICATION_FIGMATOWPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly ChatRepository _chatRepository;

        private readonly GeminiService _geminiService;

        private string _messageText;
        private bool _isSending;
        private bool _isTyping;
        private bool _isEmptyChat = true;

        public ObservableCollection<MessageViewModel> Messages
        {
            get;
        } = new ObservableCollection<MessageViewModel>();

        public ObservableCollection<AiChatViewModel> Chats { get; }
            = new ObservableCollection<AiChatViewModel>();




        private object _currentView;

        public object CurrentView
        {
            get
            {
                return _currentView;
            }
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsHomeScreenVisible));
                }
            }
        }

        private AiChatViewModel _selectedChat;

        public AiChatViewModel SelectedChat
        {
            get
            {
                return _selectedChat;
            }
            private set
            {
                if (_selectedChat == value)
                    return;

                _selectedChat = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedChat));
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


        public bool IsHomeScreenVisible
        {
            get
            {
                return CurrentView == null;
            }
        }

        #region Properties

       
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


       
        public bool IsSending
        {
            get
            {
                return _isSending;
            }
            private set
            {
                if (_isSending != value)
                {
                    _isSending = value;
                    OnPropertyChanged();

                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }


       
        public bool IsAiTyping
        {
            get
            {
                return _isTyping;
            }
            private set
            {
                if (_isTyping != value)
                {
                    _isTyping = value;
                    OnPropertyChanged();
                }
            }
        }


      
        public bool IsEmptyChat
        {
            get
            {
                return _isEmptyChat;
            }
            private set
            {
                if (_isEmptyChat != value)
                {
                    _isEmptyChat = value;
                    OnPropertyChanged();
                }
            }
        }


      
        #endregion


        #region Commands

        public ICommand SendMessageCommand
        {
            get;
        }


        public ICommand ClearChatCommand
        {
            get;
        }

        public ICommand OpenChatCommand
        {
            get;
        }


        public ICommand FirstNewChatCommand
        {
            get;
        }

        public ICommand MinimizeWindowCommand { get; }
        public ICommand CloseWindowCommand { get; }
        public ICommand SettingsCommand { get; }



        public ICommand NewChatCommand { get; }
        public int ChatId { get; private set; }
        public string Title { get; private set; }
        public DateTime CreatedDate { get; private set; }



        #endregion


        #region Constructor

        public MainViewModel()
        {

            _databaseService = new DatabaseService();

            _databaseService.InitializeDatabase();

            _chatRepository = new ChatRepository(_databaseService);



            _geminiService = new GeminiService();

            SendMessageCommand = new RelayCommand(
     async parameter => await SendMessageAsync(),
     parameter => CanSendMessage);

            
            ClearChatCommand = new RelayCommand(
                parameter => ClearCurrentChat()
            );

           

            ChatId = 0;
            Title = string.Empty;
            SelectedChatTitle = string.Empty;
            CreatedDate = DateTime.Now;

            MessageText = string.Empty;



            NewChatCommand = new RelayCommand(
               parameter => CreateNewChat());

            FirstNewChatCommand = new RelayCommand(
                parameter => FirstChat());


            MinimizeWindowCommand = new RelayCommand(MinimizeWindow);
            CloseWindowCommand = new RelayCommand(CloseWindow);

            SettingsCommand = new RelayCommand(OpenSettings);

            LoadChatHistory();

        }


        private void OpenSettings(object parameter)
        {
            var settingsViewModel = new SettingsViewModel();

            settingsViewModel.CloseRequested += CloseSettings;

            CurrentView = settingsViewModel;
        }


        private void CloseSettings()
        {
            // Return to the currently selected AI Chat
            if (SelectedChat != null)
            {
                CurrentView = SelectedChat;
            }
            else
            {
                // If no chat is selected, return to Home screen
                CurrentView = null;
            }
        }


        public bool CanSendMessage
        {
            get
            {
                return !IsSending &&
                       !string.IsNullOrWhiteSpace(MessageText);
            }
        }

        public void NavigateToHome()
        {
            CurrentView = null;

            MessageText = string.Empty;
            SelectedChatTitle = string.Empty;
            Title = string.Empty;

            CommandManager.InvalidateRequerySuggested();
        }


        private string GenerateChatTitle(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "New Chat";

            string title = message.Trim();

            title = title.Replace("\r", " ")
                         .Replace("\n", " ");

            while (title.Contains("  "))
            {
                title = title.Replace("  ", " ");
            }

            const int maxLength = 40;

            if (title.Length > maxLength)
            {
                title = title.Substring(0, maxLength).TrimEnd();
                title += "...";
            }

            return title;
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
                Chats.Add(new AiChatViewModel
                {
                    ChatId = chat.ChatId,
                    Title = chat.Title,
                    CreatedDate = chat.CreatedDate
                });
            }

            // NO CHAT HISTORY

            if (Chats.Count == 0)
            {
                SelectedChat = null;


                // Show MainWindow Home Screen
                CurrentView = null;

                IsEmptyChat = true;

                return;
            }


            // CHAT HISTORY EXISTS


            // Get the most recent chat
            AiChatViewModel latestChat = Chats[0];

            // Convert the AiChatViewModel back to Chat
            Chat existingChat =
                _chatRepository.GetChatById(latestChat.ChatId, CurrentUserSession.UserId);

            if (existingChat == null)
            {
                CurrentView = null;
                return;
            }

            // Automatically open the latest chat
            OpenExistingChat(existingChat);
        }



        #endregion


        #region Send Message

      

        #endregion



        private async Task SendMessageAsync()
        {
            if (IsSending)
                return;

            string message = MessageText?.Trim();

            if (string.IsNullOrWhiteSpace(message))
                return;

            try
            {
                IsSending = true;
                IsEmptyChat = false;

                
                // GET CURRENT CHAT
                

                AiChatViewModel chatViewModel = SelectedChat;

                

                // Create the chat and navigate to AiChatView
                

                if (chatViewModel == null)
                {
                    string chatTitle = GenerateChatTitle(message);

                    int chatId =
                        _chatRepository.CreateChat(CurrentUserSession.UserId, chatTitle);

                    Chat createdChat =
                        _chatRepository.GetChatById(chatId, CurrentUserSession.UserId);

                    chatViewModel = new AiChatViewModel
                    {
                        ChatId = createdChat.ChatId,
                        Title = createdChat.Title,
                        CreatedDate = createdChat.CreatedDate
                    };


                    // Select the SAME instance that will be displayed
                    SelectedChat = chatViewModel;

                    // Add chat to sidebar
                    Chats.Insert(0, chatViewModel);

                    // Navigate to AiChatView
                    CurrentView = chatViewModel;

                    SelectedChatTitle = chatViewModel.Title;

                    Messages.Clear();
                }

                
                // CREATE USER MESSAGE
                

                MessageViewModel userMessage =
                    new MessageViewModel
                    {
                        SenderType = "User",
                        MessageText = message,
                        Timestamp = DateTime.Now
                    };

                
                // SAVE USER MESSAGE
                

                _chatRepository.SaveMessage(
                    chatViewModel.ChatId,
                     CurrentUserSession.UserId,
                    "User",
                    message);

                
                // ADD USER MESSAGE TO CURRENT CHAT
                

                chatViewModel.Messages.Add(userMessage);

                Messages.Add(userMessage);

                
                // CLEAR INPUT
                

                MessageText = string.Empty;

                CommandManager.InvalidateRequerySuggested();

                
                // AI TYPING
                

                chatViewModel.IsAiTyping = true;

                
                // SEND TO GEMINI
                

                string aiResponse =
                    await _geminiService.SendMessageAsync(message);

                if (!string.IsNullOrWhiteSpace(aiResponse))
                {
                    
                    // CREATE AI MESSAGE
                    

                    MessageViewModel aiMessage =
                        new MessageViewModel
                        {
                            SenderType = "AI",
                            MessageText = aiResponse,
                            Timestamp = DateTime.Now
                        };

                    
                    // SAVE AI MESSAGE
                    

                    _chatRepository.SaveMessage(
                        chatViewModel.ChatId,
                         CurrentUserSession.UserId,
                        "AI",
                        aiResponse);

                    
                    // ADD AI MESSAGE
                    

                    chatViewModel.Messages.Add(aiMessage);

                    Messages.Add(aiMessage);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "SendMessageAsync Error: " + ex);
            }
            finally
            {
                if (SelectedChat != null)
                    SelectedChat.IsAiTyping = false;

                IsSending = false;

                CommandManager.InvalidateRequerySuggested();
            }
        }




        #region Message Methods


        #endregion


        private void CreateNewChat()
        {
            int chatId = _chatRepository.CreateChat(CurrentUserSession.UserId, Title);

            Chat chat = _chatRepository.GetChatById(chatId, CurrentUserSession.UserId);

            if (chat == null)
                return;

            var chatViewModel = new AiChatViewModel
            {
                ChatId = chat.ChatId,
                Title = chat.Title,
                CreatedDate = chat.CreatedDate
            };

            Chats.Insert(0, chatViewModel);

            SelectedChat = chatViewModel;


            // Clear messages for the new chat
            Messages.Clear();

            IsEmptyChat = true;
        }


        #region Clear Chat

        private void ClearCurrentChat()
        {
            Messages.Clear();

            IsEmptyChat = true;

            CommandManager.InvalidateRequerySuggested();
        }



        private void FirstChat()
        {
            SelectedChat = null;

            Messages.Clear();
            IsEmptyChat = true;
            MessageText = string.Empty;

            // Open the AI Chat view
            CurrentView = new AiChatViewModel();

            CommandManager.InvalidateRequerySuggested();
        }

        private void OpenExistingChat(Chat chat)
        {
            if (chat == null)
                return;

            // Create ViewModel for selected chat
            var chatViewModel = new AiChatViewModel
            {
                ChatId = chat.ChatId,
                Title = chat.Title,
                CreatedDate = chat.CreatedDate
            };

            // Load messages from database
            List<Message> messages =
                _chatRepository.GetMessages(chat.ChatId, CurrentUserSession.UserId);

            foreach (Message message in messages)
            {
                chatViewModel.Messages.Add(
                    new MessageViewModel
                    {
                        SenderType = message.SenderType,
                        MessageText = message.MessageText,
                        Timestamp = message.Timestamp
                    });
            }

            // Select this chat
            SelectedChat = chatViewModel;

            // Update title

            // Navigate to chat
            CurrentView = SelectedChat;

            // Update empty state
            IsEmptyChat = chatViewModel.Messages.Count == 0;

            // Clear input
            MessageText = string.Empty;

            CommandManager.InvalidateRequerySuggested();
        }




        #endregion
    }
}