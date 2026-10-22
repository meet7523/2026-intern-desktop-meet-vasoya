using POC16_AICHATAPPLICATION_FIGMATOWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels
{
    public class MessageViewModel :ObservableObject
    {
       
            private string _messageText;
            private string _senderType;
            private DateTime _timestamp;


        private string _attachmentPath;
        private string _attachmentName;
        private string _attachmentSize;
        private string _attachmentExtension;


        public string AttachmentPath
        {
            get => _attachmentPath;
            set
            {
                if (_attachmentPath == value)
                    return;

                _attachmentPath = value;
                OnPropertyChanged();
            }
        }

        public string AttachmentName
        {
            get => _attachmentName;
            set
            {
                if (_attachmentName == value)
                    return;

                _attachmentName = value;
                OnPropertyChanged();
            }
        }


        public string AttachmentSize
        {
            get => _attachmentSize;
            set
            {
                if (_attachmentSize == value)
                    return;

                _attachmentSize = value;
                OnPropertyChanged();
            }
        }

        public string AttachmentExtension
        {
            get => _attachmentExtension;
            set
            {
                if (_attachmentExtension == value)
                    return;

                _attachmentExtension = value;
                OnPropertyChanged();
            }
        }


        public bool HasAttachment =>
    !string.IsNullOrWhiteSpace(AttachmentPath);

        public bool IsImageAttachment
        {
            get
            {
                string extension =
                    AttachmentExtension?.ToLowerInvariant();

                return extension == ".png"
                    || extension == ".jpg"
                    || extension == ".jpeg"
                    || extension == ".gif"
                    || extension == ".bmp"
                    || extension == ".webp";
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
                    _messageText = value;
                    OnPropertyChanged();
                }
            }

            public string SenderType
            {
                get
                {
                    return _senderType;
                }
                set
                {
                    _senderType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsUser));
                    OnPropertyChanged(nameof(IsAI));
                }
            }

            public DateTime Timestamp
            {
                get
                {
                    return _timestamp;
                }
                set
                {
                    _timestamp = value;
                    OnPropertyChanged();
                }
            }

            public bool IsUser
            {
                get
                {
                    return SenderType == "User";
                }
            }

            public bool IsAI
            {
                get
                {
                    return SenderType == "AI";
                }
            }

        public int MessageId { get; internal set; }
        public int ChatId { get; internal set; }
    }
    }
