namespace NetherTools.Memory
{
    public static class Hooks
    {
        public static string coordinates = "9A 99 19 3F 66 66 E6 3F 0F"; //Actual coordinates this address -16 + 5 that contains: 3 x float(4bytes) for X, Y, Z coordinates.
        public static string playerState = "4D 65 6E 75 73 00 6E"; //Starts right here. Contains values like Menu, Crea, Surv ..etc. 
        public static string version = "76 31 2E 32 31 2E 31 32 31 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 00 00 38"; //game version. Starts right here

        public static Dictionary<string, string> menuScan = new Dictionary<string, string>
        {
            { "playerState", playerState },
            { "version", version },
        };

        public static Dictionary<string, string> playingScan = new Dictionary<string, string>
        {
            { "coordinates", coordinates },
        };
    }
}
