using NUnit.Framework;

public class CoinTests
{
    [Test]
    public void CoinTestsSimplePasses()
    {
        var value = 1;
        Assert.AreEqual(1, value);
    }


}
