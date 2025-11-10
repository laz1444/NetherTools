namespace NetherTools
{
    public static class Log
    {
        public static bool debugMode = false;

        public static void debug(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            if (debugMode)
            {
                Console.ForegroundColor = color;
                ToLog($"[DEBUG] {message}");
                Console.ResetColor();
            }
        }

        public static void info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ToLog($"[INFO] {message}");
            Console.ResetColor();
        }

        public static void warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            ToLog($"[WARN] {message}");
            Console.ResetColor();
        }

        public static void error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ToLog($"[ERROR] {message}");
            Console.ResetColor();
        }

        public static void ToLog(string message)
        {
            Console.WriteLine(message);
        }
    }
}
