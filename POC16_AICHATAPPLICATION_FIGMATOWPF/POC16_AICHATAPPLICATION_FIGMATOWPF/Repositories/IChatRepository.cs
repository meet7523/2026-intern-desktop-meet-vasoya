using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Repositories
{
    public interface IChatRepository
    {    
            int CreateChat(string title);

            List<Chat> GetChats();

            Chat GetChat(int chatId);

            void RenameChat(int chatId, string title);

            void DeleteChat(int chatId);
    }
}

