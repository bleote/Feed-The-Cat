using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Net.NetworkInformation;

public class Log : MonoBehaviour {

    //----------------Statics-----------------
    int logNo;
    string consoleUI;
    float sbarThickness;
    Vector2 menuBtnSize;
    public static float gap;
    public static string debugStr;
    public static Vector2 buttonSize;
    //----------------UI-----------------
    ScreenOrientation orientation;
    GUIWindow menu;
    GUIWindow console;
    GUIWindow debug;
    GUIWindow hierarchy;
    GUIWindow inspector;
    GUIWindow stats;
    GUIWindow scene;
    GUIWindow settings;

    bool valid; // check input validity
    float touchPosY; //for sliding dunc
    Transform selectedGO; // gameobject selected for inspector
    List<int> hierarchyBtn = new List<int>(); // maintain bools for hierarchy
    static Log instance;
    NetworkInterface[] nics;

    void Start () {
        if (GetComponents<Component>().Length > 2) {
            GameObject go = new GameObject();
            go.AddComponent<Log>();
            go.name = "Log";
            Destroy(this);
        }
        else { 
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad (this);
                orientation = ScreenOrientation.AutoRotation;
                Application.logMessageReceived += Application_logMessageReceived;
            }
            else
                Destroy (gameObject);
        }
    }

    string MyDebug() { // DEBUG YOUR VALUES HERE
        debugStr = "";
        debugStr += "Screen.height " + Screen.height + "\n";
        debugStr += "touchCount " + Input.touchCount + "\n";
        //debugStr += "swicthCount " + DownloadManager.switchServerCounts + "\n";
        //debugStr += ".currentDownloadings. " + DownloadManager.currentDownloadings.Count + "\n";
        return debugStr;
    }

    void OnGUI () {
        // --------Assinging Variables----------
        if (orientation != Screen.orientation) {
            InitializeValues();
            orientation = Screen.orientation;
        }
        // --------Screen View----------
        if (menu.enable)
            ShowMenu(); // Showing Menu
        if (console.enable)
            ShowConsole(); // Showing Console Logs
        if (debug.enable)
            ShowDebug(); // Showing My Debug
        if (hierarchy.enable)
            ShowHierarchy(); // Showing Scene Hierarchy
        if (inspector.enable)
            ShowInspector(); // Showing Inspector
        if (stats.enable)
            ShowStats(); // Showing System Stats
        if (scene.enable)
            ShowScene(); // Showing Build Settings
        if (settings.enable)
            ShowSettings(); // Showing Project Settings
    }

    #region Initialization

    void InitializeValues () {
        float screenSize = Screen.height + Screen.width;
        gap = screenSize / 500f;
        sbarThickness = screenSize / 50f;
        menuBtnSize = new Vector2(screenSize / 12f, screenSize / 30f);
        buttonSize = menuBtnSize * .75f;

        GUI.color = Color.white;
        GUI.skin.toggle.border = new RectOffset(1, 1, 0, 0);
        GUI.skin.horizontalScrollbar.fixedHeight = GUI.skin.verticalScrollbar.fixedWidth = sbarThickness;
        GUI.skin.horizontalScrollbarThumb.fixedHeight = GUI.skin.verticalScrollbarThumb.fixedWidth = sbarThickness;
        GUI.skin.window.fontSize = GUI.skin.box.fontSize = GUI.skin.label.fontSize = GUI.skin.button.fontSize = GUI.skin.textField.fontSize = GUI.skin.toggle.fontSize = (int)(screenSize / 80f);

        InitializeMenu();
        InitializeConsole();
        InitializeHierarchy();
        InitializeInspector();
        InitializeDebug();
        InitializeStats();
        InitializeScene();
        InitializeSettings();
    }

    #endregion Initialization 

    #region Showing Menu

    void InitializeMenu() {
        menu = new GUIWindow(1, "Menu", new Rect(0, 0, menuBtnSize.x, menuBtnSize.y * 8), false);
        menu.AttachHeader = () => {};
    }

    void ShowMenu() {
        menu.AttachContent = () => {
            if (GUI.Button(new Rect(0, menuBtnSize.y * 0, menuBtnSize.x, menuBtnSize.y), debug.enable ? "Debug+" : "Debug"))
                debug.enable = !debug.enable;
            if (GUI.Button(new Rect(0, menuBtnSize.y * 1, menuBtnSize.x, menuBtnSize.y), console.enable ? "Console+" : "Console"))
                console.enable = !console.enable;
            if (GUI.Button(new Rect(0, menuBtnSize.y * 2, menuBtnSize.x, menuBtnSize.y), hierarchy.enable ? "Hierarchy+" : "Hierarchy"))
                hierarchy.enable = !hierarchy.enable;
            if (GUI.Button(new Rect(0, menuBtnSize.y * 3, menuBtnSize.x, menuBtnSize.y), inspector.enable ? "Inspector+" : "Inspector"))
                inspector.enable = !inspector.enable;
            if (GUI.Button(new Rect(0, menuBtnSize.y * 4, menuBtnSize.x, menuBtnSize.y), stats.enable ? "Stats+" : "Stats"))
                stats.enable = !stats.enable;
            if (GUI.Button(new Rect(0, menuBtnSize.y * 5, menuBtnSize.x, menuBtnSize.y), scene.enable ? "Scene+" : "Scene"))
                scene.enable = !scene.enable;
            if (GUI.Button(new Rect(0, menuBtnSize.y * 6, menuBtnSize.x, menuBtnSize.y), settings.enable ? "Settings+" : "Settings"))
                settings.enable = !settings.enable;
        };
        menu.Show();
    }

    #endregion Showing Menu 
    
    #region Showing Debug 

    void InitializeDebug() {
        debug = new GUIWindow(2, "Debug", new Rect(0, 0, Screen.width / 2f, Screen.height / 2));
        debug.AttachHeader = () => {};
    }

    void ShowDebug() {
        debug.AttachContent = () => { GUI.Label(new Rect(gap * 2, 0, debug.bounds.width - sbarThickness * 1.4f, debug.contentSize), MyDebug());};
        debug.Show();
    }

    #endregion Showing Debug

    #region Showing Console

    void InitializeConsole() {
        console = new GUIWindow(3, "Console", new Rect(0, 0, Screen.width / 1.1f, Screen.height / 2));
        console.AttachHeader = () => {
            if (GUI.Button(new Rect(0, 0, buttonSize.x, buttonSize.y), "Clear"))
                ClearConsole();
        };
    }

    void ShowConsole() {
        GUIContent content = new GUIContent(consoleUI);
        Vector2 size = GUI.skin.label.CalcSize(content);
        console.contentSize = size.y * 2.5f;
        console.AttachContent = () => {
            GUI.Label(new Rect(gap * 2, 0, console.bounds.width - sbarThickness * 1.4f, console.contentSize), consoleUI);};
        console.Show();
    }

    void Application_logMessageReceived(string condition, string stackTrace, LogType type) {
        consoleUI += "<b><color=" + (type == LogType.Warning ? "yellow" : (type == LogType.Error || type == LogType.Exception ? "red" : "white")) + ">" + (++logNo) + " " + type  +"</color></b> " + string.Format("{0}, {1}", condition, stackTrace) + "\n\n";
	}

    void ClearConsole() {
        consoleUI = "";
        logNo = 0;
    }

    #endregion Showing Console 

    #region Showing Hierarchy
    
    void InitializeHierarchy() {
        hierarchy = new GUIWindow(4, "Hierarchy", new Rect(0, 0, Screen.width / 2, Screen.height / 2));
        hierarchy.AttachHeader = () => {};
    }

    void ShowHierarchy() {
        hierarchy.AttachContent = () => {
            int row = -1, depth = -1;
            List<Transform> rootObjects = GetRootObjectsInScene();
            ShowHierarchy(rootObjects, depth, ref row, 0);
            hierarchy.contentSize = (row + 2) * buttonSize.y * .85f;
        };
        hierarchy.Show();
    }

    void ShowHierarchy(List <Transform> objs, int depth, ref int row, int id) {
        depth++;
        Vector2 rowSize = new Vector2(hierarchy.bounds.width - sbarThickness, buttonSize.y * .85f);
        for (int i = 0; i < objs.Count; i++) {
            row++;
            id = id + i; // to maintian bool
            bool opened = hierarchyBtn.Contains(id);
            bool hasChild = objs[i].childCount > 0;
            string sign = hasChild ? (opened ? "v " : "> ") : "   ";
            GUI.Label(new Rect(depth * gap * 5, row * rowSize.y, rowSize.x, rowSize.y), sign + "      " + objs[i].name);
            if (GUI.Toggle(new Rect(gap * 4 + depth * gap * 5, row * rowSize.y, rowSize.y * .85f, rowSize.y), opened, "")) {
                List<Transform> childs = new List<Transform>();
                foreach (Transform child in objs[i])
                    childs.Add(child);
                ShowHierarchy(childs, depth, ref row, id * 10);
                if (!opened)
                    selectedGO = objs[i]; // to show its transform
                if (!opened && hasChild)
                    hierarchyBtn.Add(id); // open hierarchy
            }
            else if (opened)
                hierarchyBtn.Remove(id); // close hierarchy
        }
    }

    List<Transform> GetRootObjectsInScene() {
        List<Transform> rootObjectsInScene = new List<Transform>();
        Transform[] objects = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform go in objects)
            if (go == go.root && go.hideFlags == HideFlags.None)
                rootObjectsInScene.Add(go);
        return rootObjectsInScene;
    }

    #endregion Showing Hierarchy 

    #region Showing Inspector 

    void InitializeInspector() {
        inspector = new GUIWindow(5, "Inspector", new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height));// multiply with 8 i.e rows
        inspector.AttachHeader = () => {};
    }
    void ShowInspector() {
        inspector.AttachContent = () => {
            if (selectedGO) {
                int row = 6;
                ShowInspector(ref row);
                inspector.contentSize = (row + 2) * buttonSize.y * .85f;
            }
        };
        inspector.Show();
    }

    void ShowInspector(ref int row) {
        float rowWidth = inspector.bounds.width;
        float rowHeight = buttonSize.y * .85f;
        selectedGO.gameObject.SetActive(GUI.Toggle(new Rect(rowWidth / 90, rowHeight * 0, rowHeight * .85f, rowHeight), selectedGO.gameObject.activeSelf, ""));
        selectedGO.name = GUI.TextField(new Rect(rowWidth / 9, rowHeight * 0, rowWidth / 2, rowHeight), selectedGO.name);
        selectedGO.gameObject.isStatic = GUI.Toggle(new Rect(rowWidth / 1.5f, rowHeight * 0, rowHeight * .85f, rowHeight), selectedGO.gameObject.isStatic, "");
        GUI.Label(new Rect(rowWidth / 1.3f, rowHeight * 0, rowWidth / 3, rowHeight), "Static");
        GUI.Label(new Rect(gap * 2, rowHeight * 1, rowWidth / 5, rowHeight), "Tag");
        GUI.Box(new Rect(rowWidth / 6, rowHeight * 1, rowWidth / 3, rowHeight), selectedGO.tag);
        GUI.Label(new Rect(rowWidth / 1.8f, rowHeight * 1, rowWidth / 5, rowHeight), "Layer");
        int num;
        if (int.TryParse(GUI.TextField(new Rect(rowWidth / 1.3f, rowHeight * 1, rowWidth / 6, rowHeight), selectedGO.gameObject.layer.ToString()), out num))
            selectedGO.gameObject.layer = num;
        GUI.Box(new Rect(gap * 2, rowHeight * 2, rowWidth / 1.5f, rowHeight), "Transform");
        selectedGO.localPosition = CreateTransformFields(selectedGO.localPosition, "Position", rowHeight * 3, rowWidth, rowHeight);
        selectedGO.localEulerAngles = CreateTransformFields(selectedGO.localEulerAngles, "Rotation", rowHeight * 4, rowWidth, rowHeight);
        selectedGO.localScale = CreateTransformFields(selectedGO.localScale, "Scale", rowHeight * 5, rowWidth, rowHeight);
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        for (int i = 1; i < selectedGO.GetComponents<Component>().Length; i++) {
            Object obj = selectedGO.GetComponents<Component>()[i];
            GUI.Box(new Rect(gap * 2, row * rowHeight, rowWidth / 1.5f, rowHeight), obj.ToString());
            row++;
            foreach (FieldInfo field in obj.GetType().GetFields(bindingFlags)) {
                GUI.Label(new Rect(gap * 2, row * rowHeight, rowWidth, rowHeight), field.Name);
                string val = ""; float num2; bool flag;
                if (field.GetValue(obj) != null)
                    val = field.GetValue(obj).ToString();
                val = GUI.TextField(new Rect(rowWidth / 2.5f, row * rowHeight, rowWidth / 1.7f, rowHeight), val);
                if (field.GetValue(obj) != null && !field.IsLiteral && !field.IsInitOnly) {
                    if (field.FieldType.ToString() == "System.String")
                        field.SetValue(obj, val);
                    else if (field.FieldType.ToString() == "System.Boolean" && bool.TryParse(val, out flag))
                        field.SetValue(obj, flag);
                    else if (field.FieldType.ToString() == "System.Int32" && int.TryParse(val, out num))
                        field.SetValue(obj, num);
                    else if (field.FieldType.ToString() == "System.Single" && float.TryParse(val, out num2))
                        field.SetValue(obj, num2);
                }
                row++;
            }
        }
    }

    Vector3 CreateTransformFields(Vector3 dimension, string fieldName, float posY, float rowWidth, float rowHeight) {
        float val;
        GUI.Label(new Rect(rowWidth / 60, posY, rowWidth / 3, rowHeight), fieldName);
        GUI.Label(new Rect(rowWidth / 3.5f, posY, rowWidth / 10, rowHeight), "X");
        GUI.Label(new Rect(rowWidth / 1.9f, posY, rowWidth / 10, rowHeight), "Y");
        GUI.Label(new Rect(rowWidth / 1.32f, posY, rowWidth / 10, rowHeight), "Z");
        if (float.TryParse(GUI.TextField(new Rect(rowWidth / 3, posY, rowWidth / 5.5f, rowHeight), dimension.x.ToString()), out val))
            dimension.x = val;
        if (float.TryParse(GUI.TextField(new Rect(rowWidth / 1.75f, posY, rowWidth / 5.5f, rowHeight), dimension.y.ToString()), out val))
            dimension.y = val;
        if (float.TryParse(GUI.TextField(new Rect(rowWidth / 1.25f, posY, rowWidth / 5.5f, rowHeight), dimension.z.ToString()), out val))
            dimension.z = val;
        return dimension;
    }

    #endregion Showing Inspector 

    #region Showing Stats 

    void InitializeStats() {
        stats = new GUIWindow(6, "Stats", new Rect(0, 0, Screen.width / 1.75f, Screen.height / 1.75f));
        stats.AttachHeader = () => {
            if (GUI.Button(new Rect(0, 0, buttonSize.x, buttonSize.y), "Reset"))
                ResetStats();
        };
    }

    void ShowStats() {
        stats.AttachContent = () => { GUI.Label(new Rect(gap * 2, 0, stats.bounds.width - sbarThickness * 1.4f, stats.contentSize), GetStats());};
        stats.Show();
    }

    static int fps, minFps = 99999, maxFps = 0, avgFps, count;
    void ResetStats () {
        minFps = 99999;
        maxFps = avgFps = count = 0;
    }

    void CalculateStats () {
        fps = (int)(1f / Time.deltaTime);
        if (fps < minFps && fps != 0)
            minFps = fps;
        if (fps > maxFps)
            maxFps = fps;
        avgFps += fps;
        count++;
    }

    string GetStats() {
        if (nics == null)
            nics = NetworkInterface.GetAllNetworkInterfaces();
        CalculateStats();
        string str = "";
        str += "FPS = " + fps;
        str += "\nMin FPS = " + minFps;
        str += "\nMax FPS = " + maxFps;
        str += "\nAvg FPS = " + (int)((float)avgFps / count);
        str += "\nScreen DPI = " + Screen.dpi;
        str += "\nAlloc RAM = " + (UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1000000f).ToString("0") + " MB";
        str += "\nUsed RAM = " + (UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1000000f).ToString("0") + " MB";
        str += "\nFree RAM = " + (UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / 1000000f).ToString("0") + " MB";
        str += "\nSystem RAM = " + SystemInfo.systemMemorySize.ToString() + " MB";
        str += "\nGPU Memory = " + SystemInfo.graphicsMemorySize.ToString() + " MB";
        str += "\nCPU Cores = " + SystemInfo.processorCount.ToString();
        str += "\nCPU Frequency = " + (Mathf.Ceil(SystemInfo.processorFrequency / 10f) / 100f).ToString("0.00") + " GHz";
        str += "\nCPU = " + SystemInfo.processorType;
        str += "\nGPU = " + SystemInfo.graphicsDeviceName;
        str += "\nOS = " + SystemInfo.operatingSystem;
        str += "\nDevice Model = " + SystemInfo.deviceModel;
        if (nics.Length > 0)
            str += "\n" + nics[0].Name + " MAC Address = " + nics[0].GetPhysicalAddress();
        return str;
    }

    #endregion Showing Stats

    #region Showing Scene 

    void InitializeScene() {
        scene = new GUIWindow(7, "Build Settings", new Rect(0, 0, Screen.width / 1.5f, menuBtnSize.y * .85f * 8));
        //scene.contentSize = buttonSize.y * .85f * (SceneManager.sceneCountInBuildSettings + 1);                                       // error in SceneManger class! , some unity api is changed,   will check it later!            
        scene.AttachHeader = () => {};
    }

    void ShowScene() {
        scene.AttachContent = () => {
            float rowWidth = scene.bounds.width;
            float rowHeight = menuBtnSize.y * .85f;
            //for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {                                                                    // error in SceneManger class! , some unity api is changed,   will check it later!           
            //    if (GUI.Button(new Rect(rowWidth / 90, rowHeight * i, rowWidth / 1.75f, rowHeight), SceneManager.GetSceneByBuildIndex(i).name))   // error in SceneManger class! , some unity api is changed,   will check it later!       
            //        SceneManager.LoadScene(i);                                                                                                    // error in SceneManger class! , some unity api is changed,   will check it later!             
            //}
        };
        scene.Show();
    }

    #endregion Showing Scene

    #region Showing Settings 

    void InitializeSettings() {
        settings = new GUIWindow(8, "Project Settings", new Rect(0, 0, Screen.width / 1.5f, menuBtnSize.y * 13));
        settings.AttachHeader = () => {};
    }

    void ShowSettings() {
        settings.AttachContent = () => {
            float rowWidth = settings.bounds.width;
            float rowHeight = menuBtnSize.y * .85f;
            if (GUI.Button(new Rect(rowWidth / 90, rowHeight * 0, rowWidth / 1.75f, rowHeight), "TimeScale " + Time.timeScale)) {
                Time.timeScale = Time.timeScale + 0.1f;
                Time.timeScale = (int)(Time.timeScale * 10) / 10f;
                if (Time.timeScale > 1)
                    Time.timeScale = 0;
            }
            if (GUI.Button(new Rect(rowWidth / 90, rowHeight * 1, rowWidth / 1.75f, rowHeight), "Clear PlayerPrefs"))
                PlayerPrefs.DeleteAll();
            rowHeight = menuBtnSize.y * 2;
            for (int i = 0; i < Input.touches.Length; i++) {
                GUI.Label(new Rect(rowWidth / 90, rowHeight * (i + 1), rowWidth / 1.1f, rowHeight), "Touch " + i + " :" + Input.touches[i].position);
                GUI.Label(new Rect(rowWidth / 30, rowHeight * (i + 1.25f), rowWidth / 1.1f, rowHeight), "Radius : " + Input.touches[i].radius);
                GUI.Label(new Rect(rowWidth / 30, rowHeight * (i + 1.5f), rowWidth / 1.1f, rowHeight), "FingerId : " + Input.touches[i].fingerId);
                GUI.Label(new Rect(rowWidth / 30, rowHeight * (i + 1.75f), rowWidth / 1.1f, rowHeight), "DeltaPosition : " + Input.touches[i].deltaPosition);
            }
        };
        settings.Show();
    }

    #endregion Showing Settings

    #region Sliding Functions
    void Update() {
        if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)) {
            touchPosY = Input.mousePosition.y;
            valid = Input.mousePosition.y > Screen.height - sbarThickness * 2; // if start slide from menu area
        }

        if (Input.mousePosition.y > Screen.height - sbarThickness * 2 && ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))) {
            if (Mathf.Abs (touchPosY - Input.mousePosition.y) > sbarThickness * 2) // if end slide in menu area
                menu.enable = false;
        }

        if (valid && (Input.touchCount > 0 || Input.GetMouseButtonUp(0)) && Mathf.Abs(Input.mousePosition.y - touchPosY) > sbarThickness * 2) {
            if (!menu.enable) { 
                menu.bounds.x = Input.mousePosition.x - menu.bounds.size.x / 2;
                menu.bounds.y = Screen.height - menu.bounds.size.y;
            }
            menu.enable = true;
            valid = false;
        }
    }

    #endregion Sliding Functions

    #region Editor Functions

