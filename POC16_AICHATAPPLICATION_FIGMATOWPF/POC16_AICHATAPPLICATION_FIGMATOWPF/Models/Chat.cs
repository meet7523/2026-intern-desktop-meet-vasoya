using POC16_AICHATAPPLICATION_FIGMATOWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Models
{
    public class Chat : ObservableObject
    {
        public int ChatId { get; set; }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isRenaming;

        public bool IsRenaming
        {
            get { return _isRenaming; }
            set
            {
                if (_isRenaming != value)
                {
                    _isRenaming = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime CreatedDate { get; set; }

        public string TimeAgo
        {
            get
            {
                TimeSpan difference = DateTime.Now - CreatedDate;

                if (difference.TotalMinutes < 1)
                    return "Just now";

                if (difference.TotalHours < 1)
                    return $"{(int)difference.TotalMinutes}m ago";

                if (difference.TotalDays < 1)
                    return $"{(int)difference.TotalHours}h ago";

                if (difference.TotalDays < 2)
                    return "Yesterday";

                return $"{(int)difference.TotalDays}d ago";
            }
        }
    }
}
