using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemLabel : MonoBehaviour
{

    [SerializeField] ItemType itemType;

    [SerializeField] Text labelText;

    // TODO : 추후에 LocalizationManager 를 통해서 설정
    [SerializeField] string labelFormat;

    public static Action refresh;

    void Awake()
    {
        if (labelText == null)
        {
            this.TryGetComponent<Text>(out labelText);
        }

        refresh += UpdateLabel;

        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (labelText != null && labelFormat.Length > 0)
        {
            int value = GameManager.Instance.GetItemCount(itemType);
            labelText.text = string.Format(labelFormat, value);
        }
    }
}
