using UnityEngine;

public class GimicObject : MonoBehaviour
{

    public int ID { get; protected set; }

    public void SetGimicID(int id)
    {
        this.ID = id;
    }
}
