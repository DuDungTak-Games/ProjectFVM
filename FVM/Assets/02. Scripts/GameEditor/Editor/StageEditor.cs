using System;
using System.Collections.Generic;
using System.Linq;
using DuDungTakGames.Gimic;
using UnityEditor;
using UnityEngine;

public class StageEditor : EditorWindow
{

    StageEditorHelper GizmoHelper;

    bool isEditMode = false, isSubPreset = false;
    
    public float tileFloor = 0;
    float tileUnit = 10;
    Vector3 tileSize = new Vector3(10, 10, 10);
    
    public enum EditType { SELECT, TILE_SINGLE, TILE_PAINT }
    public EditType curEditType { get; private set; }

    TileID curTileID;
    GameObject curPrefab;
    float curFloorUnit;

    EditorTile curEditorTile;
    
    int lvlSelectIdx = 0, subSelectIdx = 0;
    string[] lvlSelect, subSelect;

    public LevelData levelData;
    public LevelData[] datas;

    private List<Tuple<Vector3, GameObject>> spawnList = new List<Tuple<Vector3, GameObject>>();
    
    private SerializeDictionary<TileID, SubList<TileSet>> tileSetList = new SerializeDictionary<TileID, SubList<TileSet>>();
    private List<GimicSet> gimicSetList = new List<GimicSet>();
    
    private SerializeDictionary<TileID, SubList<TileSet>> subTileSetList = new SerializeDictionary<TileID, SubList<TileSet>>();
    private List<GimicSet> subGimicSetList = new List<GimicSet>();
    
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

