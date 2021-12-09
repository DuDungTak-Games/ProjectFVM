using UnityEngine;
using UnityEditor;

using DuDungTakGames.Extensions;

public class StageEditorHelper : MonoBehaviour
{
    
    bool isEditMode = false;
    bool isBlock = false;

    EditorTile editorTile;
    GimicObject gimicObject;
    
    float tileUnit = 10;
    
    Vector3 tilePos = Vector3.zero;
    Vector3 multiTilePos = Vector3.zero;
    Vector3 tileRot = Vector3.zero;
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

    public void SetPos(Vector3 curTilePos, Vector3 multiTilePos, Vector3 curTileRot, Vector3 curGridPos, Vector3 curMousePos)
    {
        tilePos = curTilePos;
        this.multiTilePos = multiTilePos;
        tileRot = curTileRot;
        gridPos = curGridPos;
        mousePos = curMousePos;
    }

    public void SetSize(Vector3 size)
    {
        tileSize = size;
    }

    public void SetEditorTile(EditorTile editorTile)
    {
        this.editorTile = editorTile;
    }

    public void SetGimicObject(GimicObject gimicObject)
    {
        this.gimicObject = gimicObject;
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
            plane = GameObject.Find("Plane");
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

            if(multiTilePos != Vector3.zero)
            {
                for (int x = 0; x <= multiTilePos.x; x++)
                {
                    for (int y = 0; y <= multiTilePos.y; y++)
                    {
                        for (int z = 0; z <= multiTilePos.z; z++)
                        {
                            Vector3 multiPos = new Vector3(tilePos.x + (tileUnit * x),
                                                tilePos.y + (tileUnit * y),
                                                tilePos.z + (tileUnit * z));
                            DrawTile(multiPos);
                        }
                    }
                }
            }
            else
            {
                DrawTile(tilePos);
            }
            DrawTileOutline(halfUnit);
            DrawDirection(halfUnit);
            DrawMouseSphere();

            if (gimicObject != null)
            {
                DrawGimicInfo();
            }
            
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

    void DrawTile(Vector3 tilePos)
    {
        Color greenColor = Color.green;
        greenColor.a = 0.5f;
            
        Color redColor = Color.red;
        redColor.a = 0.5f;
        
        Color grayColor = Color.gray;
        grayColor.a = 0.5f;

        if (editorTile != null)
        {
            Gizmos.color = grayColor;
        }
        else
        {
            Gizmos.color = isBlock ? redColor : greenColor;
        }
        
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

    void DrawDirection(float halfUnit)
    {
        Gizmos.color = Color.cyan;
        
        transform.SetRotation(Quaternion.Euler(tileRot));

        Vector3 unit = gridPos + (Vector3.up * halfUnit);
        Vector3 fw = (transform.forward * tileUnit) + unit;
        Vector3 rh = (-transform.forward + transform.right * halfUnit) + unit;
        Vector3 lh = (-transform.forward + -transform.right * halfUnit) + unit;
        
        Gizmos.DrawLine(unit, fw);
        Gizmos.DrawLine(fw, rh);
        Gizmos.DrawLine(fw, lh);
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
        
        Gizmos.DrawCube(editorTile.tilePos, tileSize * 1.1f);
        
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.red;
        style.fontSize = 20;
        
        Vector3 namePos = editorTile.tilePos + (Vector3.up * 10f);
        Handles.Label(namePos, editorTile.gameObject.name, style);
    }

    void DrawGimicInfo()
    {
        Gizmos.color = Color.magenta;
        
        if (gimicObject is GimicTrigger || gimicObject is GimicCustom)
        {
            foreach (var actorObject in FindObjectsOfType<GimicActor>())
            {
                if (actorObject.ID == gimicObject.ID)
                {
                    Gizmos.DrawSphere(actorObject.transform.position, tileSize.y * 0.25f);
                    
                    DrawGimicLine(actorObject.transform.position);
                }
            }
        }
        else
        {
            Gizmos.DrawSphere(gimicObject.transform.position, tileSize.y * 0.25f);
            
            foreach (var actorObject in FindObjectsOfType<GimicTrigger>())
            {
                if (actorObject.ID == gimicObject.ID)
                {
                    DrawGimicLine(actorObject.transform.position);
                }
            }
        }
    }

    void DrawGimicLine(Vector3 targetPos)
    {
        Gizmos.DrawLine(editorTile.tilePos, targetPos);
    }
}
