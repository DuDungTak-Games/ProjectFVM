using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelText : MonoBehaviour
{

    public enum labelType
    {
        COIN,
        CUR_FUEL,
        CUR_KG,
        CUR_HEIGHT,
        MAX_HEIGHT,
        CUR_THRUST,
        CUR_VELOCITY,
        CUR_ANGLE
    }

    public labelType type;

    public Text labelText;

    public string formatTxt;

    void Awake()
    {
        if (labelText == null)
        {
            this.TryGetComponent<Text>(out labelText);
        }
    }

    void Start()
    {
        TestGameManager.Instance.AddUpdateTextEvent(type, UpdateLabel);

        UpdateLabel(0);
    }

    void UpdateLabel(int value)
    {
        if (labelText == null || formatTxt.Length <= 0)
            return;
        
        // TODO : 여러 매개변수를 받는 경우?
        labelText.text = string.Format(formatTxt, value);
    }
}
