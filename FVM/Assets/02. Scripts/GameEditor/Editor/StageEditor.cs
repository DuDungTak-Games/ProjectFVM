using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using DuDungTakGames.Extensions;

public class StageEditor : EditorWindow
{

    StageEditorHelper GizmoHelper;

    bool isEditMode = false, isSubPreset = false;
    
    public float tileFloor = 0;
    float tileUnit = 10;
    float halfHeightUnit = 7.5f;
    Vector3 tileSize = new Vector3(10, 10, 10);
    
    public enum EditType { SELECT, TILE_SINGLE, TILE_PAINT }
    public EditType curEditType { get; private set; }

    TileID curTileID;
    GameObject curPrefab, prevPrefab;
    Vector3 curOffset;

    GameObject previewTile;
    EditorTile curEditorTile;
    GimicObject curGimicObject;
    
    int thmSelectIdx = 0;
    string[] thmSelect;
    
    public ThemeData themeData;
    public ThemeData[] themeDatas;
    
    int lvlSelectIdx = 0, subSelectIdx = 0;
    string[] lvlSelect, subSelect;

    public LevelData levelData;
    public LevelData[] levelDatas;

    private List<Tuple<Vector3, GameObject>> spawnList = new List<Tuple<Vector3, GameObject>>();
    
    private Dictionary<TileID, List<TileSet>> tileSetList = new Dictionary<TileID, List<TileSet>>();
    private List<GimicSet> gimicSetList = new List<GimicSet>();
    
    private Dictionary<TileID, List<TileSet>> subTileSetList = new Dictionary<TileID, List<TileSet>>();
    private List<GimicSet> subGimicSetList = new List<GimicSet>();
    
    [MenuItem("Custom Editor/Stage Editor")]
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
        
        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.LabelField("플레이 모드에서는 수정이 불가능합니다!!",
                GetStyle("Context", Color.red, true), GUILayout.MinHeight(30));
            return;
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
        
        if(EditorApplication.isPlaying)
        {
            if (previewTile != null)
            {
                DestroyImmediate(previewTile);
            }

            return;
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

        DisplayThemePopup();
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
                ClearSpawnTile();
                
                LoadTileSet(levelData.mainPreset, ref tileSetList, ref gimicSetList);
                LoadTileSet(levelData.subPreset[subSelectIdx], ref subTileSetList, ref subGimicSetList);
                
                SpawnTileSet(tileSetList);
                SpawnTileSet(subTileSetList);

                ResetState();
            }
        
            if(GUILayout.Button("TileSet Save (ALL)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                if (CheckLevelData())
                {
                    SaveTileSet(levelData.mainPreset, tileSetList, gimicSetList);
                    SaveTileSet(levelData.subPreset[subSelectIdx], subTileSetList, subGimicSetList);
                }
            }
        }
        
