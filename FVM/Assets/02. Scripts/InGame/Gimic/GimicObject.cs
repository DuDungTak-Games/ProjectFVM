using UnityEngine;

public class GimicObject : MonoBehaviour
{

    public int ID { get; protected set; }

    protected virtual void Awake()
    {
        Init();
    }
    
    void Init()
    {
        TileSkin[] tileSkins = GetComponentsInChildren<TileSkin>();
        foreach (var skin in tileSkins)
        {
            skin.Init(ID);
        }
    }
    
    public void SetGimicID(int id)
    {
        this.ID = id;
    }
    
    public virtual void OnActive()
    {
        
    }
}
