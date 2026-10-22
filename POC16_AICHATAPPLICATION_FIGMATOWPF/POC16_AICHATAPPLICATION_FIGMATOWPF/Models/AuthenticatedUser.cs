using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Models
{
    public class AuthenticatedUser
    {
        public int Id { get; set; }

        public string GoogleUserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string ProfilePicture { get; set; }

        public bool EmailVerified { get; set; }
    }
}
