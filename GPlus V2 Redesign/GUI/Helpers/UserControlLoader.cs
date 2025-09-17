using GPlus.GUI.Elements;

namespace GPlus.GUI.Helpers
{
    internal static class UserControlLoader
    {

        // Most likely a built in way to do this but I can't find it so here we are
        // This is to allow easy access to user controls from other user controls

        public static Dashboard? Dashboard = null;
        public static Clients? Clients = null;
        public static Servers? Servers = null;
        public static Settings? Settings = null;
        public static ShuttingDown? ShuttingDown = null;
        public static NavBar? NavBar = null; // Probably never needed but just incase

        public static void InitializeUserControls(Dashboard dashboard, Clients clients, Servers servers, Settings settings, NavBar navbar, ShuttingDown shuttingdown)
        {
            Dashboard = dashboard;
            Clients = clients;
            Servers = servers;
            Settings = settings;
            ShuttingDown = shuttingdown;
            NavBar = navbar;
        }

        public static void ClearUserControls()
        {
            Dashboard = null;
            Clients = null;
            Servers = null;
            Settings = null;
            Settings = null;
            NavBar = null;
        }
    }
}
