using System;
using UnityEngine;

public class GimicTestTrigger : GimicTrigger
{

    // TODO : 트리거 또는 기믹의 프리팹별 색상, 텍스처 등 의 변수들은 스크립터블 오브젝트로
    [SerializeField] 
    Color[] skinColors;
    
    void OnEnable()
    {
        if(skinColors.Length <= 0)
            return;
        
        int selectIdx = Mathf.Clamp(ID, 0, skinColors.Length-1);
        
        MeshRenderer mr;
        if (TryGetComponent(out mr))
        {
            foreach (var material in mr.materials)
            {
                material.color = skinColors[selectIdx];
            }
        }
    }
}
