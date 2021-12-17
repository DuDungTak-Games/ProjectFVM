using System.Collections;
using UnityEngine;

using DuDungTakGames.Extensions;

namespace DuDungTakGames.Item
{
    public class Item : MonoBehaviour
    {

        [SerializeField] ItemType itemType;

        Collider itemCollider;

        protected virtual void Awake()
        {
            itemCollider = GetComponent<Collider>();
        }

        protected virtual void Start()
        {
            StartCoroutine(IdleCoroutine());    
        }

        public void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.CompareTag("Player"))
            {
                OnGetItem(col, col.transform);
            }
        }

        public virtual void OnGetItem(Collider col, Transform colTrf)
        {
            itemCollider.enabled = false;

            GameManager.Instance.AddItem(itemType);

            this.StopAllCoroutines();
            StartCoroutine(OnGetItemCoroutine(colTrf));
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

        IEnumerator OnGetItemCoroutine(Transform colTrf)
        {
            Vector3 startPos = transform.position;

            yield return CoroutineExtensions.ProcessAction(6f, (t) =>
            {
                transform.position = Vector3.Lerp(startPos, colTrf.position, t);
                transform.LerpScale(Vector3.one, Vector3.zero, t);
            });

            Destroy(this.gameObject);
        }
    }
}