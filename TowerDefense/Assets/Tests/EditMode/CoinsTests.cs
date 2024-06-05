using NUnit.Framework;
using UnityEngine;

public class NewTestScript
{
    [Test]
    public void SellTower()
    {
        GameObject playerGameObject = new GameObject();
        Player player = playerGameObject.AddComponent<Player>();
        player.SetCoinBalance(100);

        Assert.AreEqual(100, player.GetCoinBalance());
    }
}
