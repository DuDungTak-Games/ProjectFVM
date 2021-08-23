using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuDungTakGames.Gimic
{
    public class Gimic : MonoBehaviour
    {
        
        bool isOnce;

        Action gimicAction;

        public void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                OnTrigger();
            }
        }

        public virtual void OnTrigger()
        {
            Color onColor = Color.red;
            this.GetComponent<MeshRenderer>().material.color = onColor;

            gimicAction?.Invoke();

            if (isOnce)
            {
                gimicAction = null;
            }
        }

        public void SetTriggerAction(Action gimicLogic, bool isOnce = false)
        {
            this.isOnce = isOnce;
            gimicAction = gimicLogic;
        }
    }
}