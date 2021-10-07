using UnityEngine;

using DuDungTakGames.Extensions;
using UnityEditor;

public class StageEditorHelper : MonoBehaviour
{
    
    bool isEditMode = false;
    bool isBlock = false;

    EditorTile editorTile;
    
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

    public void SetEditorTile(EditorTile editorTile)
    {
        this.editorTile = editorTile;
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
            Vector3 linePos = gridPos + (Vector3.right * halfUnit) + (Vector3.forward * halfUnit);
            DrawGrid(linePos);

            DrawTile();
            DrawTileOutline(halfUnit);
            DrawMouseSphere();

            if (editorTile != null)
            {
                DrawTileInfo();
            }
        }
    }

    void DrawMouseSphere()
    {
        Color mouseColor = Color.blue;
        mouseColor.a = 0.5f;
        
        Gizmos.color = mouseColor;

        Gizmos.DrawSphere(mousePos, 0.5f);
    }

    void DrawTile()
    {
        Color greenColor = Color.green;
        greenColor.a = 0.5f;
            
        Color redColor = Color.red;
        redColor.a = 0.5f;

        Gizmos.color = isBlock ? redColor : greenColor;
        Gizmos.DrawCube(tilePos, tileSize);
    }

    void DrawTileOutline(float halfUnit)
    {
        Gizmos.color = Color.red;

        Vector3 fwrh = gridPos + (Vector3.forward + Vector3.right) * halfUnit;
        Vector3 fwlh = gridPos + (Vector3.forward + Vector3.left) * halfUnit;
        Vector3 bcrh = gridPos + (Vector3.back + Vector3.right) * halfUnit;
        Vector3 bclh = gridPos + (Vector3.back + Vector3.left) * halfUnit;
        Vector3 upUnit = (Vector3.up * tileUnit);
        
        Gizmos.DrawLine(fwrh, fwrh + upUnit);
        Gizmos.DrawLine(fwlh, fwlh + upUnit);
        Gizmos.DrawLine(bcrh, bcrh + upUnit);
        Gizmos.DrawLine(bclh, bclh + upUnit);
        
        Gizmos.DrawLine(fwrh, fwlh);
        Gizmos.DrawLine(bcrh, bclh);
        Gizmos.DrawLine(fwrh, bcrh);
        Gizmos.DrawLine(fwlh, bclh);
        
        Gizmos.DrawLine(fwrh + upUnit, fwlh + upUnit);
        Gizmos.DrawLine(bcrh + upUnit, bclh + upUnit);
        Gizmos.DrawLine(fwrh + upUnit, bcrh + upUnit);
        Gizmos.DrawLine(fwlh + upUnit, bclh + upUnit);
    }

    void DrawGrid(Vector3 linePos)
    {
        Color gridColor = Color.gray;
        gridColor.a = 0.5f;
        
        Gizmos.color = gridColor;

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

    void DrawTileInfo()
    {
        Color tileColor = Color.blue;
        tileColor.a = 0.5f;

        Gizmos.color = tileColor;
        
        Gizmos.DrawCube(editorTile.spawnPos, tileSize * 1.1f);
        
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.red;
        style.fontSize = 20;
        
        Vector3 namePos = editorTile.spawnPos + (Vector3.up * 10f);
        Handles.Label(namePos, editorTile.gameObject.name, style);
    }
}
