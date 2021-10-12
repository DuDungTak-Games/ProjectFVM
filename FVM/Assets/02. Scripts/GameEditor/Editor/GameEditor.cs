using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameEditor : EditorWindow
{

    [MenuItem("DudungtakGames/Game Editor")]
    static public void ShowWindow()
    {
        MENU_TITLE = "Select Editor";
        MENU_TYPE = menuType.NONE;

        GameEditor window = (GameEditor)EditorWindow.GetWindow(typeof(GameEditor));
        GUIContent guiContent = new GUIContent(GUIContent.none);
        guiContent.text = "Game Editor";

        window.titleContent = guiContent;
    }

    [MenuItem("GameData/Player Data Clear")]
    static public void ClearPlayerData()
    {
        // TODO : �� ó�� ���� �ʿ� (Player & Game Data �ʱ�ȭ)
    }

    [MenuItem("GameData/PlayerPrefs Data Clear")]
    static public void ClearPlayerPrefsData()
    {
        PlayerPrefs.DeleteAll();
    }



    public enum menuType { NONE, INFO, TILE_PREFAB, TILE_FLOOR_PREFAB, LEVEL }
    static menuType MENU_TYPE;
    static string MENU_TITLE;

    static List<EditorContent> contentsList = new List<EditorContent>();
    static EditorContent currentContent;

    Vector2 scrollPosition_LIST = Vector2.zero;
    Vector2 scrollPosition_CONTENT = Vector2.zero;

    void OnEnable()
    {
        if (contentsList.Count <= 0)
        {
            InitContentList();
        }
    }
    
    void OnGUI()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            // Draw Button List
            scrollPosition_LIST = EditorGUILayout.BeginScrollView(scrollPosition_LIST, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.MaxWidth(180));
            DrawButtonList();
            EditorGUILayout.EndScrollView();

            DrawVerticalLine();

            using(new EditorGUILayout.VerticalScope())
            {
                // Draw Content
                EditorGUILayout.LabelField(MENU_TITLE, GUIStyle_Title_CONTENT(), LayoutOption_Title_CONTENT());
                DrawHorizontalLine();
                EditorGUILayout.Space(6);

                scrollPosition_CONTENT = EditorGUILayout.BeginScrollView(scrollPosition_CONTENT);
                DrawContent();
                EditorGUILayout.EndScrollView();
            }
        }

        Repaint();
    }
    
        void SetMenu(menuType menuType)
    {
        MENU_TYPE = menuType;
    }

    void DrawContent()
    {
        if (contentsList.Count <= 0)
            return;

        if (currentContent == null || currentContent.MENU_TYPE != MENU_TYPE)
        {
            currentContent = GetContent(MENU_TYPE);
        }
        else if (currentContent.MENU_TYPE == MENU_TYPE)
        {
            currentContent.DrawContent();
        }
    }

    void InitContentList()
    {
        contentsList = new List<EditorContent>();
        contentsList.Add(new GameInfo());
        contentsList.Add(new TilePrefabEditor());
        contentsList.Add(new TileFloorPrefabEditor());
        contentsList.Add(new LevelEditor());

        foreach(EditorContent editorContent in contentsList)
        {
            editorContent.Init();
        }
    }

    EditorContent GetContent(menuType targetMenuType)
    {
        return contentsList.Find((x) => x.MENU_TYPE == targetMenuType);
    }

    void DrawButtonList()
    {
        if (contentsList.Count <= 0)
            return;

        EditorGUILayout.LabelField("Game Editor Panel", GUIStyle_Title_LIST(), LayoutOption_Title_LIST());

        foreach(EditorContent editorContent in contentsList)
        {
            if (GUILayout.Button(editorContent.BUTTON_TEXT, GUIStyle_Text_LIST(), LayoutOption_Text_LIST()))
                SetMenu(editorContent.MENU_TYPE);
        }
    }

    void DrawHorizontalLine() => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MaxHeight(1));
    void DrawVerticalLine() => EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.MaxWidth(1), GUILayout.Height(position.height));

    #region GUILayoutOption_LIST
    GUILayoutOption[] LayoutOption_Title_LIST()
    {
        List<GUILayoutOption> layoutOptions = new List<GUILayoutOption>();
        layoutOptions.Add(GUILayout.MaxWidth(180));
        layoutOptions.Add(GUILayout.MaxHeight(40));

        return layoutOptions.ToArray();
    }

    GUILayoutOption[] LayoutOption_Text_LIST()
    {
        List<GUILayoutOption> layoutOptions = new List<GUILayoutOption>();
        layoutOptions.Add(GUILayout.MaxWidth(180));

        return layoutOptions.ToArray();
    }
