using System;
using UnityEngine;
using UnityEngine.UI;

public class TestProduct : MonoBehaviour
{
    
    private Action<int,int> onBuyAction;

    public Text labelText;
    
    public Button buyButton;
    
    [TextArea] public string formatTxt = "{0}kg Coin : {1}";

    private int kg, price;

    public void SetData(int kg, int price)
    {
        this.kg = kg;
        this.price = price;

        buyButton.interactable = true;

        UpdateUI();
    }
    
    public void SetBuyAction(Action<int,int> action)
    {
        buyButton.onClick.AddListener(() =>
        {
            action.Invoke(kg, price);
            
            this.kg = 0;
            this.price = 0;
            
            buyButton.interactable = false;
        });
    }

    public void CheckPrice(int coinCount)
    {
        if (price == 0)
            return;

        buyButton.interactable = coinCount >= price;
    }

    void UpdateUI()
    {
        if (formatTxt.Length > 0)
        {
            labelText.text = string.Format(formatTxt, kg, price);
        }
    }
}
