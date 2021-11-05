using UnityEngine;

public class TileSkin : MonoBehaviour
{

    public TileSkinData[] skinDatas;

    public void Init(int ID)
    {
        if(skinDatas.Length <= 0)
            return;
        
        MeshRenderer mr;
        if (TryGetComponent(out mr))
        {
            if (skinDatas.Length == 1)
            {
                mr.material = skinDatas[0].GetMaterial(ID);
            }
            else
            {
                for (int i = 0; i < mr.materials.Length; i++)
                {
                    mr.materials[i] = skinDatas[i].GetMaterial(ID);
                }
            }
        }
    }
}