#if UNITY_EDITOR
    [MenuItem("File/Create/Log")]
    public static void CreateLog() {
        new GameObject().AddComponent<Log>().name = "Log";
    }

    [MenuItem("File/Scene/Restart")]
    public static void Restart() {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);          // error in SceneManger class! , some unity api is changed,   will check it later!          
    }

    [MenuItem("File/Scene/Next")]
    public static void NextScene() {
        //SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);          // error in SceneManger class! , some unity api is changed,   will check it later!             
    }
    [MenuItem("File/Clear PlayerPrefs")]
    public static void ClearPlayerPrefs() {
        PlayerPrefs.DeleteAll ();
    }

    [MenuItem("File/TimeScale/0.2f")]
    public static void TimeScaleSlow() {
        Time.timeScale = .2f;
    }

    [MenuItem("File/TimeScale/0.5f")]
    public static void TimeScaleMedium() {
        Time.timeScale = 0.5f;
    }

    [MenuItem("File/TimeScale/1.0f")]
    public static void TimeScaleDefault() {
        Time.timeScale = 1;
    }
#endif

    #endregion Editor Functions

}

class GUIWindow {
    public int id;
    public bool enable;
    public bool closeable;
    public string name;
    public float contentSize;
    public Rect bounds;

    bool drag = true;
    Rect defaultBounds;
    Vector2 scrollPos;

