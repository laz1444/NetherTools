namespace NetherTools.Memory
{
    public static class Hooks
    {
        public static string movement = "35 43 00 00 00 00 01 00 00 00 02"; //Movement data. Coordinates this address 16 * 9 + 2 that contains: 3 x float(4bytes) for X, Y, Z coordinates.
        public static string playerState = "4D 65 6E 75 73 00 6E"; //Starts right here. Contains values like Menu, Crea, Surv ..etc. 
        public static string setTime = "01 00 00 00 00 00 00 00 FF 7F 00 00 00 00 00 00 ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 04";
       // public static string queryTime = "01 00 00 00 00 00 00 00 FF 7F 00 00 00 00 00 00 ?? ?? ?? 00 00 00 00 00 ?? ?? 17 69 00 00 00 00 04";
        public static string version = "76 31 2E 32 31 2E 31 32 34 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 00 00 38"; //game version. Starts right here

        public static byte[] GetPlayerCoordinates()
        {
            return MemoryReader.ReadBytes(DynamicMemory.movement, 4 * 3, 2 + (16 * 9));
        }

        public static byte[] GetPlayerState()
        {
            return MemoryReader.ReadBytes(DynamicMemory.playerState, 4);
        }

        public static byte[] GetFPS()
        {
            return MemoryReader.ReadBytes(DynamicMemory.fps, 4);
        }

        public static Dictionary<string, string> menuScan = new Dictionary<string, string>
        {
            { "playerState", playerState },
            { "version", version },
        };

        public static Dictionary<string, string> playingScan = new Dictionary<string, string>
        {
            { "movement", movement },
            { "setTime", setTime },
           // { "queryTime", queryTime },
        };
    }
}
