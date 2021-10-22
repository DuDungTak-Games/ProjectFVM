using UnityEngine;

public class TileSkin : MonoBehaviour
{

    public TileSkinData skinData;

    public void Init(int ID)
    {
        if(skinData == null)
            return;
        
        MeshRenderer mr;
        if (TryGetComponent(out mr))
        {
            for (int i = 0; i < mr.materials.Length; i++)
            {
                mr.materials[i].mainTexture = skinData.GetTexture(i, ID);
            }
        }
    }
}
