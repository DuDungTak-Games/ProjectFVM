using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSkinData", menuName = "Scriptable Object/TileSkinData")]
public class TileSkinData : ScriptableObject
{
    
    [SerializeField]
    public Material[] materials;

    public Material GetMaterial(int id)
    {
        if(materials.Length <= 0)
            return null;

        id = (int)Mathf.Repeat(id, materials.Length);
        return materials[id];
    }
}
