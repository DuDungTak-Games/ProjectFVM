using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuDungTakGames.Item
{
    public enum ItemType
    {
        COIN
    }

    public class Item : MonoBehaviour
    {
        [SerializeField]
        ItemType type;

        public ItemType GetItemType()
        {
            return this.type;
        }

        public void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.CompareTag("Player"))
            {
                OnGetItem();
            }
        }

        public virtual void OnGetItem()
        {
            this.gameObject.SetActive(false);
        }
    }
}