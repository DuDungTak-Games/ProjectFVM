using System;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationLabel : MonoBehaviour
{

    // langKey : ID => Get Language String Data (TextFormat)

    [SerializeField] Text labelText;

    public static Action refresh;

    void Awake()
    {
        if (labelText == null)
        {
            this.TryGetComponent<Text>(out labelText);
        }
    }
}
