namespace NetherTools.Memory
{
    public static class Hooks
    {
        public static string movement = "35 43 00 00 00 00 01 00 00 00 02"; //Movement data. Coordinates this address 16 * 9 + 2 that contains: 3 x float(4bytes) for X, Y, Z coordinates.
        public static string playerState = "4D 65 6E 75 73 00 6E"; //Starts right here. Contains values like Menu, Crea, Surv ..etc. 
        public static string version = "76 31 2E 32 31 2E 31 32 32 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 00 00 38"; //game version. Starts right here

        public static Dictionary<string, string> menuScan = new Dictionary<string, string>
        {
            { "playerState", playerState },
            { "version", version },
        };

        public static Dictionary<string, string> playingScan = new Dictionary<string, string>
        {
            { "movement", movement },
        };
    }
}
