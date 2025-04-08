using Client.Models;

namespace Client
{
    public static class GlobalState
    {
        public static User CurrentUser { get; set; }
        public static string Token { get; set; }

        public static void Clear()
        {
            CurrentUser = null;
            Token = null;
        }
    }
}
