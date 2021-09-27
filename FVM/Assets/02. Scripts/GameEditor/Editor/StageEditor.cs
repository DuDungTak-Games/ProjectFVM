using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StageEditor : EditorWindow
{

    StageEditorHelper GizmoHelper;

    bool isEditMode = false;
    
    public float tileFloor = 0;
    float tileUnit = 10;

    Vector3 tileSize = new Vector3(10, 10, 10);

    TileID curTileID;
    GameObject curPrefab;
    float curFloorUnit;

    public TilePrefabData tilePrefabData;
    public TileFloorPrefabData tileFloorPrefabData;
    public TileSetData tileSetData;
    
    private List<Tuple<Vector3, GameObject>> spawnList = new List<Tuple<Vector3, GameObject>>();
    private SerializeDictionary<TileID, SubList<TileSet>> tileSetList = new SerializeDictionary<TileID, SubList<TileSet>>();
    
    [MenuItem("DudungtakGames/Stage Editor")]
    private static void ShowWindow()
    {
        StageEditor window = (StageEditor)EditorWindow.GetWindow(typeof(StageEditor));
        GUIContent guiContent = new GUIContent(GUIContent.none);
        guiContent.text = "Stage Editor";

        window.titleContent = guiContent;
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

    void OnDestroy()
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

        EditorGUILayout.LabelField(string.Format("Edit Mode : {0}", isEditMode ? "ON" : "OFF"),
            GetStyle("Context", isEditMode ? Color.green : Color.red, true), GUILayout.MinHeight(30));

        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("TilePrefabData",
            GetStyle("Context", TextAnchor.LowerLeft,true), GUILayout.MaxWidth(160), GUILayout.MinHeight(30));
        tilePrefabData = (TilePrefabData)EditorGUILayout.ObjectField(tilePrefabData, typeof(TilePrefabData), false, 
            GUILayout.MaxWidth(400), GUILayout.MaxHeight(30));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("TileFloorPrefabData",
            GetStyle("Context", TextAnchor.LowerLeft,true), GUILayout.MaxWidth(160), GUILayout.MinHeight(30));
        tileFloorPrefabData = (TileFloorPrefabData)EditorGUILayout.ObjectField(tileFloorPrefabData, typeof(TileFloorPrefabData), false, 
            GUILayout.MaxWidth(400), GUILayout.MaxHeight(30));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("TileSetData",
            GetStyle("Context", TextAnchor.LowerLeft,true), GUILayout.MaxWidth(160), GUILayout.MinHeight(30));
        tileSetData = (TileSetData)EditorGUILayout.ObjectField(tileSetData, typeof(TileSetData), false, 
            GUILayout.MaxWidth(400), GUILayout.MaxHeight(30));
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            Save();
        }

        EditorGUILayout.Space();
        isEditMode = GUILayout.Toggle(isEditMode, "Edit Mode", GetStyle("Button", 24, Color.white), GUILayout.MinHeight(40));
        
        if(GUILayout.Button("TileSet Load", GetStyle("Button", 20, Color.white), GUILayout.MinHeight(40)))
        {
            LoadTileSet();
        }
        
        if(GUILayout.Button("TileSet Save", GetStyle("Button", 20, Color.white), GUILayout.MinHeight(40)))
        {
            SaveTileSet();
        }
        
        UpdateHelper();
    }

    void DisplaySceneGUI()
    {
        UpdateRay();
        UpdateInput();
        UpdateData();
        
        Handles.BeginGUI();
        
        ShowToolBox();
        ShowInfoBox();
        
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
                        if (e.button == 0) // NOTE : 타일 배치
                        {
                            CreateTile();
                        }
                    }

                    if (e.type == EventType.KeyDown)
                    {
                        if (e.keyCode == KeyCode.V) // NOTE : 타일 배치
                        {
                            CreateTile();
                        }
                        
                        if (e.keyCode == KeyCode.C) // NOTE : 타일 삭제
                        {
                            DeleteTile();
                        }
                    }

                    if (e.type == EventType.MouseMove)
                    {
                        SceneView.RepaintAll();
                    }
                }
            }
        }
    }

    void UpdateData()
    {
        if (tilePrefabData != null)
        {
            if (tilePrefabData.prefabList.ContainsKey(curTileID))
            {
                curPrefab = tilePrefabData.prefabList[curTileID];
            }
            else
            {
                curPrefab = null;
            }
            
            if (tilePrefabData.floorUnitList.ContainsKey(curTileID))
            {
                curFloorUnit = tilePrefabData.floorUnitList[curTileID];
            }
            else
            {
                curFloorUnit = 0;
            }
        }
    }

    int toolBoxWindowID = 1000;
    Rect toolBoxRect = new Rect(10,30, 360, 0);
    void ShowToolBox()
    {
        toolBoxRect = GUILayout.Window (toolBoxWindowID, toolBoxRect, (id) => 
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField (string.Format("TileID ({0} [{1}])", curTileID.ToString(), ((int)curTileID).ToString()), 
                GetStyle("Context", Color.yellow,true), GUILayout.MinHeight(30));
            if(GUILayout.Button("◀", GUILayout.MaxWidth(30), GUILayout.MaxHeight(20))) { SetTileID(false); }
            if(GUILayout.Button("▶", GUILayout.MaxWidth(30), GUILayout.MaxHeight(20))) { SetTileID(true); }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField (string.Format("Prefab ({0})", curPrefab ? curPrefab.name : "NULL"), 
                GetStyle("Context", curPrefab ? Color.green : Color.gray, true), GUILayout.MinHeight(30));
            EditorGUILayout.LabelField (string.Format("Floor Unit ({0})", curFloorUnit), 
                GetStyle("Context", curFloorUnit > 0 ? Color.cyan : Color.gray, true), GUILayout.MinHeight(30));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField ("Floor", GetStyle("Context", 16, TextAnchor.LowerCenter, true), 
                GUILayout.MaxWidth(60), GUILayout.MinHeight(30));

            EditorGUILayout.FloatField(tileFloor, GetStyle("NumberField"), GUILayout.MinHeight(30));

            EditorGUILayout.BeginVertical();
            if(GUILayout.Button("▲", GUILayout.MaxHeight(15))) { SetTileFloor(true); }
            if(GUILayout.Button("▼", GUILayout.MaxHeight(15))) { SetTileFloor(false); }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField (string.Format("Tile Count : {0}", spawnList.Count), 
                GetStyle("Context", true), GUILayout.MinHeight(30));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Plane Pos Reset", GUILayout.MinHeight(30))) { gridPos = Vector3.zero; }
            if (GUILayout.Button("Floor Reset", GUILayout.MinHeight(30))) { tileFloor = 0; }
            if (GUILayout.Button("TileID Reset", GUILayout.MinHeight(30))) { curTileID = 0; }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("TileSet Reset", GUILayout.MinHeight(30)))
            {
                ClearSpawn();
                ClearTile();
            }

            GUI.DragWindow();

        }, "Tool", GUILayout.MinWidth(360));
    }

    int infoBoxWindowID = 1001;
    Rect windowBoxRect = new Rect(10, 840, 260, 0);
    void ShowInfoBox()
    {
        windowBoxRect = GUILayout.Window (infoBoxWindowID, windowBoxRect, (id) => 
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField (string.Format("Tile Pos ({0}, {1}, {2})", tilePos.x, tilePos.y, tilePos.z), 
                GetStyle("Context", true), GUILayout.MinHeight(30));
            EditorGUILayout.LabelField (string.Format("Grid Pos ({0}, {1}, {2})", gridPos.x, gridPos.y, gridPos.z), 
                GetStyle("Context", true), GUILayout.MinHeight(30));
            EditorGUILayout.LabelField (string.Format("Mouse Pos ({0}, {1}, {2})", Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), Mathf.Round(mousePos.z)), 
                GetStyle("Context", true), GUILayout.MinHeight(30));
            EditorGUILayout.EndVertical();
            
            GUI.DragWindow();

        }, "Info", GUILayout.MaxWidth(260));
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
        
        Save();
    }

    void SetTileID(bool isNext)
    {
        if (isNext)
        {
            int last = (int)Enum.GetValues(typeof(TileID)).Cast<TileID>().Last();
            
            if ((int) curTileID >= last)
            {
                curTileID = Enum.GetValues(typeof(TileID)).Cast<TileID>().First();
                return;
            }
            
            curTileID += 1;
        }
        else
        {
            if ((int) curTileID <= 0)
            {
                curTileID = Enum.GetValues(typeof(TileID)).Cast<TileID>().Last();
                return;
            }

            curTileID -= 1;
        }
        
        Save();
    }

    void CreateTile(bool isLoad = false)
    {
        if (!TileCheck() && curPrefab != null)
        {
            GameObject prefab = GetTilePrefab();
            GameObject tile = Instantiate(prefab, tilePos, Quaternion.identity);
            
            spawnList.Add(new Tuple<Vector3, GameObject>(tilePos, tile));

            if (!isLoad)
            {
                AddTileSet();
            }
        }
        
        Save();
    }

    void DeleteTile()
    {
        Tuple<Vector3, GameObject> tile = FindDuplicateTile();
        if (tile != null)
        {
            spawnList.Remove(tile);
            FindBottomTile(true);
            DeleteTileSet(tile.Item1);
            DestroyImmediate(tile.Item2);
        }

        Save();
    }
    
    void ReplaceBottomTile(Tuple<Vector3, GameObject> data)
    {
        Vector3 pos = data.Item1;
        GameObject tile = data.Item2;
        
        spawnList.Remove(data);
        DestroyImmediate(tile);

        if (pos.y % 1 != 0)
        {
            pos.y += 2.5f;
        }
        
        tile = Instantiate(tileFloorPrefabData.GetPrefab(TileFloorID.BOTTOM_TILE), pos, Quaternion.identity);

        spawnList.Add(new Tuple<Vector3, GameObject>(pos, tile));
    }
    
    void ReplaceTopTile(Tuple<Vector3, GameObject> data)
    {
        Vector3 pos = data.Item1;
        GameObject tile = data.Item2;

        spawnList.Remove(data);
        DestroyImmediate(tile);

        tile = Instantiate(tileFloorPrefabData.GetPrefab(pos.y % 1 != 0 ? TileFloorID.TOP_HALF_TILE : TileFloorID.TOP_TILE), pos, Quaternion.identity);

        spawnList.Add(new Tuple<Vector3, GameObject>(pos, tile));
    }
    
    bool TileCheck()
    {
        bool isAlready = FindDuplicateTile() != null;
        
        GizmoHelper.SetState(isAlready);
        
        return isAlready;
    }

    GameObject GetTilePrefab()
    {
        switch (curTileID)
        {
            case TileID.TILE:
                bool hasTop = FindTopTile();
                TileFloorID tileFloorID = hasTop ? TileFloorID.BOTTOM_TILE : TileFloorID.TOP_TILE;
                
                if (tileFloor % 1 != 0)
                {
                    tileFloorID = hasTop ? TileFloorID.BOTTOM_HALF_TILE : TileFloorID.TOP_HALF_TILE;
                }

                return tileFloorPrefabData.GetPrefab(tileFloorID);
            default:
                break;
        }
        
        return curPrefab;
    }

    bool FindTopTile()
    {
        Tuple<Vector3, GameObject> data = spawnList.Find(x =>
            x.Item1 == tilePos + (Vector3.up * 2.5f) ||
            x.Item1 == tilePos + (Vector3.up * 10));

        if (data == null)
        {
            FindBottomTile();
            return false;
        }

        return true;
    }

    bool FindBottomTile(bool isDelete = false)
    {
        Tuple<Vector3, GameObject> data = spawnList.Find(x =>
            x.Item1 == tilePos - (Vector3.up * 7.5f) ||
            x.Item1 == tilePos - (Vector3.up * 10));

        if (data != null)
        {
            if (isDelete)
            {
                ReplaceTopTile(data);
            }
            else
            {
                ReplaceBottomTile(data);
            }
            
            return true;
        }
        
        return false;
    }

    Tuple<Vector3, GameObject> FindDuplicateTile()
    {
        return spawnList.Find(x =>
            x.Item1 == tilePos || 
            x.Item1 == tilePos + (Vector3.up * 2.5f) ||
            x.Item1 == tilePos - (Vector3.up * 2.5f));
    }

    void ClearSpawn()
    {
        foreach (var tile in spawnList)
        {
            DestroyImmediate(tile.Item2);
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("Tile"))
        {
            DestroyImmediate(obj);
        }
        
        spawnList.Clear();
    }

    void ClearTile()
    {
        tileSetList.Clear();
    }

    void AddTileSet()
    {
        if (!tileSetList.ContainsKey(curTileID))
        {
            tileSetList.Add(curTileID, new SubList<TileSet>());
        }

        tileSetList[curTileID].Add(new TileSet(Vector2.zero, tilePos, tileFloor));
    }

    void DeleteTileSet(Vector3 matchPos)
    {
        foreach (var key in tileSetList.Keys)
        {
            TileSet tileSet = tileSetList[key].Find(x => x.spawnPos == matchPos);
            tileSetList[key].Remove(tileSet);
        }
    }

    void SpawnTileSet()
    {
        foreach (var key in tileSetList.Keys)
        {
            foreach (var tileSet in tileSetList[key])
            {
                curTileID = key;
                tileFloor = tileSet.spawnFloor;
                tilePos = tileSet.spawnPos;
                
                UpdateData();
                
                CreateTile(true);
            }
        }
    }

    void LoadTileSet()
    {
        if (tileSetData.tileSetList == null)
            return;
        
        ClearSpawn();
        ClearTile();

        foreach (var key in tileSetData.tileSetList.Keys)
        {
            foreach (var tileSet in tileSetData.tileSetList[key])
            {
                if (!tileSetList.ContainsKey(key))
                {
                    tileSetList.Add(key, new SubList<TileSet>());
                }
            
                tileSetList[key].Add(new TileSet(Vector2.zero, tileSet.spawnPos, tileSet.spawnFloor));
            }
        }

        SpawnTileSet();
    }

    void SaveTileSet()
    {
        if (tileSetData == null)
            return;
        
        tileSetData.tileSetList.Clear();
        
        foreach (var key in tileSetList.Keys)
        {
            foreach (var tileSet in tileSetList[key].OrderBy(x => x.spawnFloor))
            {
                if (!tileSetData.tileSetList.ContainsKey(key))
                {
                    tileSetData.tileSetList.Add(key, new SubList<TileSet>());
                }
            
                tileSetData.tileSetList[key].Add(new TileSet(Vector2.zero, tileSet.spawnPos, tileSet.spawnFloor));
            }
        }

        EditorUtility.SetDirty(tileSetData);
        Save();
    }

    void Save()
    {
        SaveChanges();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
