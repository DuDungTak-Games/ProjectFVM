using UnityEngine;
using UnityEngine.Events;

public class GimicTrigger : GimicObject
{

    [TagSelector] 
    public string targetTag;

    [HideInInspector]
    public UnityEvent gimicEvent;

    TileSkin[] tileSkins;
    
    void Start()
    {
        Init();
    }
    
    void Init()
    {
        tileSkins = GetComponentsInChildren<TileSkin>();

        foreach (var skin in tileSkins)
        {
            skin.Init(ID);
        }
    }

    public virtual void OnTrigger()
    {
        gimicEvent?.Invoke();
    }

    public void AddEvent(UnityAction action)
    {
        gimicEvent.AddListener(action);
    }
}
