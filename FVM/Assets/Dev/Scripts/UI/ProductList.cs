using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProductList : MonoBehaviour
{
    
    private TestProduct tempProduct;

    public UnityEvent<int> onBuyEvent;

    public int maxKg = 20;
    
    void Awake()
    {
        TestGameManager.Instance.AddPrepareGameEvent(TestGameManager.eventType.PREPARE, OnInit);
        
        tempProduct = GetComponentInChildren<TestProduct>();
        tempProduct.gameObject.SetActive(false);
    }

    void OnBuyProduct(int kg, int price)
    {
        TestGameManager.Instance.DietVM(kg);
        TestGameManager.Instance.BuyProductEvent(price);
        
        onBuyEvent.Invoke(TestGameManager.Instance.GetCoinCount());
    }

    void OnInit()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(tempProduct.gameObject, Vector3.zero, Quaternion.identity, this.transform);
            TestProduct product = obj.GetComponent<TestProduct>();
            
            product.SetData(2, 1);
            product.SetBuyAction(OnBuyProduct);

            onBuyEvent.AddListener(product.CheckPrice);
            
            obj.SetActive(true);
        }
    }
}
