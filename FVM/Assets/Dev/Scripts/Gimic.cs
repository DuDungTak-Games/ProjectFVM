using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuDungTakGames.Gimic
{
    public class Gimic : MonoBehaviour
    {
        GameObject testGimicObj;

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



            if (testGimicObj != null)
            {
                testGimicObj.SetActive(false);
            }
        }

        public void SetTriggerObject(GameObject targetObj)
        {
            testGimicObj = targetObj;
        }
    }
}