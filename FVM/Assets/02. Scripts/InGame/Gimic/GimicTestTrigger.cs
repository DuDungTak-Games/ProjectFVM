using System;
using UnityEngine;

public class GimicTestTrigger : GimicTrigger
{

    // TODO : Ʈ���� �Ǵ� ����� �����պ� ����, �ؽ�ó �� �� �������� ��ũ���ͺ� ������Ʈ��
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
