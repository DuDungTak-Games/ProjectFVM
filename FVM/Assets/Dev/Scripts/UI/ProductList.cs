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
        TestGameManager.Instance.AddGameEvent(gameState.Prepare, OnInit);
        
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
