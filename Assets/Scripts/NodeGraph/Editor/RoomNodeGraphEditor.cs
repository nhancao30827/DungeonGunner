using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungGunCore
{
    public class RoomNodeGraphEditor : EditorWindow
    {
        private GUIStyle _nodeStyle;  

        #region NODE LAYOUTS
        private const float _nodeWidth = 160f;
        private const float _nodeHeight = 75f;
        private const int _nodePadding = 25;
        private const int _nodeBorder = 12; 
        #endregion

        [MenuItem("Toom Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")] 
        private static void OpenWindow()
        {
            RoomNodeGraphEditor window = GetWindow<RoomNodeGraphEditor>();
            window.titleContent = new GUIContent("Room Node Graph Editor");
        }

        private void OnEnable()
        {
            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
            _nodeStyle.normal.textColor = Color.white;
            _nodeStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);
            _nodeStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);  
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(new Vector2(100, 200), new Vector2(_nodeWidth, _nodeHeight)), _nodeStyle);
            GUILayout.Label("Node1", EditorStyles.boldLabel);
            GUILayout.EndArea();
        }

    }
}
