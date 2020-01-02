using NUnit.Framework;
using SlxLuhnLibrary;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestLuhn()
        {
            var accountNumber = "000001";
            var accNumberWithLuhn = ClsLuhnLibrary.WithLuhn_Base10(accountNumber);
            Assert.That(accNumberWithLuhn.EndsWith("8"));

            var isValid = ClsLuhnLibrary.CheckLuhn_Base10(accNumberWithLuhn);
            Assert.That(isValid);
        }
    }
}