        using (new EditorGUILayout.HorizontalScope())
        {
            if(GUILayout.Button("TileSet Load (Main Preset)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                if (CheckLevelData())
                {
                    ClearSpawnTile();
                    
                    LoadTileSet(levelData.mainPreset, ref tileSetList, ref gimicSetList);
                    
                    SpawnTileSet(tileSetList);

                    ResetState();
                }
            }
        
            if(GUILayout.Button("TileSet Save (Main Preset)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                if (CheckLevelData())
                {
                    SaveTileSet(levelData.mainPreset, tileSetList, gimicSetList);
                }
            }
        }
        
        using (new EditorGUILayout.HorizontalScope())
        {
            if(GUILayout.Button("TileSet Load (Sub Preset)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                if (CheckLevelData())
                {
                    ClearSpawnTile();
                    
                    LoadTileSet(levelData.subPreset[subSelectIdx], ref subTileSetList, ref subGimicSetList);
                    
                    SpawnTileSet(subTileSetList);

                    ResetState();
                }
            }
        
            if(GUILayout.Button("TileSet Save (Sub Preset)", GetStyle("Button", 16, Color.white), GUILayout.MinHeight(40)))
            {
                if (CheckLevelData())
                {
                    SaveTileSet(levelData.subPreset[subSelectIdx], subTileSetList, subGimicSetList);
                }
            }
        }
        
        using (new EditorGUILayout.HorizontalScope())
        {
            if(GUILayout.Button("Test Play", GetStyle("Button", 24, Color.white), GUILayout.MinHeight(40)))
            {
                TileManager tm = GameObject.FindObjectOfType<TileManager>();
                if (tm != null)
                {
                    ClearTileSet(ref tileSetList, ref gimicSetList);
                    ClearTileSet(ref subTileSetList, ref subGimicSetList);
                    ClearSpawnTile();

                    ResetState();

                    tm.prefabData = themeData.tilePrefabData;
                    tm.floorPrefabData = themeData.tileFloorPrefabData;
                    tm.mainTileSetData = levelData.mainPreset;
                    tm.subTileSetData = levelData.subPreset[subSelectIdx];
                    
                    EditorApplication.isPlaying = true;
                }
            }
        }

        UpdateHelper();
    }
    
    void DisplayThemePopup()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Theme Data", 
                GetStyle("Context", TextAnchor.LowerLeft,true), GUILayout.MaxWidth(160), GUILayout.MinHeight(30));

            themeDatas = Resources.LoadAll<ThemeData>("GameData/Theme");
            if (themeDatas.Length > 0)
            {
                thmSelect = new string[themeDatas.Length];

                if (thmSelect.Length > 0)
                {
                    for (int i = 0; i < themeDatas.Length; i++)
                    {
                        thmSelect[i] = themeDatas[i].name;
                    }
                    
                    thmSelectIdx = EditorGUILayout.Popup(thmSelectIdx, thmSelect, GetStyle("Popup"));
                }
                
                themeData = themeDatas[thmSelectIdx];
            }
        }
    }

    void DisplayLevelPopup()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Level Data", 
                GetStyle("Context", TextAnchor.LowerLeft,true), GUILayout.MaxWidth(160), GUILayout.MinHeight(30));

            levelDatas = Resources.LoadAll<LevelData>("GameData/Level");
            if (levelDatas.Length > 0)
            {
                lvlSelect = new string[levelDatas.Length];

                if (lvlSelect.Length > 0)
                {
                    for (int i = 0; i < levelDatas.Length; i++)
                    {
                        lvlSelect[i] = levelDatas[i].name;
                    }
                    
                    lvlSelectIdx = EditorGUILayout.Popup(lvlSelectIdx, lvlSelect, GetStyle("Popup"));
                }
                
                levelData = levelDatas[lvlSelectIdx];
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
        else if(curEditorTile != null)
        {
            DeselectTile();
        }

        if (curPrefab != null)
        {
            ShowPreviewBox();
        }
        
        ShowPreviewTile();
        
        Handles.EndGUI();
    }

    
    
    Vector3 tilePos = Vector3.zero;
    Vector3 multiTilePos = Vector3.zero;
    Vector3 tileRot = Vector3.zero;
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

                // NOTE : 일반 타일만 halfHeightUnit 사용
                float offsetY = (IsGimic(curTileID) ? (halfHeightUnit - 2.5f) : halfHeightUnit);

                float posX = (Mathf.RoundToInt(mousePos.x / tileUnit) * tileUnit);
                float posY = (Mathf.FloorToInt(tileFloor) * tileUnit) + (tileFloor % 1 != 0 ? offsetY : 0);
                float posZ = (Mathf.RoundToInt(mousePos.z / tileUnit) * tileUnit);

                tilePos = new Vector3(posX, posY, posZ);
                gridPos = new Vector3(posX, (tileFloor * (tileUnit) - tileUnit/2), posZ);
                
                if (prevTilePos != tilePos)
                {
                    prevTilePos = tilePos;
                    TileCheck();
                }

                GizmoHelper.SetPos(tilePos, multiTilePos, tileRot, gridPos, mousePos);
            }
        }
    }
    
    void UpdateHelper()
    {
        GizmoHelper.SetInfo(isEditMode, tileUnit);
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

                    CommonInput(e);

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
        
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.C)
            {
                RotateTile();
            }

            // NOTE : 기믹 ID 표시 토글
            if (e.keyCode == KeyCode.F)
            {
                EditorTile.showGimicID = !EditorTile.showGimicID;
            }
        }
    }

    bool isMouseDown = false; // NOTE : 마우스 드래그로 인한 삭제 방지
    void SingleInput(Event e)
    {
        if (e.type == EventType.MouseDown)
        {
            if (e.button == 0)
            {
                if (multiTilePos == Vector3.zero)
                {
                    CreateTile();
                }
                else
                {
                    CreateMultiTile();
                }
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
                if (multiTilePos == Vector3.zero)
                {
                    DeleteTile();
                }
                else
                {
                    DeleteMultiTile();
                }
            }
        }
        
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.C)
            {
                RotateTile();
            }
        }

        if (e.type == EventType.MouseDrag || e.type == EventType.MouseMove)
        {
            isMouseDown = false;
        }
    }

    bool isCreate = true; // NOTE : 배치 or 삭제 모드 
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
                RotateTile();
            }
            
            if (e.keyCode == KeyCode.F)
            {
                isCreate = !isCreate;
            }
        }
        
        if (isMouseDown)
        {
            if (isCreate)
            {
                if(multiTilePos == Vector3.zero)
                {
                    CreateTile();
                }
                else
                {
                    CreateMultiTile();
                }
            }
            else
            {
                if (multiTilePos == Vector3.zero)
                {
                    DeleteTile();
                }
                else
                {
                    DeleteMultiTile();
                }
            }
        }
    }

    void CommonInput(Event e)
    {
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.V)
            {
                SetTileFloor(false);
            }

            if (e.keyCode == KeyCode.B)
            {
                SetTileFloor(true);
            }
        }
    }

    void UpdateData()
    {
        if (themeData != null)
        {
            if (themeData.tilePrefabData != null && themeData.tileFloorPrefabData != null)
            {
                if (themeData.tilePrefabData.prefabList.ContainsKey(curTileID))
                {
                    curPrefab = themeData.tilePrefabData.prefabList[curTileID];
                }
                else
                {
                    curPrefab = null;
                }

                if (themeData.tilePrefabData.offsetList.ContainsKey(curTileID))
                {
                    curOffset = themeData.tilePrefabData.offsetList[curTileID];
                }
                else
                {
                    curOffset = Vector3.zero;
                }
            }
        }
    }

    void UpdateGimicData(ref List<GimicSet> gimicSetList)
    {
        GimicSet gimicSet = gimicSetList.Find(x => x.targetPos == curEditorTile.tilePos);
        if (gimicSet != null)
        {
            gimicSetList.Remove(gimicSet);
            gimicSet.ID = curGimicObject.ID;
            gimicSetList.Add(gimicSet);
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
            EditorGUILayout.LabelField (string.Format("Offset ({0}, {1}, {2})", curOffset.x, curOffset.y, curOffset.z), 
                GetStyle("Context", curOffset != Vector3.zero ? Color.cyan : Color.gray, true), GUILayout.MinHeight(30));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Multi Tile Pos", GetStyle("Context", Color.cyan, true), GUILayout.MinHeight(30));
            multiTilePos.x = EditorGUILayout.FloatField(multiTilePos.x, GetStyle("NumberField"), GUILayout.MinHeight(30));
            multiTilePos.y = EditorGUILayout.FloatField(multiTilePos.y, GetStyle("NumberField"), GUILayout.MinHeight(30));
            multiTilePos.z = EditorGUILayout.FloatField(multiTilePos.z, GetStyle("NumberField"), GUILayout.MinHeight(30));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField ("Floor", GetStyle("Context", 16, TextAnchor.LowerCenter, true), 
                GUILayout.MaxWidth(60), GUILayout.MinHeight(30));

            EditorGUILayout.FloatField(tileFloor, GetStyle("NumberField"), GUILayout.MinHeight(30));

            EditorGUILayout.BeginVertical();
            if(GUILayout.Button("▲", GUILayout.MaxHeight(15))) { SetTileFloor(true); }
            if(GUILayout.Button("▼", GUILayout.MaxHeight(15))) { SetTileFloor(false); }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField ("Edit Type", GetStyle("Context", 16, TextAnchor.LowerCenter, true), 
                GUILayout.MaxWidth(80), GUILayout.MinHeight(30));
            
            curEditType = (EditType)EditorGUILayout.EnumPopup(curEditType, GetStyle("Popup"));
            EditorGUILayout.EndHorizontal();

            switch (curEditType)
            {
                case EditType.SELECT:
                    EditorGUILayout.LabelField ("'C' 를 눌러서 회전 가능 (타일 기준 Y축 우회전)", 
                        GetStyle("Context", 16, TextAnchor.LowerLeft, true), GUILayout.MinHeight(30));
                    EditorGUILayout.LabelField("'F' 를 눌러서 기믹 ID 표시 토글 가능",
                        GetStyle("Context", 16, TextAnchor.LowerLeft, true), GUILayout.MinHeight(30));
                    break;
                case EditType.TILE_SINGLE:
                    EditorGUILayout.LabelField ("'C' 를 눌러서 회전 가능 (타일 기준 Y축 우회전)", 
                        GetStyle("Context", 16, TextAnchor.LowerLeft, true), GUILayout.MinHeight(30));
                    break;
                case EditType.TILE_PAINT:
                    EditorGUILayout.LabelField (string.Format("Toggle : {0}", isCreate ? "Create" : "Delete"), 
                        GetStyle("Context", isCreate ? Color.green : Color.magenta, true), GUILayout.MinHeight(30));
                    EditorGUILayout.LabelField ("'C' 를 눌러서 회전 가능 (타일 기준 Y축 우회전)", 
                        GetStyle("Context", 16, TextAnchor.LowerLeft, true), GUILayout.MinHeight(30));
                    EditorGUILayout.LabelField ("'F' 를 눌러서 배치 & 삭제 토글 가능", 
                        GetStyle("Context", 16, TextAnchor.LowerLeft, true), GUILayout.MinHeight(30));
                    break;
                default:
                    break;
            }

            EditorGUILayout.LabelField("'V' 와 'B' 를 눌러서 Floor 업/다운 가능",
                        GetStyle("Context", 16, TextAnchor.LowerLeft, true), GUILayout.MinHeight(30));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField (string.Format("Tile Count : {0}", spawnList.Count), 
                GetStyle("Context", true), GUILayout.MinHeight(30));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Grid Pos Reset", GUILayout.MinHeight(30))) { gridPos = Vector3.zero; }
            if (GUILayout.Button("Floor Reset", GUILayout.MinHeight(30))) { tileFloor = 0; }
            if (GUILayout.Button("TileID Reset", GUILayout.MinHeight(30))) { curTileID = 0; }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("TileSet Reset (ALL)", GUILayout.MinHeight(30)))
            {
                ClearTileSet(ref tileSetList, ref gimicSetList);
                ClearTileSet(ref subTileSetList, ref subGimicSetList);
                ClearSpawnTile();
            }

            GUI.DragWindow();

        }, "Tool", GUILayout.MinWidth(360));
    }

    int infoBoxWindowID = 1001;
    Rect windowBoxRect = new Rect(10, 800, 260, 0);
    void ShowInfoBox()
    {
        windowBoxRect = GUILayout.Window (infoBoxWindowID, windowBoxRect, (id) => 
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField (string.Format("Tile Pos ({0}, {1}, {2})", tilePos.x, tilePos.y, tilePos.z), 
                GetStyle("Context", true), GUILayout.MinHeight(30));
            EditorGUILayout.LabelField (string.Format("Tile Rot ({0}, {1}, {2})", tileRot.x, tileRot.y, tileRot.z), 
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
                EditorGUILayout.LabelField (string.Format("Tile ID : {0}", curEditorTile.tileID), 
                    GetStyle("Context", true), GUILayout.MinHeight(30));
                EditorGUILayout.LabelField (string.Format("Spawn Pos : {0}", curEditorTile.transform.position), 
                    GetStyle("Context", true), GUILayout.MinHeight(30));
                EditorGUILayout.LabelField (string.Format("Spawn Rot : {0}", curEditorTile.spawnRot), 
                    GetStyle("Context", true), GUILayout.MinHeight(30));
                EditorGUILayout.LabelField (string.Format("Spawn Floor : {0}", curEditorTile.floor), 
                    GetStyle("Context", true), GUILayout.MinHeight(30));

                if (curGimicObject != null)
                {
                    EditorGUILayout.Space();
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField ("Gimic ID", GetStyle("Context", 16, TextAnchor.LowerCenter, true), 
                        GUILayout.MaxWidth(80), GUILayout.MinHeight(30));

                    if (GUILayout.Button("◀◀", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30))) { SetGimicID(-10); }
                    if (GUILayout.Button("◀", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30))) { SetGimicID(false); }
                    EditorGUILayout.FloatField(curGimicObject.ID, GetStyle("NumberField"), GUILayout.MinHeight(30));
                    if (GUILayout.Button("▶", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30))) { SetGimicID(true); }
                    if (GUILayout.Button("▶▶", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30))) { SetGimicID(10); }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField ("타일을 선택해주세요!", 
                    GetStyle("Context", TextAnchor.MiddleCenter, true), GUILayout.MinHeight(30));
            }

            GUI.DragWindow();

        }, "Current Selected Tile", GUILayout.MaxWidth(260));
    }
    
    int previewBoxWindowID = 1003;
    Rect previewBoxRect = new Rect(10, 640, 120, 0);
    void ShowPreviewBox()
    {
        previewBoxRect = GUILayout.Window (previewBoxWindowID, previewBoxRect, (id) => 
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
            
                Texture2D texture = AssetPreview.GetAssetPreview(curPrefab);
                GUILayout.Label(texture, GUILayout.MaxWidth(120));
                
                GUILayout.FlexibleSpace();
            }

            GUI.DragWindow();

        }, "Prefab Preview", GUILayout.MaxWidth(260));
    }

    void ShowPreviewTile()
    {
        if (curEditType == EditType.SELECT)
        {
            if (previewTile != null)
            {
                DestroyImmediate(previewTile);
            }

            return;
        }
        
        if (prevPrefab != GetTilePrefab(false))
        {
            prevPrefab = GetTilePrefab(false);

            if (previewTile != null)
            {
                DestroyImmediate(previewTile);
            }
        }
        
        if (previewTile == null)
        {
            if (curPrefab != null)
            {
                GameObject prefab = GetTilePrefab(false);
                previewTile = Instantiate(prefab, GetPosByOffset(), Quaternion.Euler(tileRot));
                previewTile.name = previewTile.name.Replace("(Clone)", " (Preview)").Trim();

                ClearCollider(previewTile);
            }
        }
        else
        {
            previewTile.transform.SetPosition(GetPosByOffset());
            previewTile.transform.SetRotation(Quaternion.Euler(tileRot));
            
            if (curPrefab == null)
            {
                DestroyImmediate(previewTile);
            }
        }
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

            curTileID = curTileID.Next();
        }
        else
        {
            if ((int) curTileID <= 0)
            {
                curTileID = Enum.GetValues(typeof(TileID)).Cast<TileID>().Last();
                return;
            }

            curTileID = curTileID.Previous();
        }
        
        Save();
    }

    void SetGimicID(bool isNext)
    {
        int id = curGimicObject.ID + (isNext ? 1 : -1);
        ChangeGimicID(id);
    }

    void SetGimicID(int value)
    {
        int id = (curGimicObject.ID + value);
        ChangeGimicID(id);
    }

    void ChangeGimicID(int newID)
    {
        newID = Mathf.Clamp(newID, -1, 9999);

        curGimicObject.SetGimicID(newID);

        if (isSubPreset)
        {
            UpdateGimicData(ref subGimicSetList);
        }
        else
        {
            UpdateGimicData(ref gimicSetList);
        }
    }

    void ResetState()
    {
        curTileID = 0;
        tileFloor = 0;
        tileRot.Set(Vector3.zero);
    }
    
    bool CheckLevelData()
    {
        if (levelData == null)
        {
            Debug.LogWarningFormat("[STAGE EDITOR] Level Data 가 존재하지 않습니다!");
            return false;
        }

        if (levelData.mainPreset == null)
        {
            Debug.LogWarningFormat("[STAGE EDITOR] '{0}' Level Data 의 Main Preset 이 존재하지 않습니다!", levelData.name);
            return false;
        }

        if (levelData.subPreset.Count <= 0)
        {
            Debug.LogWarningFormat("[STAGE EDITOR] '{0}' Level Data 의 Sub Preset 이 1개 이상이라도 존재해야 합니다!", levelData.name);
            return false;
        }
        
        if (levelData.mainPreset.tileSetList == null)
        {
            levelData.mainPreset.tileSetList = new SerializeDictionary<TileID, SubList<TileSet>>();
        }

        if (subSelectIdx > levelData.subPreset.Count - 1)
        {
            subSelectIdx = 0;
        }
        
        if (levelData.subPreset[subSelectIdx].tileSetList == null)
        {
            levelData.subPreset[subSelectIdx].tileSetList = new SerializeDictionary<TileID, SubList<TileSet>>();
        }

        return true;
    }


    
    GameObject GetTilePrefab(bool checkTop = true)
    {
        switch (curTileID)
        {
            case TileID.TILE:
                bool hasTop = checkTop ? CheckTopTile() : false;
                TileFloorID tileFloorID = hasTop ? TileFloorID.BOTTOM_TILE : TileFloorID.TOP_TILE;
                
                if (tileFloor % 1 != 0)
                {
                    tileFloorID = hasTop ? TileFloorID.BOTTOM_HALF_TILE : TileFloorID.TOP_HALF_TILE;
                }

                return themeData.tileFloorPrefabData.GetPrefab(tileFloorID);
            default:
                return curPrefab;
        }
    }

    Vector3 GetPosByOffset()
    {
        return tilePos + curOffset;
    }
    
    Tuple<Vector3, GameObject> FindDuplicateTile()
    {
        return spawnList.Find(x =>
            x.Item1 == tilePos || 
            x.Item1 == tilePos + (Vector3.up * 2.5f) ||
            x.Item1 == tilePos - (Vector3.up * 2.5f));
    }

    Tuple<Vector3, GameObject> FindTile(TileID tileID, Dictionary<TileID, List<TileSet>> tileSetList)
    {
        if (!tileSetList.ContainsKey(tileID) || tileSetList[tileID].Count <= 0)
            return null;

        Vector3 targetPos = tileSetList[tileID][0].spawnPos;

        return spawnList.Find(x => Vector3.Equals(x.Item1, targetPos));
    }
    
    bool CheckTopTile()
    {
        Tuple<Vector3, GameObject> data = spawnList.Find(x =>
            x.Item1 == tilePos + (Vector3.up * halfHeightUnit) ||
            x.Item1 == tilePos + (Vector3.up * tileUnit));

        CheckBottomTile();
        
        if (data == null)
            return false;
        
        EditorTile editorTile;
        if (data.Item2.TryGetComponent(out editorTile))
        {
            if (editorTile.tileID != TileID.TILE)
                return false;
        }

        return true;
    }

    bool CheckBottomTile(bool isDelete = false)
    {
        Tuple<Vector3, GameObject> data = spawnList.Find(x =>
            x.Item1 == tilePos - (Vector3.up * halfHeightUnit) ||
            x.Item1 == tilePos - (Vector3.up * tileUnit));

        if (data == null)
            return false;
        
        EditorTile editorTile;
        if (data.Item2.TryGetComponent(out editorTile))
        {
            if (editorTile.tileID != TileID.TILE)
                return false;
        }
            
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

    bool CheckSaveSafety()
    {
        int tileCount = GameObject.FindGameObjectsWithTag("Tile").Length;
        return (tileCount > 0 && spawnList.Count > 0 && (tileSetList.Count > 0 || subTileSetList.Count > 0));
    }
    
    bool TileCheck()
    {
        bool isAlready = FindDuplicateTile() != null;
        
        GizmoHelper.SetState(isAlready);
        
        return isAlready;
    }
    
    bool IsGimic(TileID tileID)
    {
        return ((int)tileID) >= ((int)TileID.GIMIC_CUSTOM);
    }

    bool IsUnique(TileID tileID)
    {
        if (tileID == TileID.START_POINT || tileID == TileID.VM_POINT)
            return true;

        return false;
    }

    void ReplaceBottomTile(Tuple<Vector3, GameObject> data)
    {
        GameObject tile = data.Item2;
        Vector3 pos = tile.transform.position;
        Vector3 rot  = tile.transform.eulerAngles;
        float floor = 0;
        
        EditorTile editorTile;
        if (tile.TryGetComponent(out editorTile))
        {
            floor = editorTile.floor;

            if (floor % 1 != 0)
            {
                floor = Mathf.Ceil(floor);
                pos.y += 2.5f;
                ReplaceTileSet(editorTile, pos, floor);
            }
        }
        
        spawnList.Remove(data);
        DestroyImmediate(tile);

        GameObject prefab = themeData.tileFloorPrefabData.GetPrefab(TileFloorID.BOTTOM_TILE);
        RespawnTile(prefab, pos, rot, floor);
    }

    void ReplaceTopTile(Tuple<Vector3, GameObject> data)
    {
        GameObject tile = data.Item2;
        Vector3 pos = tile.transform.position;
        Vector3 rot  = tile.transform.eulerAngles;
        float floor = 0;
        
        EditorTile editorTile;
        if (tile.TryGetComponent(out editorTile))
        {
            floor = editorTile.floor;
        }

        spawnList.Remove(data);
        DestroyImmediate(tile);

        GameObject prefab = themeData.tileFloorPrefabData.GetPrefab(floor % 1 != 0 ? TileFloorID.TOP_HALF_TILE : TileFloorID.TOP_TILE);
        RespawnTile(prefab, pos, rot, floor);
    }

    void RespawnTile(GameObject prefab, Vector3 pos, Vector3 rot, float floor)
    {
        GameObject tile = Instantiate(prefab, pos, Quaternion.Euler(rot));
        SetEditorTile(tile, pos, rot, floor);
        
        spawnList.Add(new Tuple<Vector3, GameObject>(pos, tile));
    }

    void ReplaceTileSet(EditorTile editorTile, Vector3 newPos, float newFloor)
    {
        TileSet tileSet;
        if (isSubPreset)
        {
            tileSet = subTileSetList[curTileID].Find(x => x.spawnPos == editorTile.tilePos);
        }
        else
        {
            tileSet = tileSetList[curTileID].Find(x => x.spawnPos == editorTile.tilePos);
        }

        if (tileSet != null)
        {
            tileSet.spawnPos = newPos;
            tileSet.spawnFloor = newFloor;
        }
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
                tileRot.Set(tile.transform.eulerAngles);
                GizmoHelper.SetSize(tileSize);
                GizmoHelper.SetEditorTile(curEditorTile);

                if(curEditorTile.gimicObject != null)
                {
                    curGimicObject = curEditorTile.gimicObject;
                }
                else
                {
                    curGimicObject = null;
                }

                GizmoHelper.SetGimicObject(curGimicObject);
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
        curGimicObject = null;

        tileRot.Set(Vector3.zero);

        GizmoHelper.SetEditorTile(curEditorTile);
        GizmoHelper.SetGimicObject(curGimicObject);
    }

    void RotateTile()
    {
        float y = Mathf.Repeat((tileRot.y + 90), 360);
        tileRot.SetY(y);
    }

    void ActionMultiTile(Action action)
    {
        multiTilePos.x = Mathf.Clamp(multiTilePos.x, 0, 100);
        multiTilePos.y = Mathf.Clamp(multiTilePos.y, 0, 100);
        multiTilePos.z = Mathf.Clamp(multiTilePos.z, 0, 100);

        Vector3 prevPos = tilePos;
        for (int x = 0; x <= multiTilePos.x; x++)
        {
            for (int y = 0; y <= multiTilePos.y; y++)
            {
                for (int z = 0; z <= multiTilePos.z; z++)
                {
                    tilePos = new Vector3(tilePos.x + (tileUnit * x),
                                        tilePos.y + (tileUnit * y),
                                        tilePos.z + (tileUnit * z));
                    action?.Invoke();
                    tilePos = prevPos;
                }
            }
        }
    }

    void CreateMultiTile()
    {
        ActionMultiTile(() => { CreateTile(); });
    }

    void CreateTile(bool isLoad = false)
    {
        if (!TileCheck() && curPrefab != null)
        {
            GameObject tile = SpawnTile();
            EditorTile editorTile = SetEditorTile(tile);
            ClearCollider(tile);

            spawnList.Add(new Tuple<Vector3, GameObject>(tilePos, tile));

            if (IsGimic(curTileID))
            {
                GimicObject gimic = CreateGimic(tile);
                LoadGimic(gimic, editorTile.tilePos);
            }

            if (!isLoad)
            {
                if (IsUnique(curTileID))
                {
                    DeleteDuplicateTile();
                }
                
                AddTileSet();
            }
        }
        
        Save();
    }

    void DeleteMultiTile()
    {
        ActionMultiTile(() => { DeleteTile(); });
    }

    void DeleteTile()
    {
        var data = FindDuplicateTile();
        if (data != null)
        {
            GameObject tile;

            if (isSubPreset)
            {
                tile = RemoveTileSet(data, ref subTileSetList, ref subGimicSetList);                
            }
            else
            {
                tile = RemoveTileSet(data, ref tileSetList, ref gimicSetList);
            }

            if (tile != null)
            {
                DestroyImmediate(tile);
            }
            
            Save();
        }
    }

    GimicObject CreateGimic(GameObject tile)
    {
        GimicObject gimic;
        if (!tile.TryGetComponent(out gimic))
        {
            gimic = tile.AddComponent<GimicObject>();
        }

        gimic.SetGimicID(0);
        
        return gimic;
    }

    void LoadGimic(GimicObject gimic, Vector3 gimicPos)
    {
        GimicSet gimicSet = new GimicSet();
        if (isSubPreset)
        {
            gimicSet = subGimicSetList.Find(x => x.targetPos == gimicPos);
        }
        else
        {
            gimicSet = gimicSetList.Find(x => x.targetPos == gimicPos);
        }

        if (gimicSet != null)
        {
            gimic.SetGimicID(gimicSet.ID);
        }
    }

    void DeleteDuplicateTile()
    {
        var tile = FindTile(curTileID, isSubPreset ? subTileSetList : tileSetList);
        if (tile != null)
        {
            GameObject targetTile = RemoveTileSet(tile);
            if (targetTile != null)
            {
                DestroyImmediate(targetTile);
            }
        }
    }
    
    GameObject SpawnTile()
    {
        GameObject prefab = GetTilePrefab();
        GameObject tile = Instantiate(prefab, GetPosByOffset(), Quaternion.Euler(tileRot));
        tile.name = tile.name.Replace("(Clone)", "").Trim();

        return tile;
    }

    EditorTile SetEditorTile(GameObject tile, Vector3 pos, Vector3 rot, float floor)
    {
        EditorTile editorTile;
        if (!tile.TryGetComponent(out editorTile))
        {
            editorTile = tile.AddComponent<EditorTile>();
        }
        
        editorTile.tileID = curTileID;
        editorTile.tilePos = pos;
        editorTile.spawnRot = rot;
        editorTile.floor = floor;

        return editorTile;
    }
    
    EditorTile SetEditorTile(GameObject tile)
    {
        EditorTile editorTile;
        if (!tile.TryGetComponent(out editorTile))
        {
            editorTile = tile.AddComponent<EditorTile>();
        }
        
        editorTile.tileID = curTileID;
        editorTile.tilePos = tilePos;
        editorTile.spawnRot = tileRot;
        editorTile.floor = tileFloor;

        return editorTile;
    }
    
    void ClearCollider(GameObject tile)
    {
        Collider collider;
        if (tile.TryGetComponent(out collider))
        {
            DestroyImmediate(collider);
        }

        Collider[] colliders = tile.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            DestroyImmediate(col);
        }
    }
    
    void ClearSpawnTile()
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
    
    void SpawnTileSet(Dictionary<TileID, List<TileSet>> tileSetList)
    {
        foreach (var key in tileSetList.Keys)
        {
            foreach (var tileSet in tileSetList[key])
            {
                curTileID = key;
                tileFloor = tileSet.spawnFloor;
                tilePos.Set(tileSet.spawnPos);
                tileRot.Set(tileSet.spawnRot);

                UpdateData();

                CreateTile(true);
            }
        }
    }

    void AddTileSet()
    {
        if (isSubPreset)
        {
            AddTileSet(ref subTileSetList, ref subGimicSetList);
        }
        else
        {
            AddTileSet(ref tileSetList, ref gimicSetList);
        }
    }
    
    void AddTileSet(ref Dictionary<TileID, List<TileSet>> tileSetList, ref List<GimicSet> gimicSetList)
    {
        if (!tileSetList.ContainsKey(curTileID))
        {
            tileSetList.Add(curTileID, new SubList<TileSet>());
        }

        TileSet tileSet = new TileSet();
        tileSet.spawnPos = tilePos;
        tileSet.spawnRot = tileRot;
        tileSet.spawnFloor = tileFloor;
        
        tileSetList[curTileID].Add(tileSet);

        if (IsGimic(curTileID))
        {
            GimicSet gimicSet = new GimicSet();
            gimicSet.targetPos = tilePos;
            gimicSet.ID = 0;

            gimicSetList.Add(gimicSet);
        }
    }

    GameObject RemoveTileSet(Tuple<Vector3, GameObject> targetTile)
    {
        GameObject tile;
        if (isSubPreset)
        {
            tile = RemoveTileSet(targetTile, ref subTileSetList, ref subGimicSetList);
        }
        else
        {
            tile = RemoveTileSet(targetTile, ref tileSetList, ref gimicSetList);
        }

        return tile;
    }
    
    GameObject RemoveTileSet(Tuple<Vector3, GameObject> targetTile, ref Dictionary<TileID, List<TileSet>> tileSetList, ref List<GimicSet> gimicSetList)
    {
        foreach (var key in tileSetList.Keys)
        {
            TileSet tileSet = tileSetList[key].Find(x => Vector3.Equals(x.spawnPos, targetTile.Item1));
            if (tileSetList[key].Remove(tileSet))
            {
                spawnList.Remove(targetTile);
                
                GimicSet gimicSet = gimicSetList.Find(x => x.targetPos == targetTile.Item1);
                gimicSetList.Remove(gimicSet);
                
                if (key == TileID.TILE)
                {
                    CheckBottomTile(true);
                }

                return targetTile.Item2;
            }
        }

        return null;
    }

    void LoadTileSet(TileSetData tileSetData, ref Dictionary<TileID, List<TileSet>> tileSetList, ref List<GimicSet> gimicSetList)
    {
        ClearTileSet(ref tileSetList, ref gimicSetList);

        foreach (var key in tileSetData.tileSetList.Keys)
        {
            foreach (var tileSet in tileSetData.tileSetList[key])
            {
                if (!tileSetList.ContainsKey(key))
                {
                    tileSetList.Add(key, new SubList<TileSet>());
                }
            
                tileSetList[key].Add(new TileSet(tileSet));
            }
        }
        
        foreach (var gimicSet in tileSetData.gimicSetList.OrderBy(x => x.ID))
        {
            gimicSetList.Add(new GimicSet(gimicSet));
        }
    }
    
    void SaveTileSet(TileSetData tileSetData, Dictionary<TileID, List<TileSet>> tileSetList, List<GimicSet> gimicSetList)
    {
        if(!CheckSaveSafety())
        {
            Debug.LogWarningFormat("[STAGE EDITOR] Save Safety!\n" +
                "Scene 에 배치된 타일의 수와 Spawn List 의 수가 일치하지 않습니다!\n" +
                "Spawn Count : {0}\nTileSet Count : {1}\nGimicSet Count : {2}", 
                spawnList.Count, tileSetList.Count, gimicSetList.Count);
            return;
        }

        ClearTileSet(ref tileSetData);

        if (tileSetList.Count > 0)
        {
            foreach (var key in tileSetList.Keys)
            {
                foreach (var tileSet in tileSetList[key].OrderByDescending(x => x.spawnFloor))
                {
                    if (!tileSetData.tileSetList.ContainsKey(key))
                    {
                        tileSetData.tileSetList.Add(key, new SubList<TileSet>());
                    }
            
                    tileSetData.tileSetList[key].Add(new TileSet(tileSet));
                }
            }

            foreach (var gimicSet in gimicSetList.OrderBy(x => x.ID))
            {
                tileSetData.gimicSetList.Add(new GimicSet(gimicSet));
            }
        }

        EditorUtility.SetDirty(tileSetData);
        Save();
    }

    void ClearTileSet(ref TileSetData tileSetData)
    {
        if (tileSetData.tileSetList.Count > 0)
        {
            foreach (var key in tileSetData.tileSetList.Keys)
            {
                tileSetData.tileSetList[key].ClearData();
            }
        }

        tileSetData.tileSetList.Clear();
        tileSetData.gimicSetList.Clear();
    }

    void ClearTileSet(ref Dictionary<TileID, List<TileSet>> tileSetList, ref List<GimicSet> gimicSetList)
    {
        if (tileSetList.Count > 0)
        {
            foreach (var key in tileSetList.Keys)
            {
                tileSetList[key].Clear();
            }
        }

        tileSetList.Clear();
        gimicSetList.Clear();
    }

    void Save()
    {
        this.SaveChanges();
        
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