                tileFloor = 0;
                GizmoResize();
            }
        }
    }

    void DisplayGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Stage Editor", GetStyle("Title"), GUILayout.Height(40));

        EditorGUILayout.LabelField(string.Format("Edit Mode : {0}", isEditMode ? "ON" : "OFF"),
            GetStyle("Context", isEditMode ? Color.green : Color.red, true), GUILayout.MinHeight(30));
        
        EditorGUILayout.LabelField(string.Format("Current Preset : {0}", isSubPreset ? "Sub" : "Main"),
            GetStyle("Context", isSubPreset ? Color.green : Color.red, true), GUILayout.MinHeight(30));

        EditorGUI.BeginChangeCheck();
        
        DisplayLevelPopup();
        DisplaySubTileSetPopup();

        if (EditorGUI.EndChangeCheck())
        {
            Save();
        }

        EditorGUILayout.Space();
        isEditMode = GUILayout.Toggle(isEditMode, "Edit Mode", GetStyle("Button", 24, Color.white), GUILayout.MinHeight(40));

        if (isEditMode)
        {
            isSubPreset = GUILayout.Toggle(isSubPreset, "Preset Toggle", GetStyle("Button", 24, Color.white), GUILayout.MinHeight(40));
        }
        else
        {
            isSubPreset = false;
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if(GUILayout.Button("TileSet Load (ALL)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                LoadTileSet(false); // NOTE : Main Preset (First Load)
                LoadSubTileSet(true); // NOTE : Sub Preset (After Load)
            }
        
            if(GUILayout.Button("TileSet Save (ALL)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                SaveTileSet();
                SaveSubTileSet();
            }
        }
        
        using (new EditorGUILayout.HorizontalScope())
        {
            if(GUILayout.Button("TileSet Load (Main)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                LoadTileSet(true);
            }
        
            if(GUILayout.Button("TileSet Save (Main)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                SaveTileSet();
            }
        }
        
        using (new EditorGUILayout.HorizontalScope())
        {
            if(GUILayout.Button("TileSet Load (Sub)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                LoadSubTileSet(true);
            }
        
            if(GUILayout.Button("TileSet Save (Sub)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                SaveSubTileSet();
            }
        }

        UpdateHelper();
    }

    void DisplayLevelPopup()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Level Data", 
                GetStyle("Context", TextAnchor.LowerLeft,true), GUILayout.MaxWidth(160), GUILayout.MinHeight(30));

            datas = Resources.LoadAll<LevelData>("GameData/Level");
            if (datas.Length > 0)
            {
                lvlSelect = new string[datas.Length];

                if (lvlSelect.Length > 0)
                {
                    for (int i = 0; i < datas.Length; i++)
                    {
                        lvlSelect[i] = datas[i].name;
                    }
                    
                    lvlSelectIdx = EditorGUILayout.Popup(lvlSelectIdx, lvlSelect, GetStyle("Popup"));
                }
                
                levelData = datas[lvlSelectIdx];
            }
        }
    }
    
    void DisplaySubTileSetPopup()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Sub TileSet", 
                GetStyle("Context", TextAnchor.LowerLeft,true), GUILayout.MaxWidth(160), GUILayout.MinHeight(30));

            if (levelData != null)
            {
                if (levelData.subPreset.Count > 0)
                {
                    subSelect = new string[levelData.subPreset.Count];

                    if (subSelect.Length > 0)
                    {
                        for (int i = 0; i < levelData.subPreset.Count; i++)
                        {
                            if (levelData.subPreset[i] != null)
                            {
                                subSelect[i] = levelData.subPreset[i].name;
                            }
                            else
                            {
                                subSelect[i] = "NULL";
                            }
                        }
                        
                        subSelectIdx = EditorGUILayout.Popup(subSelectIdx, subSelect, GetStyle("Popup"));
                    }
                }
            }
        }
    }

    void DisplaySceneGUI()
    {
        UpdateRay();
        UpdateInput();
        UpdateData();
        
        Handles.BeginGUI();
        
        ShowToolBox();
        ShowInfoBox();

        if (curEditType == EditType.SELECT)
        {
            ShowTileBox();
        }
        else
        {
            DeselectTile();
        }
        
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
                    switch (curEditType)
                    {
                        case EditType.SELECT:
                            SelectInput(e);
                            break;
                        case EditType.TILE_SINGLE:
                            SingleInput(e);
                            break;
                        case EditType.TILE_PAINT:
                            PaintInput(e);
                            break;
                        default:
                            break;
                    }
                
                    if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag)
                    {
                        SceneView.RepaintAll();
                    }
                }
            }
        }
    }

    void SelectInput(Event e)
    {
        if (e.type == EventType.MouseDown)
        {
            if (e.button == 0)
            {
                SelectTile();
            }
        }
    }

    bool isMouseDown = false; // NOTE : ���콺 �巡�׷� ���� ���� ����
    void SingleInput(Event e)
    {
        if (e.type == EventType.MouseDown)
        {
            if (e.button == 0)
            {
                CreateTile();
            }
            
            if (e.button == 1)
            {
                isMouseDown = true;
            }
        }

        if (e.type == EventType.MouseUp)
        {
            if (e.button == 1 && isMouseDown)
            {
                DeleteTile();
            }
        }

        if (e.type == EventType.MouseDrag || e.type == EventType.MouseMove)
        {
            isMouseDown = false;
        }
    }

    bool isCreate = true; // NOTE : ��ġ or ���� ��� 
    void PaintInput(Event e)
    {
        if (e.type == EventType.MouseDown)
        {
            if (e.button == 0)
            {
                isMouseDown = true;
            }
        }

        if (e.type == EventType.MouseUp)
        {
            if (e.button == 0)
            {
                isMouseDown = false;
            }
        }

        if (e.type == EventType.MouseLeaveWindow)
        {
            isMouseDown = false;
        }
        
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.C)
            {
                isCreate = !isCreate;
            }
        }
        
        if (isMouseDown)
        {
            if (isCreate)
            {
                CreateTile();
            }
            else
            {
                DeleteTile();
            }
        }
    }

    void UpdateData()
    {
        if (levelData != null && levelData.tilePrefabData != null && levelData.tileFloorPrefabData != null)
        {
            if (levelData.tilePrefabData.prefabList.ContainsKey(curTileID))
            {
                curPrefab = levelData.tilePrefabData.prefabList[curTileID];
            }
            else
            {
                curPrefab = null;
            }
            
            if (levelData.tilePrefabData.floorUnitList.ContainsKey(curTileID))
            {
                curFloorUnit = levelData.tilePrefabData.floorUnitList[curTileID];
            }
            else
            {
                curFloorUnit = 0;
            }
        }
        else
        {
            curPrefab = null;
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
            if(GUILayout.Button("��", GUILayout.MaxWidth(30), GUILayout.MaxHeight(20))) { SetTileID(false); }
            if(GUILayout.Button("��", GUILayout.MaxWidth(30), GUILayout.MaxHeight(20))) { SetTileID(true); }
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
            if(GUILayout.Button("��", GUILayout.MaxHeight(15))) { SetTileFloor(true); }
            if(GUILayout.Button("��", GUILayout.MaxHeight(15))) { SetTileFloor(false); }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField ("Edit Type", GetStyle("Context", 16, TextAnchor.LowerCenter, true), 
                GUILayout.MaxWidth(80), GUILayout.MinHeight(30));

            curEditType = (EditType)EditorGUILayout.EnumPopup(curEditType, GetStyle("Popup"));
            EditorGUILayout.EndHorizontal();

            switch (curEditType)
            {
                case EditType.TILE_PAINT:
                    EditorGUILayout.LabelField (string.Format("Toggle : {0}", isCreate ? "Create" : "Delete"), 
                        GetStyle("Context", isCreate ? Color.green : Color.magenta, true), GUILayout.MinHeight(30));
                    EditorGUILayout.LabelField ("'C' �� ������ ��ġ / ���� ��� ����", 
                        GetStyle("Context", 16, TextAnchor.LowerLeft, true), GUILayout.MinHeight(30));
                    EditorGUILayout.Space();
                    break;
                default:
                    break;
            }

            EditorGUILayout.LabelField (string.Format("Tile Count : {0}", spawnList.Count), 
                GetStyle("Context", true), GUILayout.MinHeight(30));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Plane Pos Reset", GUILayout.MinHeight(30))) { gridPos = Vector3.zero; }
            if (GUILayout.Button("Floor Reset", GUILayout.MinHeight(30))) { tileFloor = 0; }
            if (GUILayout.Button("TileID Reset", GUILayout.MinHeight(30))) { curTileID = 0; }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("TileSet Reset (ALL)", GUILayout.MinHeight(30)))
            {
                ClearTile();
                ClearSubTile();
                ClearSpawn();
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
    
    int tileBoxWindowID = 1002;
    Rect tileBoxRect = new Rect(390, 30, 260, 0);
    void ShowTileBox()
    {
        tileBoxRect = GUILayout.Window (tileBoxWindowID, tileBoxRect, (id) => 
        {
            if (curEditorTile != null)
            {
                EditorGUILayout.LabelField (string.Format("Tile : {0}", curEditorTile.gameObject.name), 
                    GetStyle("Context", true), GUILayout.MinHeight(30));
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField (string.Format("Tile Floor : {0}", curEditorTile.floor), 
                    GetStyle("Context", true), GUILayout.MinHeight(30));
                EditorGUILayout.LabelField (string.Format("Tile Pos : {0}", curEditorTile.spawnPos), 
                    GetStyle("Context", true), GUILayout.MinHeight(30));
            }
            else
            {
                EditorGUILayout.LabelField ("Ÿ���� �������ּ���!", 
                    GetStyle("Context", TextAnchor.MiddleCenter, true), GUILayout.MinHeight(30));
            }

            // TODO : ����� ��쿡 �߰� ���� �Ǵ� ���� ǥ��
            
            GUI.DragWindow();

        }, "Current Selected Tile", GUILayout.MaxWidth(260));
    }

    void GizmoResize()
    {
        tileSize = new Vector3(tileUnit, (Mathf.Abs(tileFloor) % 1) == 0.5f ? (tileUnit/2) : tileUnit, tileUnit);
        GizmoHelper.SetSize(tileSize);
    }

    void SetTileFloor(bool isUp)
    {
        tileFloor += isUp ? 0.5f : -0.5f;
        
        float remian = Mathf.Abs(tileFloor) % 1;
        if (remian != 0 && remian != 0.5f)
        {
            tileFloor = Mathf.RoundToInt(tileFloor);
        }

        DeselectTile();
        
        GizmoResize();
        
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

    void SelectTile()
    {
        GameObject tile = spawnList.Find(x =>
            x.Item1 == tilePos || 
            x.Item1 == tilePos + (Vector3.up * 2.5f) ||
            x.Item1 == tilePos - (Vector3.up * 2.5f))?.Item2;
        
        if (tile != null)
        {
            if (tile.TryGetComponent(out curEditorTile))
            {
                tileSize = new Vector3(tileUnit, (Mathf.Abs(curEditorTile.floor) % 1) == 0.5f ? (tileUnit/2) : tileUnit, tileUnit);
                GizmoHelper.SetSize(tileSize);
                GizmoHelper.SetEditorTile(curEditorTile);
            }
        }
        else
        {
            DeselectTile();
        }
    }

    void DeselectTile()
    {
        curEditorTile = null;
        GizmoHelper.SetEditorTile(curEditorTile);
    }

    #region MainTileset
    void CreateTile(bool isLoad = false)
    {
        if (!TileCheck() && curPrefab != null)
        {
            GameObject prefab = GetTilePrefab();
            GameObject tile = Instantiate(prefab, tilePos, Quaternion.identity);

            spawnList.Add(new Tuple<Vector3, GameObject>(tilePos, tile));

            EditorTile editorTile = tile.AddComponent<EditorTile>();
            editorTile.floor = tileFloor;
            editorTile.spawnPos = tilePos;

            if ((int) curTileID >= 1000)
            {
                GimicObject gimic = tile.AddComponent<GimicObject>();
                gimic.SetGimicID(0);
                
                // TODO : isLoad �� ���, �̹� GimicSet �� ������ ���, GimicSet List ���� �ҷ�����
            }

            if (!isLoad)
            {
                if (isSubPreset)
                {
                    AddSubTileSet();
                }
                else
                {
                    AddTileSet();
                }
            }
        }
        
        Save();
    }

    void DeleteTile()
    {
        Tuple<Vector3, GameObject> tile = FindDuplicateTile();
        if (tile != null)
        {
            if (isSubPreset)
            {
                DeleteSubTileSet(tile);                
            }
            else
            {
                DeleteTileSet(tile);
            }
        }

        Save();
    }
    
    void ReplaceBottomTile(Tuple<Vector3, GameObject> data)
    {
        Vector3 pos = data.Item1;
        GameObject tile = data.Item2;
        
        float floor = 0;
        EditorTile editorTile;
        if (tile.TryGetComponent(out editorTile))
        {
            floor = editorTile.floor;
        }
        
        spawnList.Remove(data);
        DestroyImmediate(tile);

        if (pos.y % 1 != 0)
        {
            pos.y += 2.5f;
        }
        
        tile = Instantiate(levelData.tileFloorPrefabData.GetPrefab(TileFloorID.BOTTOM_TILE), pos, Quaternion.identity);
        
        editorTile = tile.AddComponent<EditorTile>();
        editorTile.floor = floor;
        editorTile.spawnPos = pos;

        spawnList.Add(new Tuple<Vector3, GameObject>(pos, tile));
    }
    
    void ReplaceTopTile(Tuple<Vector3, GameObject> data)
    {
        Vector3 pos = data.Item1;
        GameObject tile = data.Item2;

        float floor = 0;
        EditorTile editorTile;
        if (tile.TryGetComponent(out editorTile))
        {
            floor = editorTile.floor;
        }

        spawnList.Remove(data);
        DestroyImmediate(tile);

        tile = Instantiate(levelData.tileFloorPrefabData.GetPrefab(pos.y % 1 != 0 ? TileFloorID.TOP_HALF_TILE : TileFloorID.TOP_TILE), pos, Quaternion.identity);

        editorTile = tile.AddComponent<EditorTile>();
        editorTile.floor = floor;
        editorTile.spawnPos = pos;
        
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

                return levelData.tileFloorPrefabData.GetPrefab(tileFloorID);
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
        gimicSetList.Clear();
    }

    void ClearSubTile()
    {
        subTileSetList.Clear();
        subGimicSetList.Clear();
    }

    void AddTileSet()
    {
        if (!tileSetList.ContainsKey(curTileID))
        {
            tileSetList.Add(curTileID, new SubList<TileSet>());
        }

        tileSetList[curTileID].Add(new TileSet(Vector2.zero, tilePos, tileFloor));
        
        if ((int) curTileID >= 1000)
        {
            gimicSetList.Add(new GimicSet(tilePos, 0));
        }
    }

    void DeleteTileSet(Tuple<Vector3, GameObject> tile)
    {
        foreach (var key in tileSetList.Keys)
        {
            TileSet tileSet = tileSetList[key].Find(x => x.spawnPos == tile.Item1);
            if (tileSetList[key].Remove(tileSet))
            {
                GimicSet gimicSet = subGimicSetList.Find(x => x.targetPos == tile.Item1);
                subGimicSetList.Remove(gimicSet);
                
                spawnList.Remove(tile);
                FindBottomTile(true);
                DestroyImmediate(tile.Item2);
            }
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

        SpawnSubTileSet();
    }

    void LoadTileSet(bool canSpawn)
    {
        if (levelData.mainPreset.tileSetList == null)
            return;
        
        ClearTile();

        foreach (var key in levelData.mainPreset.tileSetList.Keys)
        {
            foreach (var tileSet in levelData.mainPreset.tileSetList[key])
            {
                if (!tileSetList.ContainsKey(key))
                {
                    tileSetList.Add(key, new SubList<TileSet>());
                }
            
                tileSetList[key].Add(new TileSet(Vector2.zero, tileSet.spawnPos, tileSet.spawnFloor));
            }
        }
        
        foreach (var gimcset in levelData.mainPreset.gimicSetList)
        {
            gimicSetList.Add(new GimicSet(gimcset.targetPos, gimcset.gimicID));
        }

        if (canSpawn)
        {
            ClearSpawn();
            
            SpawnTileSet();
            
            // TODO : GIMIC ���� �Լ� ȣ��
        }
    }

    void SaveTileSet()
    {
        if (levelData.mainPreset == null)
            return;
        
        levelData.mainPreset.tileSetList.Clear();
        
        foreach (var key in tileSetList.Keys)
        {
            foreach (var tileSet in tileSetList[key].OrderBy(x => x.spawnFloor))
            {
                if (!levelData.mainPreset.tileSetList.ContainsKey(key))
                {
                    levelData.mainPreset.tileSetList.Add(key, new SubList<TileSet>());
                }
            
                levelData.mainPreset.tileSetList[key].Add(new TileSet(Vector2.zero, tileSet.spawnPos, tileSet.spawnFloor));
            }
        }
        
        levelData.mainPreset.gimicSetList.Clear();
        
        foreach (var gimcset in subGimicSetList)
        {
            levelData.mainPreset.gimicSetList.Add(new GimicSet(gimcset.targetPos, gimcset.gimicID));
        }

        EditorUtility.SetDirty(levelData.mainPreset);

        Save();
    }
    #endregion

    #region SubTileset
    void AddSubTileSet()
    {
        if (!subTileSetList.ContainsKey(curTileID))
        {
            subTileSetList.Add(curTileID, new SubList<TileSet>());
        }

        subTileSetList[curTileID].Add(new TileSet(Vector2.zero, tilePos, tileFloor));

        if ((int) curTileID >= 1000)
        {
            subGimicSetList.Add(new GimicSet(tilePos, 0));
        }
    }
    
    void DeleteSubTileSet(Tuple<Vector3, GameObject> tile)
    {
        foreach (var key in subTileSetList.Keys)
        {
            TileSet tileSet = subTileSetList[key].Find(x => x.spawnPos == tile.Item1);
            if (subTileSetList[key].Remove(tileSet))
            {
                GimicSet gimicSet = subGimicSetList.Find(x => x.targetPos == tile.Item1);
                subGimicSetList.Remove(gimicSet);
                
                spawnList.Remove(tile);
                FindBottomTile(true);
                DestroyImmediate(tile.Item2);
            }
        }
    }
    
    void SpawnSubTileSet()
    {
        foreach (var key in subTileSetList.Keys)
        {
            foreach (var tileSet in subTileSetList[key])
            {
                curTileID = key;
                tileFloor = tileSet.spawnFloor;
                tilePos = tileSet.spawnPos;
                
                UpdateData();
                
                CreateTile(true);
            }
        }
    }
    
    void LoadSubTileSet(bool canSpawn)
    {
        if (levelData.subPreset.Count <= 0)
            return;

        if (levelData.subPreset[subSelectIdx].tileSetList == null)
        {
            levelData.subPreset[subSelectIdx].tileSetList = new SerializeDictionary<TileID, SubList<TileSet>>();
        }
        
        ClearSubTile();

        foreach (var key in levelData.subPreset[subSelectIdx].tileSetList.Keys)
        {
            foreach (var tileSet in levelData.subPreset[subSelectIdx].tileSetList[key])
            {
                if (!subTileSetList.ContainsKey(key))
                {
                    subTileSetList.Add(key, new SubList<TileSet>());
                }
            
                subTileSetList[key].Add(new TileSet(Vector2.zero, tileSet.spawnPos, tileSet.spawnFloor));
            }
        }
        
        foreach (var gimcset in levelData.subPreset[subSelectIdx].gimicSetList)
        {
            gimicSetList.Add(new GimicSet(gimcset.targetPos, gimcset.gimicID));
        }
        
        if (canSpawn)
        {
            ClearSpawn();
            
            SpawnTileSet();
            
            // TODO : GIMIC ���� �Լ� ȣ��
        }
    }

    void SaveSubTileSet()
    {
        if (levelData.subPreset.Count <= 0)
            return;

        if (levelData.subPreset[subSelectIdx].tileSetList == null)
        {
            levelData.subPreset[subSelectIdx].tileSetList = new SerializeDictionary<TileID, SubList<TileSet>>();
        }
        
        levelData.subPreset[subSelectIdx].tileSetList.Clear();
        
        foreach (var key in subTileSetList.Keys)
        {
            foreach (var tileSet in subTileSetList[key].OrderBy(x => x.spawnFloor))
            {
                if (!levelData.subPreset[subSelectIdx].tileSetList.ContainsKey(key))
                {
                    levelData.subPreset[subSelectIdx].tileSetList.Add(key, new SubList<TileSet>());
                }
            
                levelData.subPreset[subSelectIdx].tileSetList[key].Add(new TileSet(Vector2.zero, tileSet.spawnPos, tileSet.spawnFloor));
            }
        }
        
        levelData.subPreset[subSelectIdx].gimicSetList.Clear();
        
        foreach (var gimcset in subGimicSetList)
        {
            levelData.subPreset[subSelectIdx].gimicSetList.Add(new GimicSet(gimcset.targetPos, gimcset.gimicID));
        }

        EditorUtility.SetDirty(levelData.subPreset[subSelectIdx]);
        
        Save();
    }
    #endregion

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
        
        GUIStyle popupStyle = new GUIStyle(EditorStyles.popup);
        popupStyle.alignment = TextAnchor.MiddleCenter;
        popupStyle.fontStyle = FontStyle.Bold;
        popupStyle.fontSize = 16;
        popupStyle.fixedHeight = 30;
        GUIStyles.Add("Popup", popupStyle);
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
