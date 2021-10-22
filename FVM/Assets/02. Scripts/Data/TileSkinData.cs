using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSkinData", menuName = "Scriptable Object/TileSkinData")]
public class TileSkinData : ScriptableObject
{
    
    [SerializeField]
    public List<SubList<Texture>> textureList;

    public Texture GetTexture(int idx, int id)
    {
        idx = Mathf.Clamp(id, 0, textureList.Count-1);
        
        if(textureList.Count <= 0)
            return null;
        
        id = Mathf.Clamp(id, 0, textureList[idx].Count-1);
        return textureList[idx][id];
    }
}