    public delegate void AttachHeaderDelegate();
    public delegate void AttachContentDelegate();
    public AttachHeaderDelegate AttachHeader;
    public AttachContentDelegate AttachContent;

    public GUIWindow (int _id, string _name, Rect _bounds, bool _closeable = true) {
        id = _id;
        name = _name;
        bounds = defaultBounds = _bounds;
        contentSize = _bounds.height;
        closeable = _closeable;
    }

    public void Show() {
        if (bounds == Rect.zero || bounds.x > Screen.width || bounds.y > Screen.height) // set position first time and when goes out of screen
            bounds = defaultBounds;
        bounds = GUI.Window(id, bounds, DoWindow, name);
        GUI.BringWindowToFront(1);
    }

    void DoWindow(int windowID)  {
        AttachHeader();
        if (closeable) {
            if (GUI.Button(new Rect(bounds.width - Log.buttonSize.y, 0, Log.buttonSize.y, Log.buttonSize.y), "X"))
                enable = false;
            if (GUI.Button(new Rect(bounds.width - Log.buttonSize.y * 2 - Log.gap * 5, 0, Log.buttonSize.y, Log.buttonSize.y), drag ? "O" : "o")) {
                bounds = drag ? new Rect(0, 0, Screen.width, Screen.height) : defaultBounds;
                drag = !drag;
            }
        }
        scrollPos = GUI.BeginScrollView(new Rect(0, Log.buttonSize.y, bounds.width, bounds.height), scrollPos, new Rect(0, 0, bounds.width, contentSize));
        AttachContent();
        GUI.EndScrollView();
        if (drag)
            GUI.DragWindow (new Rect (0,0,10000, 10000));
    }
}
