using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProductList : MonoBehaviour
{
    
    private TestProduct tempProduct;

    public UnityEvent<int> onBuyEvent;

    public int maxProduct = 10;
    
    void Awake()
    {
        GameManager.Instance.AddGameEvent(GameState.LAUNCH_PREPARE, OnInit);
        
        tempProduct = GetComponentInChildren<TestProduct>();
        tempProduct.gameObject.SetActive(false);
    }

    void OnBuyProduct(int kg, int price)
    {
        // TODO : 여기 수정해야함
        // GameManager.Instance.DietVM(kg);
        // GameManager.Instance.BuyProductEvent(price);

        // onBuyEvent.Invoke(GameManager.Instance.GetCoinCount());
    }

    void OnInit()
    {
        if (transform.childCount > 1)
            return;
        
        for (int i = 0; i < maxProduct; i++)
        {
            GameObject obj = Instantiate(tempProduct.gameObject, Vector3.zero, Quaternion.identity, this.transform);
            TestProduct product = obj.GetComponent<TestProduct>();
            
            product.SetData(3, 1);
            product.SetBuyAction(OnBuyProduct);

            onBuyEvent.AddListener(product.CheckPrice);
            
            obj.SetActive(true);
        }
    }
}
