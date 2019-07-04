using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace ARYKEI.EditorExtension
{
    public class SceneTab : EditorWindow
    {
        public Vector2 scrollPosition;
        [MenuItem("Window/SceneTab/Open Window")]
        static void Open()
        {
            GetWindow<SceneTab>(false, "SceneTab");
        }

        private  void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            SceneTabDrawer.DrawButtons(this);
            GUILayout.EndScrollView();
        }
    }


    public static class SceneTabDrawer
    {      
        public static Vector2 scrollPosition;
        private static int buttonCountPerLine = 5;
        private static float lineHeight = 21;
        private static float minWidth = 100;
        private static float ControlButtonWidth = 25;
        
        private static Color minusButtonColor = new Color(0.8f,0.6f,0.6f,0.5f);
        private static Color plusButtonColor =   new Color(0.6f,0.6f,0.8f,0.5f);
        private static Color SceneButtonColor =  new Color(0.6f,0.8f,0.6f,0.5f);
        private static Color ButtonColor =  new Color(0.9f,0.9f,0.9f,0.5f);

        public static void OnSceneGUI(SceneView sceneView)
        {
            if(!SceneTabInSceneView.showTab) return;

            Handles.BeginGUI ();
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition,GUILayout.Width((int)(sceneView.position.width-100)), GUILayout.Height(50));
            DrawButtons(null);
            GUILayout.EndScrollView();

            Handles.EndGUI ();
        }
        public static void DrawButtons(EditorWindow win)
        {            
            int sceneCount = EditorSceneManager.sceneCountInBuildSettings;

            if (sceneCount == 0)
            {
                GUILayout.Label("There is no scenes in Build Settings.");

                if(win != null)
                    win.minSize = new Vector2(minWidth, lineHeight);
                return;
            }

            bool isHorizontal = true;

            if (win != null)
            {
                isHorizontal = win.position.height < win.position.width;
                win.minSize = new Vector2(minWidth, lineHeight * Mathf.Ceil(sceneCount / (float)buttonCountPerLine));
            }

            if (isHorizontal)
                GUILayout.BeginHorizontal();

            for (int i = 0; i < sceneCount; i++)
            {
                if (i % buttonCountPerLine == 0 && isHorizontal)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                DrawButton(i);
                GUILayout.Space(5);
            }
            if (isHorizontal)
                GUILayout.EndHorizontal();
        }
        
        public static void DrawButton(int i)
        {
            bool isPlaymode = Application.isPlaying;
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var scene = EditorSceneManager.GetSceneByBuildIndex(i);
            var filename = Path.GetFileName(scenePath);


            Color defualtColor = GUI.backgroundColor;


            GUILayout.BeginHorizontal();
            if (scene.isLoaded)
            {
                GUI.enabled = !isPlaymode && EditorSceneManager.loadedSceneCount > 1;
                GUI.backgroundColor = GUI.enabled ? minusButtonColor : Color.gray;
                if (GUILayout.Button("-", GUILayout.Width(ControlButtonWidth)))
                {
                    if (!scene.isDirty ||EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = !isPlaymode;
                GUI.backgroundColor = plusButtonColor;
                if (GUILayout.Button("+", GUILayout.Width(ControlButtonWidth)))
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
                GUI.enabled = true;
            }

            GUI.enabled = !isPlaymode;
            GUI.backgroundColor = scene.isLoaded ? SceneButtonColor : ButtonColor;
            if (GUILayout.Button(filename.Substring(0, filename.IndexOf(".unity"))))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                }
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUI.backgroundColor = defualtColor;
        }
    }
}