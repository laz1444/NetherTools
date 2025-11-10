namespace NetherTools
{
    public class FromBytes
    {
        public static float ToFloat(byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToSingle(bytes, startIndex);
        }
    }
}
