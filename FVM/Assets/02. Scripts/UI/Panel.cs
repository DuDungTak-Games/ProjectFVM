using UnityEngine;

public class Panel : MonoBehaviour
{
    public void SetPanel(bool isOn)
    {
        this.gameObject.SetActive(isOn);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
