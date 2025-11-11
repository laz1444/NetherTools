using NetherTools.Functions.Internal;
using NetherTools.Memory;

namespace NetherTools.Functions
{
    public static class MemoryScanner
    {
        public enum ScanType
        {
            Menu,
            Game
        }

        public static void Scan(ScanType type)
        {
            if (type == ScanType.Menu)
            {
                var addressData = MemoryReader.ScanMemory(Program.hProc, Hooks.menuScan);

                if (addressData.ContainsKey("playerState"))
                {
                    DynamicMemory.playerState = addressData["playerState"];
                    PlayerStateF.Run();
                }
                else
                {
                    Log.error("get playerState Failed");
                    return;
                }

                if (addressData.ContainsKey("version"))
                {
                    DynamicMemory.version = addressData["version"];
                }
            }
            else if (type == ScanType.Game)
            {
                var addressData = MemoryReader.ScanMemory(Program.hProc, Hooks.playingScan);

                if (addressData.ContainsKey("movement"))
                {
                    DynamicMemory.movement = addressData["movement"];
                    PositionF.Run();
                }
                else
                {
                    Log.error("get movement Failed");
                }
            }
        }
    }
}
