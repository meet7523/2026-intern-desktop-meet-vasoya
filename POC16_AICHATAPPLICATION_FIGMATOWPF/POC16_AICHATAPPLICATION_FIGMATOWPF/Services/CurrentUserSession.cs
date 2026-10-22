using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Services
{
    public static class CurrentUserSession
    {
        public static AuthenticatedUser User { get; private set; }

        public static bool IsAuthenticated
        {
            get { return User != null; }
        }

        public static int UserId
        {
            get { return User != null ? User.Id : 0; }
        }

        public static string Name
        {
            get { return User != null ? User.Name : string.Empty; }
        }

        public static string Email
        {
            get { return User != null ? User.Email : string.Empty; }
        }

        public static string ProfilePicture
        {
            get { return User != null ? User.ProfilePicture : string.Empty; }
        }

        public static void SetUser(AuthenticatedUser user)
        {
            User = user;
        }

        public static void Clear()
        {
            User = null;
        }
    }
}