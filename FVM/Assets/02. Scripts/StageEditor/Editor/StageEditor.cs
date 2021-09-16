using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using DuDungTakGames.Extensions;

public class StageEditor : EditorWindow
{

    StageEditorHelper GizmoHelper;

    bool isEditMode = false;
    
    public float tileFloor = 0;
    float tileUnit = 10;

    Vector3 tileSize = new Vector3(10, 10, 10);

    private GameObject testTileObject;
    private static List<Tuple<Vector3, GameObject>> tileList = new List<Tuple<Vector3, GameObject>>();
    
    [MenuItem("DudungtakGames/Stage Editor")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(StageEditor));
    }

    void OnGUI()
    {
        if (GUIStyles.Count <= 0)
        {
            CreateGUIStyle();
        }
        
        if (GizmoHelper == null)
            return;

        DisplayGUI();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (GUIStyles.Count <= 0)
        {
            CreateGUIStyle();
        }
        
        if (GizmoHelper == null)
            return;

        DisplaySceneGUI();
    }

    void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
        EditorApplication.playModeStateChanged += (mode) =>
        {
            if (mode == PlayModeStateChange.EnteredPlayMode)
            {
                isEditMode = false;
            }
        };
        
        Init();
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    void Init()
    {
        if (GizmoHelper == null)
        {
            GizmoHelper = FindObjectOfType<StageEditorHelper>();
            
            if (GizmoHelper == null)
            {
                GameObject obj = new GameObject("/// StageEditorHelper ///");
                GizmoHelper = obj.AddComponent<StageEditorHelper>();
            }
        }
    }

    void DisplayGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Stage Editor", GetStyle("Title"), GUILayout.Height(40));

        testTileObject = (GameObject)EditorGUILayout.ObjectField(testTileObject, typeof(GameObject), true);
        
        EditorGUILayout.LabelField(string.Format("타일 배치 모드 : {0}", isEditMode ? "ON" : "OFF"),
            GetStyle("Context", isEditMode ? Color.green : Color.red, true), GUILayout.MinHeight(30));

        EditorGUILayout.Space();
        isEditMode = GUILayout.Toggle(isEditMode, "타일 배치 모드", GetStyle("Button", 24, Color.white), GUILayout.MinHeight(40));

        UpdateHelper();
    }

    void DisplaySceneGUI()
    {
        Handles.BeginGUI();

        UpdateRay();
        UpdateInput();

        ShowToolBox();
        
        Handles.EndGUI();
    }

    void UpdateHelper()
    {
        GizmoHelper.SetInfo(isEditMode, tileUnit);
    }

    Vector3 tilePos = Vector3.zero;
    Vector3 prevTilePos = Vector3.zero;
    Vector3 gridPos = Vector3.zero;
    Vector3 mousePos = Vector3.zero;
    void UpdateRay()
    {
        if (isEditMode)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity))
            {
                mousePos = hit.point;

                float posX = (Mathf.RoundToInt(mousePos.x / tileUnit) * tileUnit);
                float posY = (Mathf.FloorToInt(tileFloor) * tileUnit) + (tileFloor % 1 != 0 ? 7.5f : 0);
                float posZ = (Mathf.RoundToInt(mousePos.z / tileUnit) * tileUnit);

                tilePos = new Vector3(posX, posY, posZ);
                gridPos = new Vector3(posX, (tileFloor * (tileUnit) - tileUnit/2), posZ);
                
                if (prevTilePos != tilePos)
                {
                    prevTilePos = tilePos;
                    TileCheck();
                }

                GizmoHelper.SetPos(tilePos, gridPos, mousePos);
            }
        }
    }

    void UpdateInput()
    {
        if (isEditMode)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;
            if (e != null)
            {
                if (e.isMouse || e.isKey)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        if (e.button == 0 && e.keyCode != KeyCode.C) // NOTE : 타일 배치
                        {
                            CreateTile();
                        }
                    }

                    if (e.type == EventType.KeyDown)
                    {
                        if (e.keyCode == KeyCode.T) // NOTE : 타일 배치
                        {
                            CreateTile();
                        }
                        
                        if (e.keyCode == KeyCode.C) // NOTE : 타일 삭제
                        {
                            DeleteTile();
                        }
                        
                        if (e.keyCode == KeyCode.R) // NOTE : 층 올리기
                        {
                            SetTileFloor(true);
                        }
                
                        if (e.keyCode == KeyCode.F) // NOTE : 층 내리기
                        {
                            SetTileFloor(false);
                        }
                    }
                }
            }
        }
    }

    int toolBoxwindowID = 1000;
    Rect toolBoxRect = new Rect(20,40, 260, 0);
    void ShowToolBox()
    {
        toolBoxRect = GUILayout.Window (toolBoxwindowID, toolBoxRect, (id) => 
        {

            EditorGUILayout.LabelField (string.Format("타일 위치 ({0}, {1}, {2})", tilePos.x, tilePos.y, tilePos.z), 
                GetStyle("Context", true), GUILayout.MinHeight(30));
            EditorGUILayout.LabelField (string.Format("그리드 위치 ({0}, {1}, {2})", gridPos.x, gridPos.y, gridPos.z), 
                GetStyle("Context", true), GUILayout.MinHeight(30));
            EditorGUILayout.LabelField (string.Format("마우스 위치 ({0}, {1}, {2})", Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), Mathf.Round(mousePos.z)), 
                GetStyle("Context", true), GUILayout.MinHeight(30));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField ("현재 층", GetStyle("Context", 16, TextAnchor.LowerCenter, true), 
                GUILayout.MaxWidth(60), GUILayout.MinHeight(30));

            EditorGUILayout.FloatField(tileFloor, GetStyle("NumberField"), GUILayout.MinHeight(30));

            EditorGUILayout.BeginVertical();
            if(GUILayout.Button("▲", GUILayout.MaxHeight(15))) { SetTileFloor(true); }
            if(GUILayout.Button("▼", GUILayout.MaxHeight(15))) { SetTileFloor(false); }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("그리드 위치 초기화", GUILayout.MinHeight(30))) { gridPos = Vector3.zero; }
            if (GUILayout.Button("층 초기화", GUILayout.MinHeight(30))) { tileFloor = 0; }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField (string.Format("타일 수 : {0}", tileList.Count), 
                GetStyle("Context", true), GUILayout.MinHeight(30));
            if (GUILayout.Button("타일 초기화", GUILayout.MinHeight(30))) { ClearTileList(); }

        }, "Stage Editor ToolBox", GUILayout.MaxWidth(260));
    }

    void SetTileFloor(bool isUp)
    {
        tileFloor += isUp ? 0.5f : -0.5f;
        
        float remian = Mathf.Abs(tileFloor) % 1;
        if (remian != 0 && remian != 0.5f)
        {
            tileFloor = Mathf.RoundToInt(tileFloor);
        }

        tileSize = new Vector3(tileUnit, remian == 0.5f ? (tileUnit/2) : tileUnit, tileUnit);
        GizmoHelper.SetSize(tileSize);
    }

    void CreateTile()
    {
        if (!TileCheck())
        {
            GameObject tile = Instantiate(testTileObject, tilePos, Quaternion.identity);

            if (tileFloor % 1 != 0)
            {
                tile.transform.MultipleScaleY(0.5f);
            }
                                
            tileList.Add(new Tuple<Vector3, GameObject>(tilePos, tile));
        }
    }

    void DeleteTile()
    {
        Tuple<Vector3, GameObject> tile = FindDuplicateTile();
        if (tile != null)
        {
            tileList.Remove(tile);
            DestroyImmediate(tile.Item2);
        }
    }

    bool TileCheck()
    {
        bool isAlready = FindDuplicateTile() != null;
        
        GizmoHelper.SetState(isAlready);
        
        return isAlready;
    }

    Tuple<Vector3, GameObject> FindDuplicateTile()
    {
        return tileList.Find(x =>
            x.Item1 == tilePos || x.Item1 == tilePos - (Vector3.up * 2.5f) || x.Item1 == tilePos + (Vector3.up * 7.5f));
    }

    void ClearTileList()
    {
        foreach (var tile in tileList)
        {
            DestroyImmediate(tile.Item2);
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("Tile"))
        {
            DestroyImmediate(obj);
        }
        
        tileList.Clear();
    }

    #region GUI_Style
    Dictionary<string, GUIStyle> GUIStyles = new Dictionary<string, GUIStyle>();

    void CreateGUIStyle()
    {
        GUIStyle labelStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        labelStyle.fixedHeight = 32;
        labelStyle.fontSize = 18;
        GUIStyles.Add("Title", labelStyle);
        
        GUIStyle textStyle = new GUIStyle(EditorStyles.label);
        textStyle.margin = new RectOffset(5, 5, 15, 15);
        textStyle.fixedHeight = 24;
        textStyle.fontSize = 16;
        GUIStyles.Add("Context", textStyle);
        
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        GUIStyles.Add("Button", buttonStyle);
        
        GUIStyle numberStyle = new GUIStyle(EditorStyles.numberField);
        numberStyle.alignment = TextAnchor.MiddleCenter;
        numberStyle.fontStyle = FontStyle.Bold;
        numberStyle.fontSize = 16;
        GUIStyles.Add("NumberField", numberStyle);
    }

    GUIStyle GetStyle(string styleName)
    {
        return GUIStyles[styleName];
    }
    
    GUIStyle GetStyle(string styleName, bool isBold = false)
    {
        GUIStyle style = new GUIStyle(GUIStyles[styleName]);
        style.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
        return style;
    }

    GUIStyle GetStyle(string styleName, int fontSzie, bool isBold = false)
    {
        GUIStyle style = new GUIStyle(GUIStyles[styleName]);
        style.fontSize = fontSzie;
        style.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
        return style;
    }
    
    GUIStyle GetStyle(string styleName, TextAnchor textAnchor, bool isBold = false)
    {
        GUIStyle style = new GUIStyle(GUIStyles[styleName]);
        style.alignment = textAnchor;
        style.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
        return style;
    }
    
    GUIStyle GetStyle(string styleName, int fontSzie, TextAnchor textAnchor, bool isBold = false)
    {
        GUIStyle style = new GUIStyle(GUIStyles[styleName]);
        style.fontSize = fontSzie;
        style.alignment = textAnchor;
        style.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
        return style;
    }

    GUIStyle GetStyle(string styleName, int fontSzie, float fixedHeight)
    {
        GUIStyle style = new GUIStyle(GUIStyles[styleName]);
        style.fontSize = fontSzie;
        style.fixedHeight = fixedHeight;
        return style;
    }
    
    GUIStyle GetStyle(string styleName, Color textColor, bool isBold = false)
    {
        GUIStyle style = new GUIStyle(GUIStyles[styleName]);
        style.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
        style.normal.textColor = textColor;
        return style;
    }
    
    GUIStyle GetStyle(string styleName, int fontSzie, Color textColor, bool isBold = false)
    {
        GUIStyle style = new GUIStyle(GUIStyles[styleName]);
        style.fontSize = fontSzie;
        style.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;
        style.normal.textColor = textColor;
        return style;
    }
    #endregion
}
