using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EditorTile : MonoBehaviour
{

#if UNITY_EDITOR
    public static bool showGimicID;
#endif

    [HideInInspector] 
    public TileID tileID;
    
    [HideInInspector]
    public float floor;
    
    [HideInInspector]
    public Vector3 tilePos, spawnRot;

    [HideInInspector]
    public GimicObject gimicObject;

#if UNITY_EDITOR
    void Awake()
    {
        if (gimicObject == null)
        {
            this.TryGetComponent(out gimicObject);
        }
    }


    void OnDrawGizmos()
    {
        if(showGimicID && gimicObject != null)
        {
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.red;
            style.fontSize = 18;

            Vector3 namePos = transform.position + (Vector3.up * 2.5f);
            Handles.Label(namePos, gimicObject.ID.ToString(), style);
        }
    }
#endif
}
