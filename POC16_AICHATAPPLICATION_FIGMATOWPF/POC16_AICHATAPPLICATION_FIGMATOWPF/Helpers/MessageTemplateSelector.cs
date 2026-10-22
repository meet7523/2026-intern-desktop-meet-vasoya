using POC16_AICHATAPPLICATION_FIGMATOWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Helpers
{
  
        public class MessageTemplateSelector : System.Windows.Controls.DataTemplateSelector
        {
            public DataTemplate UserTemplate { get; set; }

            public DataTemplate AITemplate { get; set; }

            public override DataTemplate SelectTemplate(
                object item,
                DependencyObject container)
            {
                if (item is MessageViewModel message)
                {
                    if (message.IsUser)
                    {
                        return UserTemplate;
                    }

                    if (message.IsAI)
                    {
                        return AITemplate;
                    }
                }

                return base.SelectTemplate(item, container);
            }
        }
    }

