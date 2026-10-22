using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Repositories
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);

        List<Message> GetMessages(int chatId);

        void DeleteMessages(int chatId);
    }
}
