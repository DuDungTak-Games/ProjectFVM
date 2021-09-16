using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DuDungTakGames.Extensions;
using UnityEngine;

public class StageEditorHelper : MonoBehaviour
{
    
    bool isEditMode = false;
    bool isBlock = false;

    float tileUnit = 10;
    
    Vector3 tilePos = Vector3.zero;
    Vector3 tileSize = Vector3.zero;
    Vector3 gridPos = Vector3.zero;
    Vector3 mousePos = Vector3.zero;

    GameObject plane;

    public void SetInfo(bool editMode, float unit)
    {
        isEditMode = editMode;
        tileUnit = unit;
    }

    public void SetState(bool isBlock)
    {
        this.isBlock = isBlock;
    }

    public void SetSize(Vector3 size)
    {
        tileSize = size;
    }

    public void SetPos(Vector3 curTilePos, Vector3 curGridPos, Vector3 curMousePos)
    {
        tilePos = curTilePos;
        gridPos = curGridPos;
        mousePos = curMousePos;
    }

    void OnDrawGizmos()
    {
        UpdatePlane();
        UpdateGizmo();
    }

    void UpdatePlane()
    {
        if (isEditMode)
        {
            if (plane == null)
            {
                plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.MultipleScale(10000);
            
                plane.GetComponent<MeshRenderer>().enabled = false;
            }
        
            plane.transform.SetPosition(gridPos);
        }
        else if (plane != null)
        {
            DestroyImmediate(plane);
        }
    }

    void UpdateGizmo()
    {
        if (isEditMode)
        {
            float halfUnit = tileUnit / 2;
            Vector3 linePos = gridPos + (Vector3.right * tileUnit / 2) + (Vector3.forward * tileUnit / 2);
            
            Color greenColor = Color.green;
            greenColor.a = 0.5f;
            
            Color redColor = Color.red;
            redColor.a = 0.5f;

            Gizmos.color = isBlock ? redColor : greenColor;
            Gizmos.DrawCube(tilePos, tileSize);
        
            Gizmos.color = redColor;
            Gizmos.DrawLine(gridPos + (Vector3.forward * halfUnit), gridPos + (Vector3.back * halfUnit));
            Gizmos.DrawLine(gridPos + (Vector3.right * halfUnit), gridPos + (Vector3.left * halfUnit));
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(mousePos, 0.5f);
            
            Gizmos.color = Color.gray;
            for (int x = -10; x < 11; x++)
            {
                Vector3 startPos = linePos + (Vector3.back * 100);
                Vector3 endPos = linePos + (Vector3.forward * 100);
                Vector3 xPos = (Vector3.right * tileUnit) * x;
                
                Gizmos.DrawLine(startPos + xPos, endPos + xPos);
            }
            
            for (int z = -10; z < 11; z++)
            {
                Vector3 startPos = linePos + (Vector3.left * 100);
                Vector3 endPos = linePos + (Vector3.right * 100);
                Vector3 zPos = (Vector3.forward * tileUnit) * z;
                
                Gizmos.DrawLine(startPos + zPos, endPos + zPos);
            }
        }
    }
}
