using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuDungTakGames.Item
{
    public class Coin : Item
    {
        public override void OnGetItem()
        {
            TestGameManager.Instance.GetCoinEvent();

            base.OnGetItem();
        }
    }
}