#endregion

    #region GUIStyle_LIST
    GUIStyle GUIStyle_Title_LIST()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 16;

        return style;
    }

    GUIStyle GUIStyle_Text_LIST()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 12;

        return style;
    }
    #endregion

    #region GUILayoutOption_CONTENT
    GUILayoutOption[] LayoutOption_Title_CONTENT()
    {
        List<GUILayoutOption> layoutOptions = new List<GUILayoutOption>();
        layoutOptions.Add(GUILayout.MinHeight(35));

        return layoutOptions.ToArray();
    }
    #endregion

    #region GUIStyle_CONTENT
    GUIStyle GUIStyle_Title_CONTENT()
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.LowerCenter;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 26;

        return style;
    }
    #endregion



    public abstract class EditorContent
    {
        public menuType MENU_TYPE;
        public string BUTTON_TEXT { get; protected set; }

        public abstract void Init();
        public abstract void DrawContent();
        public virtual void Update() { }

        protected void Save()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private class GameInfo : EditorContent
    {
        public override void Init()
        {
            MENU_TYPE = menuType.INFO;
            BUTTON_TEXT = "Game Info";
        }

        public override void DrawContent()
        {
            MENU_TITLE = "Game Info";

            string projectName = string.Format("������Ʈ : ProjectFVM");
            string productName = string.Format("���� �̸� : {0}", Application.productName);
            string gameVersion = string.Format("���� ���� : {0}", Application.version);
            string companyName = string.Format("���� �� : {0}", Application.companyName);
            string unityVersion = string.Format("����Ƽ ���� : {0}", Application.unityVersion);

            using (new EditorGUILayout.VerticalScope("HelpBox"))
            {
                EditorGUILayout.LabelField(projectName, GUIStyle_Text(), LayoutOption_Text());
                EditorGUILayout.LabelField(productName, GUIStyle_Text(), LayoutOption_Text());
                EditorGUILayout.LabelField(gameVersion, GUIStyle_Text(), LayoutOption_Text());
                EditorGUILayout.LabelField(companyName, GUIStyle_Text(), LayoutOption_Text());
                EditorGUILayout.LabelField(unityVersion, GUIStyle_Text(), LayoutOption_Text());
            }
        }

        #region GUI
        public GUILayoutOption[] LayoutOption_Text()
        {
            List<GUILayoutOption> layoutOptions = new List<GUILayoutOption>();
            layoutOptions.Add(GUILayout.MinHeight(40));

            return layoutOptions.ToArray();
        }

        public GUIStyle GUIStyle_Text()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 20;

            return style;
        }
        #endregion
    }

    private class TilePrefabEditor : EditorContent
    {

        TilePrefabData selectData;
        TilePrefabData[] datas;

        string serachValue_LIST = string.Empty;

        string selectDataKey = "TileID";
        string selectDataValue = "Prefab / Floor Unit";

        Vector2 scrollPosition_LIST = Vector2.zero;
        Vector2 scrollPosition_DATA = Vector2.zero;

        TileID[] keys;

        public override void Init()
        {
            MENU_TYPE = menuType.TILE_PREFAB;
            BUTTON_TEXT = "Tile Prefab Editor";

            Update();
        }

        public override void DrawContent()
        {
            MENU_TITLE = "Tile Prefab Editor";

            using (new EditorGUILayout.HorizontalScope("GroupBox", GUILayout.ExpandWidth(true)))
            {
                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.MaxWidth(260), GUILayout.ExpandHeight(true)))
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        EditorGUILayout.LabelField("Serach", GUILayout.MaxWidth(45));
                        serachValue_LIST = EditorGUILayout.TextField(serachValue_LIST);

                        if (GUILayout.Button("Refresh", GUILayout.MaxWidth(80)))
                        {
                            serachValue_LIST = string.Empty;
                            Update();
                        }
                    }

                    EditorGUILayout.Space(6);

                    scrollPosition_LIST = EditorGUILayout.BeginScrollView(scrollPosition_LIST);

                    bool isSerach = serachValue_LIST != string.Empty;

                    foreach (TilePrefabData data in datas)
                    {
                        if (data == null)
                            break;
                        
                        bool isKeyMatch = serachValue_LIST.SpecialContains(data.name);

                        if ((isSerach && isKeyMatch) || !isSerach)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                if (GUILayout.Button(data.name, selectData == data ? GUIStyle_SelectedButton() : GUIStyle_SelectButton()))
                                {
                                    selectData = data;
                                    Update();
                                }
                            }
                        }
                    }

                    EditorGUILayout.EndScrollView();
                }

                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    EditorGUILayout.Space(6);

                    using (new EditorGUILayout.HorizontalScope(GUIStyle_HelpBox(), GUILayout.ExpandWidth(true), GUILayout.MaxHeight(10)))
                    {
                        EditorGUILayout.LabelField(selectDataKey, GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(320));
                        EditorGUILayout.LabelField(selectDataValue, GUIStyle_KeyValueTitle_Text());

                        EditorGUILayout.LabelField("Action", GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(120));
                    }

                    scrollPosition_DATA = EditorGUILayout.BeginScrollView(scrollPosition_DATA);

                    if (selectData != null)
                    {
                        if (selectData.prefabList.Count > 0)
                        {
                            keys = selectData.prefabList.Keys.ToArray();

                            foreach (TileID key in keys)
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUI.BeginChangeCheck();

                                    EditorGUILayout.LabelField(key.ToString(), GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(320));
                                    selectData.prefabList[key] = (GameObject)EditorGUILayout.ObjectField(selectData.prefabList[key], typeof(GameObject), false);
                                    selectData.floorUnitList[key] = EditorGUILayout.FloatField(selectData.floorUnitList[key]);

                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        EditorUtility.SetDirty(selectData);
                                        Save();
                                    }

                                    if (GUILayout.Button("Reset", GUILayout.MaxWidth(120)))
                                    {
                                        selectData.prefabList[key] = null;
                                        selectData.floorUnitList[key] = 0;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space(6);

                        EditorGUILayout.LabelField("���õ� �����Ͱ� �����!!", GUIStyle_KeyValueTitle_Text());
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
        }

        public override void Update()
        {
            datas = Resources.LoadAll<TilePrefabData>("GameData/TilePrefab");

            if (selectData != null)
            {
                if (selectData.prefabList.Count <= 0)
                {
                    foreach (TileID tileID in Enum.GetValues(typeof(TileID)))
                    {
                        if (!selectData.prefabList.ContainsKey(tileID))
                        {
                            selectData.prefabList.Add(tileID, null);
                        }
                        
                        if (!selectData.floorUnitList.ContainsKey(tileID))
                        {
                            selectData.floorUnitList.Add(tileID, 0);
                        }
                    }
                    
                    keys = selectData.prefabList.Keys.ToArray();
                }
            }
        }

        #region GUI
        public GUIStyle GUIStyle_HelpBox()
        {
            GUIStyle style = new GUIStyle("HelpBox");
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(0, 0, 0, 0);

            return style;
        }

        public GUIStyle GUIStyle_KeyValueTitle_Text()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.LowerCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 14;

            return style;
        }

        public GUIStyle GUIStyle_SelectButton()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 2, 2);
            style.padding = new RectOffset(0, 0, 5, 5);

            return style;
        }

        public GUIStyle GUIStyle_SelectedButton()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 10, 10);
            style.padding = new RectOffset(0, 0, 5, 5);

            style.normal.textColor = Color.green;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 20;

            return style;
        }
        #endregion
    }
    
    private class TileFloorPrefabEditor : EditorContent
    {

        TileFloorPrefabData selectData;
        TileFloorPrefabData[] datas;

        string serachValue_LIST = string.Empty;

        string selectDataKey = "TileFloorID";
        string selectDataValue = "Prefab";

        Vector2 scrollPosition_LIST = Vector2.zero;
        Vector2 scrollPosition_DATA = Vector2.zero;

        TileFloorID[] keys;

        public override void Init()
        {
            MENU_TYPE = menuType.TILE_FLOOR_PREFAB;
            BUTTON_TEXT = "Tile Floor Prefab Editor";

            Update();
        }

        public override void DrawContent()
        {
            MENU_TITLE = "Tile Floor Prefab Editor";

            using (new EditorGUILayout.HorizontalScope("GroupBox", GUILayout.ExpandWidth(true)))
            {
                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.MaxWidth(260), GUILayout.ExpandHeight(true)))
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        EditorGUILayout.LabelField("Serach", GUILayout.MaxWidth(45));
                        serachValue_LIST = EditorGUILayout.TextField(serachValue_LIST);

                        if (GUILayout.Button("Refresh", GUILayout.MaxWidth(80)))
                        {
                            serachValue_LIST = string.Empty;
                            Update();
                        }
                    }

                    EditorGUILayout.Space(6);

                    scrollPosition_LIST = EditorGUILayout.BeginScrollView(scrollPosition_LIST);

                    bool isSerach = serachValue_LIST != string.Empty;

                    foreach (TileFloorPrefabData data in datas)
                    {
                        if (data == null)
                            break;
                        
                        bool isKeyMatch = serachValue_LIST.SpecialContains(data.name);

                        if ((isSerach && isKeyMatch) || !isSerach)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                if (GUILayout.Button(data.name, selectData == data ? GUIStyle_SelectedButton() : GUIStyle_SelectButton()))
                                {
                                    selectData = data;
                                    Update();
                                }
                            }
                        }
                    }

                    EditorGUILayout.EndScrollView();
                }

                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    EditorGUILayout.Space(6);

                    using (new EditorGUILayout.HorizontalScope(GUIStyle_HelpBox(), GUILayout.ExpandWidth(true), GUILayout.MaxHeight(10)))
                    {
                        EditorGUILayout.LabelField(selectDataKey, GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(320));
                        EditorGUILayout.LabelField(selectDataValue, GUIStyle_KeyValueTitle_Text());

                        EditorGUILayout.LabelField("Action", GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(120));
                    }

                    scrollPosition_DATA = EditorGUILayout.BeginScrollView(scrollPosition_DATA);

                    if (selectData != null)
                    {
                        if (selectData.prefabList.Count > 0)
                        {
                            keys = selectData.prefabList.Keys.ToArray();

                            foreach (TileFloorID key in keys)
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUI.BeginChangeCheck();

                                    EditorGUILayout.LabelField(key.ToString(), GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(320));
                                    selectData.prefabList[key] = (GameObject)EditorGUILayout.ObjectField(selectData.prefabList[key], typeof(GameObject), false);

                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        EditorUtility.SetDirty(selectData);
                                        Save();
                                    }

                                    if (GUILayout.Button("Reset", GUILayout.MaxWidth(120)))
                                    {
                                        selectData.prefabList[key] = null;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space(6);

                        EditorGUILayout.LabelField("���õ� �����Ͱ� �����!!", GUIStyle_KeyValueTitle_Text());
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
        }

        public override void Update()
        {
            datas = Resources.LoadAll<TileFloorPrefabData>("GameData/TileFloorPrefab");

            if (selectData != null)
            {
                if (selectData.prefabList.Count <= 0)
                {
                    foreach (TileFloorID tileID in Enum.GetValues(typeof(TileFloorID)))
                    {
                        if (!selectData.prefabList.ContainsKey(tileID))
                        {
                            selectData.prefabList.Add(tileID, null);
                        }
                    }
                    
                    keys = selectData.prefabList.Keys.ToArray();
                }
            }
        }

        #region GUI
        public GUIStyle GUIStyle_HelpBox()
        {
            GUIStyle style = new GUIStyle("HelpBox");
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(0, 0, 0, 0);

            return style;
        }

        public GUIStyle GUIStyle_KeyValueTitle_Text()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.LowerCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 14;

            return style;
        }

        public GUIStyle GUIStyle_SelectButton()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 2, 2);
            style.padding = new RectOffset(0, 0, 5, 5);

            return style;
        }

        public GUIStyle GUIStyle_SelectedButton()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 10, 10);
            style.padding = new RectOffset(0, 0, 5, 5);

            style.normal.textColor = Color.green;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 20;

            return style;
        }
        #endregion
    }
    
    private class LevelEditor : EditorContent
    {

        LevelData selectData;
        LevelData[] datas;

        string serachValue_LIST = string.Empty;

        string selectDataKey = "Preset";
        string selectDataValue = "Data";

        Vector2 scrollPosition_LIST = Vector2.zero;
        Vector2 scrollPosition_DATA = Vector2.zero;

        public override void Init()
        {
            MENU_TYPE = menuType.LEVEL;
            BUTTON_TEXT = "Level Editor";

            Update();
        }

        public override void DrawContent()
        {
            MENU_TITLE = "Level Editor";

            using (new EditorGUILayout.HorizontalScope("GroupBox", GUILayout.ExpandWidth(true)))
            {
                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.MaxWidth(260), GUILayout.ExpandHeight(true)))
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        EditorGUILayout.LabelField("Serach", GUILayout.MaxWidth(45));
                        serachValue_LIST = EditorGUILayout.TextField(serachValue_LIST);

                        if (GUILayout.Button("Refresh", GUILayout.MaxWidth(80)))
                        {
                            serachValue_LIST = string.Empty;
                            Update();
                        }
                    }

                    EditorGUILayout.Space(6);

                    scrollPosition_LIST = EditorGUILayout.BeginScrollView(scrollPosition_LIST);

                    bool isSerach = serachValue_LIST != string.Empty;

                    foreach (LevelData data in datas)
                    {
                        if (data == null)
                            break;
                        
                        bool isKeyMatch = serachValue_LIST.SpecialContains(data.name);

                        if ((isSerach && isKeyMatch) || !isSerach)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                if (GUILayout.Button(data.name, selectData == data ? GUIStyle_SelectedButton() : GUIStyle_SelectButton()))
                                {
                                    selectData = data;
                                    Update();
                                }
                            }
                        }
                    }
                    
                    if (GUILayout.Button("Add (Empty)", GUIStyle_SelectedButton()))
                    {
                        LevelData levelData = CreateInstance<LevelData>();
                        AssetDatabase.CreateAsset(levelData, string.Format("Assets/Resources/GameData/Level/Level_{0}_{1}.asset", 
                            datas.Length, UnityEngine.Random.Range(0, 999999)));
                        
                        Update();
                    }

                    EditorGUILayout.EndScrollView();
                }

                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    EditorGUILayout.Space(6);

                    using (new EditorGUILayout.HorizontalScope(GUIStyle_HelpBox(), GUILayout.ExpandWidth(true), GUILayout.MaxHeight(10)))
                    {
                        EditorGUILayout.LabelField(selectDataKey, GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(320));
                        EditorGUILayout.LabelField(selectDataValue, GUIStyle_KeyValueTitle_Text());

                        EditorGUILayout.LabelField("Action", GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(120));
                    }

                    scrollPosition_DATA = EditorGUILayout.BeginScrollView(scrollPosition_DATA);

                    if (selectData != null)
                    {
                        EditorGUI.BeginChangeCheck();

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Tile Prefab Data", GUIStyle_KeyValueTitle_Text(),
                                GUILayout.MaxWidth(320));
                            selectData.tilePrefabData =
                                (TilePrefabData) EditorGUILayout.ObjectField(selectData.tilePrefabData,
                                    typeof(TilePrefabData), false);

                            if (GUILayout.Button("Reset", GUILayout.MaxWidth(120)))
                            {
                                selectData.tilePrefabData = null;
                            }
                        }
                        
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Tile Floor Prefab Data", GUIStyle_KeyValueTitle_Text(),
                                GUILayout.MaxWidth(320));
                            selectData.tileFloorPrefabData =
                                (TileFloorPrefabData) EditorGUILayout.ObjectField(selectData.tileFloorPrefabData,
                                    typeof(TileFloorPrefabData), false);

                            if (GUILayout.Button("Reset", GUILayout.MaxWidth(120)))
                            {
                                selectData.tileFloorPrefabData = null;
                            }
                        }
                        
                        EditorGUILayout.Space(20);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Main Preset", GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(320));
                            selectData.mainPreset = 
                                (TileSetData)EditorGUILayout.ObjectField(selectData.mainPreset, 
                                    typeof(TileSetData), false);

                            if (selectData.mainPreset != null)
                            {
                                if (GUILayout.Button("Reset", GUILayout.MaxWidth(120)))
                                {
                                    selectData.mainPreset = null;
                                }
                                
                                if (GUILayout.Button("Delete", GUILayout.MaxWidth(120)))
                                {
                                    string path = AssetDatabase.GetAssetPath(selectData.mainPreset);
                                    if (path.Length > 0)
                                    {
                                        AssetDatabase.DeleteAsset(path);
                                    }
                                            
                                    selectData.mainPreset = null;
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Create"))
                                {
                                    TileSetData tileSetData = CreateInstance<TileSetData>();
                                    AssetDatabase.CreateAsset(tileSetData, string.Format("Assets/Resources/GameData/TileSet/{0}_MainTileSet.asset", selectData.name));
                                    
                                    selectData.mainPreset = tileSetData;
                                }
                            }
                        }
                        
                        EditorGUILayout.Space(20);

                        using (new EditorGUILayout.VerticalScope())
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("Sub Preset", GUIStyle_KeyValueTitle_Text(),
                                    GUILayout.MaxWidth(320));

                                if (selectData.subPreset == null)
                                {
                                    selectData.subPreset = new List<TileSetData>();
                                }
                                
                                if (selectData.subPreset.Count <= 0)
                                {
                                    EditorGUILayout.LabelField("�����͸� �߰����ּ���!!", GUIStyle_KeyValueTitle_Text());
                                }
                            }

                            if (selectData.subPreset.Count > 0)
                            {
                                for(int i = 0; i < selectData.subPreset.Count; i++)
                                {
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        GUILayout.FlexibleSpace();
                                        selectData.subPreset[i] = (TileSetData)EditorGUILayout.ObjectField(selectData.subPreset[i], typeof(TileSetData), false);
 
                                        if (GUILayout.Button("Reset", GUILayout.MaxWidth(110)))
                                        {
                                            selectData.subPreset[i] = null;
                                            break;
                                        }
                                    
                                        if (GUILayout.Button("Remove", GUILayout.MaxWidth(110)))
                                        {
                                            selectData.subPreset.RemoveAt(i);
                                            break;
                                        }
                                        
                                        if (GUILayout.Button("Delete", GUILayout.MaxWidth(110)))
                                        {
                                            string path = AssetDatabase.GetAssetPath(selectData.subPreset[i]);
                                            if (path.Length > 0)
                                            {
                                                AssetDatabase.DeleteAsset(path);
                                            }
                                            
                                            selectData.subPreset.RemoveAt(i);
                                            break;
                                        }
                                    }
                                }
                            }

                            using (new EditorGUILayout.HorizontalScope())
                            {
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button("Add (Empty)", GUILayout.MaxWidth(270)))
                                {
                                    selectData.subPreset.Add(null);
                                }
                                
                                if (GUILayout.Button("Add (Create)", GUILayout.MaxWidth(270)))
                                {
                                    TileSetData tileSetData = CreateInstance<TileSetData>();
                                    AssetDatabase.CreateAsset(tileSetData, string.Format("Assets/Resources/GameData/TileSet/{0}_SubTileSet{1}.asset", 
                                        selectData.name, selectData.subPreset.Count));
                                    
                                    selectData.subPreset.Add(tileSetData);
                                }
                            }
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(selectData);
                            Save();
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space(6);

                        EditorGUILayout.LabelField("���õ� �����Ͱ� �����!!", GUIStyle_KeyValueTitle_Text());
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
        }

        public override void Update()
        {
            datas = Resources.LoadAll<LevelData>("GameData/Level");
        }

        #region GUI
        public GUIStyle GUIStyle_HelpBox()
        {
            GUIStyle style = new GUIStyle("HelpBox");
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(0, 0, 0, 0);

            return style;
        }

        public GUIStyle GUIStyle_KeyValueTitle_Text()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.LowerCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 14;

            return style;
        }

        public GUIStyle GUIStyle_SelectButton()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 2, 2);
            style.padding = new RectOffset(0, 0, 5, 5);

            return style;
        }

        public GUIStyle GUIStyle_SelectedButton()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 10, 10);
            style.padding = new RectOffset(0, 0, 5, 5);

            style.normal.textColor = Color.green;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 20;

            return style;
        }
        #endregion
    }
}