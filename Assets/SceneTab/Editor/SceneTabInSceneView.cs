using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ARYKEI.EditorExtension
{
    public class SceneTabInSceneView
    {               
        public static bool showTab = false;
        private const string menuPath = "Window/SceneTab/Show in SceneView";

        [MenuItem(menuPath)]
        static void EnableSceneTab()
        {
            bool ch = Menu.GetChecked(menuPath);
            Menu.SetChecked(menuPath,!ch);
            showTab = !ch;
        }

        [InitializeOnLoadMethod]
        private static void InitDelegate()
        {
            showTab = Menu.GetChecked(menuPath);

 #if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += SceneTabDrawer.OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += SceneTabDrawer.OnSceneGUI;
#endif

        }
    }
}
