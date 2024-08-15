using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using UnityEngine;

namespace DungGunCore
{
    public class RoomNodeGraphEditor : EditorWindow
    {
        private GUIStyle _nodeStyle;  
        private static RoomNodeGraphSO _roomNodeGraph;
        private RoomNodeSO _hoverRoomNode;
        private RoomNodeTypeListSO _roomNodeTypeList;

        #region NODE LAYOUTS
        private const float _nodeWidth = 160f;
        private const float _nodeHeight = 75f;
        private const int _nodePadding = 25;
        private const int _nodeBorder = 12;
        #endregion

        [MenuItem("Toom Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
        
        /// <summary> 
        /// Initialize the graph editor window 
        /// </summary>
        private static void OpenWindow()
        {
            RoomNodeGraphEditor window = GetWindow<RoomNodeGraphEditor>();
            window.titleContent = new GUIContent("Room Node Graph Editor");
        }

        /// <summary>
        /// Open this this graph window when double clicking on a .asset file
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset(0)]
        public static bool OnDoubleClickAsset(int instanceID, int line)
        {
            RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
            if (roomNodeGraph != null)
            {
                OpenWindow();
                _roomNodeGraph = roomNodeGraph;
                return true;
            }
            Debug.Log("Not a RoomNodeGraphSO");
            return false;
        }

        // Called when the window is opened
        private void OnEnable() 
        {
            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
            _nodeStyle.normal.textColor = Color.white;
            _nodeStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);
            _nodeStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);  

            _roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }

        // Handle creating nodes
        private void OnGUI()
        {
           if (_roomNodeGraph != null)
           {
               HandleEventDetection(Event.current);
               DrawRoomNodes();
           }

           if (GUI.changed)
           {
                Repaint();
           }
        }

        private void HandleEventDetection(Event e)
        {
            if (_hoverRoomNode == null || _hoverRoomNode.isDragging == false)
            {
                _hoverRoomNode = MouseOverNode(e);
            }

            if (_hoverRoomNode == null)
            {
                ProcessGraphEvents(e);
            }

            else
            {
                _hoverRoomNode.HandleNodeEvents(e);
            }
        }

        /// <summary>
        /// Handle events happening in the node graph editor
        /// </summary>
        /// <param name="e">Type of events happen in the editor</param>
        private void ProcessGraphEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    HandleMouseDownEven(e);
                    break;

                default:
                    break;
            }
        }

        private RoomNodeSO MouseOverNode(Event e)
        {
            foreach (RoomNodeSO roomNode in _roomNodeGraph.roomNodeList)
            {
                if (roomNode.rect.Contains(e.mousePosition))
                {
                    return roomNode;
                }
            }
            return null;
        }


        private void HandleMouseDownEven(Event e)
        {

           if (e.button == 1) // Right click
            {
                ShowContextMenu(e.mousePosition);
            }
        }

        /// <summary>
        /// Create custom context menus and dropdown menus
        /// </summary>
        /// <param name="mousePosition"></param>
        private void ShowContextMenu(Vector2 mousePosition)
        {
            GenericMenu contextMenu = new GenericMenu(); 
            contextMenu.AddItem(new GUIContent("Add Room Node"), false, () => { AddRoomNode(mousePosition); });
            contextMenu.ShowAsContext();
        }

        /// <summary>
        /// Create a new room node in the graph
        /// </summary>
        /// <param name="mousePositionObj">Mouse position</param>
        private void AddRoomNode(object mousePositionObj)
        {
            AddRoomNode(mousePositionObj, _roomNodeTypeList.typeList.Find(t => t.isNone));
        }

        /// <summary>
        /// Create a new room node in the graph - Overload
        /// </summary>
        /// <param name="mousePositionObj">Mouse position</param>
        /// <param name="roomNodeType">Type of room to create</param>
        private void AddRoomNode(object mousePositionObj, RoomNodeTypeSO roomNodeType)
        {
            Vector2 mousePos = (Vector2)mousePositionObj; 
            RoomNodeSO newRoomNode = ScriptableObject.CreateInstance<RoomNodeSO>(); 
            _roomNodeGraph.roomNodeList.Add(newRoomNode);
            newRoomNode.Initialize(new Rect(mousePos, new Vector2(_nodeWidth, _nodeHeight)), _roomNodeGraph, roomNodeType);

            AssetDatabase.AddObjectToAsset(newRoomNode, _roomNodeGraph); //Scriptable object can be saved in another scriptable object
            AssetDatabase.SaveAssets();
        }

        private void DrawRoomNodes()
        {
                foreach (RoomNodeSO roomNode in _roomNodeGraph.roomNodeList)
                {
                    roomNode.Draw(_nodeStyle);
                }

            GUI.changed = true;
        }
    }
}
