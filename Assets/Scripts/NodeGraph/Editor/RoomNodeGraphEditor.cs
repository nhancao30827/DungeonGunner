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
        private GUIStyle _selectedNodeStyle;
        
        private static RoomNodeGraphSO _roomNodeGraph;
        private RoomNodeSO _hoverRoomNode;
        private RoomNodeTypeListSO _roomNodeTypeList;
        
        private float _connectionLineWidth = 3f;

        #region NODE LAYOUTS
        private const float _nodeWidth = 160f;
        private const float _nodeHeight = 75f;
        private const int _nodePadding = 25;
        private const int _nodeBorder = 12;
        #endregion

        [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
        
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
            
            return false;
        }

        // Called when the window is opened
        private void OnEnable() 
        {
            SetUpNodesStyle();

            Selection.selectionChanged += InspectorSelectionChange;
            _roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= InspectorSelectionChange;
        }

        private void SetUpNodesStyle()
        {
            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
            _nodeStyle.normal.textColor = Color.white;
            _nodeStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);
            _nodeStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);


            _selectedNodeStyle = new GUIStyle();
            _selectedNodeStyle.normal.background = EditorGUIUtility.Load("node2 on") as Texture2D;
            _selectedNodeStyle.normal.textColor = Color.white;
            _selectedNodeStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);
            _selectedNodeStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);
        }

        // Handle creating nodes
        private void OnGUI()
        {

           if (_roomNodeGraph != null)
           {
               DrawDraggedLine();
               HandleEventDetection(Event.current);
               SetupNodesConnection();
               DrawRoomNodes();
           }

           if (GUI.changed)
           {
                Repaint();
           }
        }

        private void DrawDraggedLine()
        {
            if (_roomNodeGraph.endOfLinePosition != Vector2.zero)
            {
                Handles.DrawBezier(
                    _roomNodeGraph.startNode.rect.center,
                    _roomNodeGraph.endOfLinePosition,
                    _roomNodeGraph.startNode.rect.center,
                    _roomNodeGraph.endOfLinePosition,
                    Color.white,
                    null,
                    _connectionLineWidth
                );
            }
        }

        #region Graph Event
        private void HandleEventDetection(Event e)
        {
            if (_hoverRoomNode == null || _hoverRoomNode.isDragging == false)
            {
                _hoverRoomNode = HandleMouseOverNode(e);
            }

            if (_hoverRoomNode == null || _roomNodeGraph.startNode != null)
            {
                HandleGraphEvents(e);
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
        private void HandleGraphEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    HandleMouseDownEvent(e);
                    break;

                case EventType.MouseUp:
                    HandleMouseUpEvent(e);
                    break;

                case EventType.MouseDrag:
                    HandleMouseDragEvent(e);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Check if a node is hovered by the mouse cursor
        /// </summary>
        /// <param name="e"></param>
        /// <returns>The node which is hovered</returns>
        private RoomNodeSO HandleMouseOverNode(Event e)
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


        private void HandleMouseDownEvent(Event e)
        {

            if (e.button == 1) // Right click
            {
                ShowContextMenu(e.mousePosition);
            }
        }

        private void HandleMouseDragEvent(Event e)
        {
            if (e.button == 1)
            {
                HandleRightMouseDrag(e);
            }
        }

        private void HandleMouseUpEvent(Event e)
        {
            if (e.button == 1 && _roomNodeGraph.startNode != null)
            {
                RoomNodeSO roomNode = HandleMouseOverNode(e);  //Retrieve the node under the mouse cursor when the button is released

                if (roomNode != null && roomNode != _roomNodeGraph.startNode)
                {
                    if (_roomNodeGraph.startNode.AddChildID(roomNode.id) == true)
                    {
                        roomNode.AddParentID(_roomNodeGraph.startNode.id);
                    }
                }

                ClearDraggedLine();
            }
        }

        private void HandleRightMouseDrag(Event e)
        {
            if (_roomNodeGraph.startNode != null)
            {
                _roomNodeGraph.endOfLinePosition += e.delta;
                GUI.changed = true;
            }
        } 
        #endregion


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
            if (_roomNodeGraph.roomNodeList.Count == 0)
            {
                AddRoomNode(new Vector2(200f, 200f), _roomNodeTypeList.typeList.Find(x => x.isEntrance));
            }

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

            _roomNodeGraph.OnValidate();
        }

        /// <summary>
        /// Undo the unconnected dragged line when the mouse is released.
        /// </summary>
        private void ClearDraggedLine()
        {
            _roomNodeGraph.startNode = null;
            _roomNodeGraph.endOfLinePosition = Vector2.zero;
            GUI.changed = true;
        }

        /// <summary>
        ///  
        /// </summary>
        private void SetupNodesConnection()
        {
            foreach (RoomNodeSO parentNode in _roomNodeGraph.roomNodeList)
            {
                foreach (string childID in parentNode.childrenID)
                {
                    RoomNodeSO childNode = _roomNodeGraph.roomNodeDict[childID];
                    DrawConnectionLLine(parentNode, childNode);
                }
            }
        }

        /// <summary>
        /// Create bezier curves between two nodes
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="childNode"></param>
        private void DrawConnectionLLine(RoomNodeSO parentNode, RoomNodeSO childNode)
        {
            // Draw a bezier curve between two nodes
            Vector2 startPosition = parentNode.rect.center;
            Vector2 endPosition = childNode.rect.center;
            
            Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, _connectionLineWidth);
            

            // Draw an arrow in the middle of the bezier curve
            Vector2 midPosition = (endPosition + startPosition) / 2f;
            Vector2 direction = endPosition - startPosition;
            Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * _connectionLineWidth * 2;
            Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * _connectionLineWidth * 2;
            Vector2 arrowHeadPoint = midPosition + direction.normalized * _connectionLineWidth * 2;

            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, _connectionLineWidth);
            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, _connectionLineWidth);

            GUI.changed = true;
        }

        private void InspectorSelectionChange()
        {
            RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

            if (roomNodeGraph != null)
            {
                _roomNodeGraph = roomNodeGraph;
                 GUI.changed = true;
            }

        }

        private void DrawRoomNodes()
        {
            foreach (RoomNodeSO roomNode in _roomNodeGraph.roomNodeList)
            {
                 if (roomNode.isSelected == true)
                 {
                     roomNode.Draw(_selectedNodeStyle);
                        
                 }

                 else
                 {
                     roomNode.Draw(_nodeStyle);
                 }
            }

            GUI.changed = true;
        }
    }
}
