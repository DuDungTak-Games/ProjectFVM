using System.Collections;
using UnityEngine;

using DuDungTakGames.Extensions;

namespace DuDungTakGames.Item
{
    public enum ItemType
    {
        COIN
    }

    public class Item : MonoBehaviour
    {

        [SerializeField] ItemType type;

        Transform targetTrf;

        Collider collider;

        protected virtual void Awake()
        {
            collider = GetComponent<Collider>();
        }

        protected virtual void Start()
        {
            StartCoroutine(IdleCoroutine());    
        }

        //public ItemType GetItemType()
        //{
        //    return this.type;
        //}

        public void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.CompareTag("Player"))
            {
                targetTrf = col.gameObject.transform;

                OnGetItem();
            }
        }

        public virtual void OnGetItem()
        {
            this.StopAllCoroutines();
            StartCoroutine(OnGetItemCoroutine());
        }

        IEnumerator IdleCoroutine()
        {
            Vector3 originPos = transform.position;

            while(true)
            {
                transform.position = new Vector3(originPos.x, (originPos.y) + Mathf.Sin(Time.time * 1f), originPos.z);
                transform.Rotate((Vector3.up * 45f) * Time.smoothDeltaTime);

                yield return null;
            }
        }

        IEnumerator OnGetItemCoroutine()
        {
            collider.enabled = false;

            Vector3 startPos = transform.position;

            yield return CoroutineExtensions.ProcessAction(6f, (t) =>
            {
                transform.position = Vector3.Lerp(startPos, targetTrf.position, t);
                transform.LerpScale(Vector3.one, Vector3.zero, t);
            });

            Destroy(this.gameObject);
        }
    }
}