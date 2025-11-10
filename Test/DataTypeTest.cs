using NetherTools;

namespace Test
{
    [TestClass]
    public sealed class DataTypeTest
    {
        [TestMethod]
        public void FloatTest()
        {
            Console.WriteLine(FromBytes.ToFloat(new byte[] { 0x60, 0x50, 0x58, 0x41 }));
        }
    }
}
