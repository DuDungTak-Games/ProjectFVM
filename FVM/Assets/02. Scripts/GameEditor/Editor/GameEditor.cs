using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameEditor : EditorWindow
{

    [MenuItem("Custom Editor/Game Editor")]
    static public void ShowWindow()
    {
        CUR_MENU_TITLE = "Select Editor";
        CUR_MENU_TYPE = menuType.NONE;

        GameEditor window = (GameEditor)EditorWindow.GetWindow(typeof(GameEditor));
        GUIContent guiContent = new GUIContent(GUIContent.none);
        guiContent.text = "Game Editor";

        window.titleContent = guiContent;
    }

    [MenuItem("Custom Data/Player Data Clear")]
    static public void ClearPlayerData()
    {
        Debug.Log("TODO : 구현 필요");
    }

    [MenuItem("Custom Data/PlayerPrefs Data Clear")]
    static public void ClearPlayerPrefsData()
    {
        PlayerPrefs.DeleteAll();
    }



    public enum menuType 
    { 
        NONE, 
        INFO, 
        LEVEL, 
        THEME, 
        TILE_PREFAB, 
        TILE_FLOOR_PREFAB,
        // PLAYER_DATA,
    }

    static string CUR_MENU_TITLE;
    static menuType CUR_MENU_TYPE;

    static List<EditorContent> contentList = new List<EditorContent>();
    static EditorContent curContent;

    Vector2 scrollPosition_LIST = Vector2.zero;
    Vector2 scrollPosition_CONTENT = Vector2.zero;

    void OnEnable()
    {
        if (contentList.Count <= 0)
        {
            InitContentList();
        }
    }
    
    void OnGUI()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            // MENU BUTTON LIST
            scrollPosition_LIST = EditorGUILayout.BeginScrollView(scrollPosition_LIST, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.MaxWidth(180));
            DrawButtonList();
            EditorGUILayout.EndScrollView();

            DrawVerticalLine();

            using(new EditorGUILayout.VerticalScope())
            {
                // MENU TITLE TEXT
                EditorGUILayout.LabelField(CUR_MENU_TITLE, GUIStyle_Title_CONTENT(), LayoutOption_Title_CONTENT());
                DrawHorizontalLine();
                EditorGUILayout.Space(6);

                scrollPosition_CONTENT = EditorGUILayout.BeginScrollView(scrollPosition_CONTENT);
                DrawContent();
                EditorGUILayout.EndScrollView();
            }
        }

        Repaint();
    }

    void DrawContent()
    {
        if (contentList.Count > 0)
        {
            if(curContent == null || curContent.MENU_TYPE != CUR_MENU_TYPE)
            {
                curContent = GetContent(CUR_MENU_TYPE);
                return;
            }

            curContent.DrawContent();
        }
    }

    void DrawButtonList()
    {
        EditorGUILayout.LabelField("Game Editor Panel", GUIStyle_Title_LIST(), LayoutOption_Title_LIST());

        if (contentList.Count > 0)
        {
            foreach (EditorContent content in contentList)
            {
                if (GUILayout.Button(content.BUTTON_TEXT, GUIStyle_Text_LIST(), LayoutOption_Text_LIST()))
                {
                    CUR_MENU_TITLE = content.BUTTON_TEXT;

                    SetMenu(content.MENU_TYPE);
                }
            }
        }
    }

    void DrawHorizontalLine() => EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MaxHeight(1));
    void DrawVerticalLine() => EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.MaxWidth(1), GUILayout.Height(position.height));

    void InitContentList()
    {
        contentList = new List<EditorContent>();
        contentList.Add(new GameInfo());
        contentList.Add(new LevelEditor());
        contentList.Add(new ThemeEditor());
        contentList.Add(new TilePrefabEditor());
        contentList.Add(new TileFloorPrefabEditor());

        foreach(EditorContent content in contentList)
        {
            content.Init();
        }
    }

    void SetMenu(menuType menuType)
    {
        CUR_MENU_TYPE = menuType;
    }

    EditorContent GetContent(menuType menuType)
    {
        return contentList.Find((x) => x.MENU_TYPE == menuType);
    }

    #region GUI_Style
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

    GUILayoutOption[] LayoutOption_Title_CONTENT()
    {
        List<GUILayoutOption> layoutOptions = new List<GUILayoutOption>();
        layoutOptions.Add(GUILayout.MinHeight(35));

        return layoutOptions.ToArray();
    }

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
            string projectName = string.Format("프로젝트 : ProjectFVM", Application.productName);
            string productName = string.Format("게임 이름 : {0}", Application.productName);
            string gameVersion = string.Format("게임 버전 : {0}", Application.version);
            string companyName = string.Format("개발 팀 : {0}", Application.companyName);
            string unityVersion = string.Format("유니티 버전 : {0}", Application.unityVersion);

            using (new EditorGUILayout.VerticalScope("HelpBox"))
            {
                EditorGUILayout.LabelField(projectName, GUIStyle_Text(), LayoutOption_Text());
                EditorGUILayout.LabelField(productName, GUIStyle_Text(), LayoutOption_Text());
                EditorGUILayout.LabelField(gameVersion, GUIStyle_Text(), LayoutOption_Text());
                EditorGUILayout.LabelField(companyName, GUIStyle_Text(), LayoutOption_Text());
                EditorGUILayout.LabelField(unityVersion, GUIStyle_Text(), LayoutOption_Text());
            }
        }

        #region GUI_Style
        GUILayoutOption[] LayoutOption_Text()
        {
            List<GUILayoutOption> layoutOptions = new List<GUILayoutOption>();
            layoutOptions.Add(GUILayout.MinHeight(40));

            return layoutOptions.ToArray();
        }

        GUIStyle GUIStyle_Text()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
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

        string serachValue = string.Empty;

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
            using (new EditorGUILayout.HorizontalScope("GroupBox", GUILayout.ExpandWidth(true)))
            {
                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.MaxWidth(260), GUILayout.ExpandHeight(true)))
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        EditorGUILayout.LabelField("Serach", GUILayout.MaxWidth(45));
                        serachValue = EditorGUILayout.TextField(serachValue);

                        if (GUILayout.Button("Refresh", GUILayout.MaxWidth(80)))
                        {
                            serachValue = string.Empty;
                            Update();
                        }
                    }

                    EditorGUILayout.Space(6);

                    scrollPosition_LIST = EditorGUILayout.BeginScrollView(scrollPosition_LIST);

                    bool isSerach = serachValue != string.Empty;

                    foreach (LevelData data in datas)
                    {
                        if (data == null)
                            break;
                        
                        bool isKeyMatch = data.name.SpecialContains(serachValue);

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
                    
                    if (GUILayout.Button("Add Level", GUIStyle_SelectedButton()))
                    {
                        LevelData levelData = CreateInstance<LevelData>();
                        AssetDatabase.CreateAsset(levelData, string.Format("Assets/Resources/GameData/Level/Level_{0:0000}.asset", datas.Length));
                        
                        Update();
                    }

                    EditorGUILayout.EndScrollView();
                }

                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    EditorGUILayout.Space(6);

                    using (new EditorGUILayout.HorizontalScope(GUIStyle_HelpBox(), GUILayout.ExpandWidth(true), GUILayout.MaxHeight(10)))
                    {
                        EditorGUILayout.LabelField(selectDataKey, GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(200));
                        EditorGUILayout.LabelField(selectDataValue, GUIStyle_KeyValueTitle_Text());

                        EditorGUILayout.LabelField("Action", GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(240));
                    }

                    scrollPosition_DATA = EditorGUILayout.BeginScrollView(scrollPosition_DATA);

                    if (selectData != null)
                    {
                        EditorGUI.BeginChangeCheck();

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Main Preset", GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(200));
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
                                if (GUILayout.Button("Create", GUILayout.MaxWidth(240)))
                                {
                                    TileSetData tileSetData = CreateInstance<TileSetData>();
                                    AssetDatabase.CreateAsset(tileSetData, string.Format("Assets/Resources/GameData/TileSet/{0}_MainTileSet.asset", 
                                        selectData.name));
                                    
                                    selectData.mainPreset = tileSetData;
                                }
                            }
                        }
                        
                        EditorGUILayout.Space(20);

                        using (new EditorGUILayout.VerticalScope())
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("Sub Preset List", GUIStyle_KeyValueTitle_Text());

                                if (selectData.subPreset == null)
                                {
                                    selectData.subPreset = new List<TileSetData>();
                                }
                            }

                            EditorGUILayout.Space(10);

                            if (selectData.subPreset.Count > 0)
                            {
                                for(int i = 0; i < selectData.subPreset.Count; i++)
                                {
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        selectData.subPreset[i] = (TileSetData)EditorGUILayout.ObjectField(selectData.subPreset[i], typeof(TileSetData), false);
 
                                        if (GUILayout.Button("Reset", GUILayout.MaxWidth(120)))
                                        {
                                            selectData.subPreset[i] = null;
                                            break;
                                        }

                                        if (GUILayout.Button("Delete", GUILayout.MaxWidth(120)))
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
                            else
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUILayout.LabelField("(새로운 데이터를 추가해주세요!)", GUIStyle_KeyValueTitle_Text());
                                }

                                EditorGUILayout.Space(10);
                            }

                            using (new EditorGUILayout.HorizontalScope())
                            {
                                if (GUILayout.Button("Add (Empty)"))
                                {
                                    selectData.subPreset.Add(null);
                                }
                                
                                if (GUILayout.Button("Add (Create)"))
                                {
                                    TileSetData tileSetData = CreateInstance<TileSetData>();
                                    AssetDatabase.CreateAsset(tileSetData, string.Format("Assets/Resources/GameData/TileSet/{0}_SubTileSet_{1}.asset", 
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

                        EditorGUILayout.LabelField("데이터를 선택해주세요!", GUIStyle_KeyValueTitle_Text());
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
        }

        public override void Update()
        {
            datas = Resources.LoadAll<LevelData>("GameData/Level");
        }

        #region GUI_Style
        GUIStyle GUIStyle_HelpBox()
        {
            GUIStyle style = new GUIStyle("HelpBox");
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(0, 0, 0, 0);

            return style;
        }

        GUIStyle GUIStyle_KeyValueTitle_Text()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.LowerCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 14;

            return style;
        }

        GUIStyle GUIStyle_SelectButton()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 2, 2);
            style.padding = new RectOffset(0, 0, 5, 5);

            return style;
        }

        GUIStyle GUIStyle_SelectedButton()
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
    
    private class ThemeEditor : EditorContent
    {

        ThemeData selectData;
        ThemeData[] datas;

        string serachValue = string.Empty;

        string selectDataKey = "Tile Prefab";
        string selectDataValue = "Data";

        Vector2 scrollPosition_LIST = Vector2.zero;
        Vector2 scrollPosition_DATA = Vector2.zero;

        public override void Init()
        {
            MENU_TYPE = menuType.THEME;
            BUTTON_TEXT = "Theme Editor";

            Update();
        }

        public override void DrawContent()
        {
            using (new EditorGUILayout.HorizontalScope("GroupBox", GUILayout.ExpandWidth(true)))
            {
                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.MaxWidth(260), GUILayout.ExpandHeight(true)))
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        EditorGUILayout.LabelField("Serach", GUILayout.MaxWidth(45));
                        serachValue = EditorGUILayout.TextField(serachValue);

                        if (GUILayout.Button("Refresh", GUILayout.MaxWidth(80)))
                        {
                            serachValue = string.Empty;
                            Update();
                        }
                    }

                    EditorGUILayout.Space(6);

                    scrollPosition_LIST = EditorGUILayout.BeginScrollView(scrollPosition_LIST);

                    bool isSerach = serachValue != string.Empty;

                    foreach (ThemeData data in datas)
                    {
                        if (data == null)
                            break;
                        
                        bool isKeyMatch = data.name.SpecialContains(serachValue);

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
                    
                    if (GUILayout.Button("Add Theme", GUIStyle_SelectedButton()))
                    {
                        ThemeData themeData = CreateInstance<ThemeData>();
                        AssetDatabase.CreateAsset(themeData, string.Format("Assets/Resources/GameData/Theme/Theme_{0:000}.asset", datas.Length));
                        
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

                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(selectData);
                            Save();
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space(6);

                        EditorGUILayout.LabelField("데이터를 선택해주세요!", GUIStyle_KeyValueTitle_Text());
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
        }

        public override void Update()
        {
            datas = Resources.LoadAll<ThemeData>("GameData/Theme");
        }

        #region GUI_Style
        GUIStyle GUIStyle_HelpBox()
        {
            GUIStyle style = new GUIStyle("HelpBox");
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(0, 0, 0, 0);

            return style;
        }

        GUIStyle GUIStyle_KeyValueTitle_Text()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.LowerCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 14;

            return style;
        }

        GUIStyle GUIStyle_SelectButton()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 2, 2);
            style.padding = new RectOffset(0, 0, 5, 5);

            return style;
        }

        GUIStyle GUIStyle_SelectedButton()
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

    private class TilePrefabEditor : EditorContent
    {

        TilePrefabData selectData;
        TilePrefabData[] datas;

        string serachValue = string.Empty;

        string selectDataKey = "TileID";
        string selectDataValue = "Prefab / Offset";

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
            using (new EditorGUILayout.HorizontalScope("GroupBox", GUILayout.ExpandWidth(true)))
            {
                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.MaxWidth(260), GUILayout.ExpandHeight(true)))
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        EditorGUILayout.LabelField("Serach", GUILayout.MaxWidth(45));
                        serachValue = EditorGUILayout.TextField(serachValue);

                        if (GUILayout.Button("Refresh", GUILayout.MaxWidth(80)))
                        {
                            serachValue = string.Empty;
                            Update();
                        }
                    }

                    EditorGUILayout.Space(6);

                    scrollPosition_LIST = EditorGUILayout.BeginScrollView(scrollPosition_LIST);

                    bool isSerach = serachValue != string.Empty;

                    foreach (TilePrefabData data in datas)
                    {
                        if (data == null)
                            break;

                        bool isKeyMatch = data.name.SpecialContains(serachValue);

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
                        EditorGUILayout.LabelField(selectDataKey, GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(260));
                        EditorGUILayout.LabelField(selectDataValue, GUIStyle_KeyValueTitle_Text());

                        EditorGUILayout.LabelField("Action", GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(80));
                    }

                    scrollPosition_DATA = EditorGUILayout.BeginScrollView(scrollPosition_DATA);

                    if (selectData != null)
                    {
                        if (selectData.prefabList.Count > 0)
                        {
                            keys = selectData.prefabList.Keys.OrderBy(x => (int)x).ToArray();

                            foreach (TileID key in keys)
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUI.BeginChangeCheck();

                                    EditorGUILayout.LabelField(key.ToString(), GUIStyle_KeyValueTitle_Text(), GUILayout.MaxWidth(260));
                                    selectData.prefabList[key] = (GameObject)EditorGUILayout.ObjectField(selectData.prefabList[key], typeof(GameObject), false);
                                    selectData.offsetList[key] = EditorGUILayout.Vector3Field("", selectData.GetOffset(key), GUILayout.MaxWidth(120));

                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        EditorUtility.SetDirty(selectData);
                                        Save();
                                    }

                                    if (GUILayout.Button("Reset", GUILayout.MaxWidth(80)))
                                    {
                                        selectData.prefabList[key] = null;
                                        selectData.offsetList[key] = Vector3.zero;
                                        break;
                                    }
                                }
                            }
                        }
                        
                        EditorGUILayout.Space(6);
                        
                        if (GUILayout.Button("Data Check & Sort"))
                        {
                            DataCheck();
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space(6);

                        EditorGUILayout.LabelField("데이터를 선택해주세요!", GUIStyle_KeyValueTitle_Text());
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
                    DataCheck();
                }
            }
        }

        void DataCheck()
        {
            foreach (TileID tileID in Enum.GetValues(typeof(TileID)))
            {
                if (!selectData.prefabList.ContainsKey(tileID))
                {
                    selectData.prefabList.Add(tileID, null);
                }
                        
                if (!selectData.offsetList.ContainsKey(tileID))
                {
                    selectData.offsetList.Add(tileID, Vector3.zero);
                }
            }

            keys = selectData.prefabList.Keys.ToArray();
        }

        #region GUI_Style
        GUIStyle GUIStyle_HelpBox()
        {
            GUIStyle style = new GUIStyle("HelpBox");
            style.margin = new RectOffset(0, 0, 0, 0);
            style.padding = new RectOffset(0, 0, 0, 0);

            return style;
        }

        GUIStyle GUIStyle_KeyValueTitle_Text()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.LowerCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 14;

            return style;
        }

        GUIStyle GUIStyle_SelectButton()
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.margin = new RectOffset(0, 0, 2, 2);
            style.padding = new RectOffset(0, 0, 5, 5);

            return style;
        }

        GUIStyle GUIStyle_SelectedButton()
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

        string serachValue = string.Empty;

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
            using (new EditorGUILayout.HorizontalScope("GroupBox", GUILayout.ExpandWidth(true)))
            {
                using (new EditorGUILayout.VerticalScope("HelpBox", GUILayout.MaxWidth(260), GUILayout.ExpandHeight(true)))
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        EditorGUILayout.LabelField("Serach", GUILayout.MaxWidth(45));
                        serachValue = EditorGUILayout.TextField(serachValue);

                        if (GUILayout.Button("Refresh", GUILayout.MaxWidth(80)))
                        {
                            serachValue = string.Empty;
                            Update();
                        }
                    }

                    EditorGUILayout.Space(6);

                    scrollPosition_LIST = EditorGUILayout.BeginScrollView(scrollPosition_LIST);

                    bool isSerach = serachValue != string.Empty;

                    foreach (TileFloorPrefabData data in datas)
                    {
                        if (data == null)
                            break;

                        bool isKeyMatch = data.name.SpecialContains(serachValue);

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

                        EditorGUILayout.LabelField("데이터를 선택해주세요!", GUIStyle_KeyValueTitle_Text());
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

        #region GUI_Style
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
