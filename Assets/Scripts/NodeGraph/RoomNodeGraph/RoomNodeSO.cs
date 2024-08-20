using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungGunCore
{
    public class RoomNodeSO : ScriptableObject
    {
         public string id;
         public List<string> parentsID= new List<string>();
         public List<string> childrenID = new List<string>();
        [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
        [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
        [HideInInspector] public Rect rect;
        [HideInInspector] public bool isSelected = false;
        [HideInInspector] public bool isDragging = false;

        public RoomNodeTypeSO roomNodeType;


        /// <summary>
        /// Create a new node
        /// </summary>
        /// <param name="rect">Positon and size of the node</param>
        /// <param name="roomNodeGraph">Graph contains this node</param>
        /// <param name="roomNodeType">Type of room node</param>
        public void Initialize(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
        {
            this.id = System.Guid.NewGuid().ToString(); // Generate a unique ID
            this.rect = rect;
            this.name = "Room Node";
            this.roomNodeGraph = roomNodeGraph;
            this.roomNodeType = roomNodeType;
            this.roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }

        /// <summary>
        /// Draw the node
        /// </summary>
        /// <param name="style">Defined style in graph editor script</param>
        public void Draw(GUIStyle style)
        {
            GUILayout.BeginArea(rect, style);
            EditorGUI.BeginChangeCheck();

            if (parentsID.Count > 0 || this.roomNodeType.isEntrance)
            {
                EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
            }

            else
            {
                int selected = roomNodeTypeList.typeList.FindIndex(t => t == roomNodeType);
                int selection = EditorGUILayout.Popup(selected, roomNodeTypeList.typeList.ConvertAll(t => t.roomNodeTypeName).ToArray());
                roomNodeType = roomNodeTypeList.typeList[selection];
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// Handle events emitted by the node
        /// </summary>
        /// <param name="e"></param>
        public void HandleNodeEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    HandleMouseDownEvent(e);
                    break;

                case EventType.MouseUp:
                    HandleMouseUpEvent();
                    break;

                case EventType.MouseDrag:
                    HandleMouseDragEvent(e);
                    break;
            }
        }

        private void HandleMouseDownEvent(Event e)
        {      
            if (e.button == 0 ) // Left click
            {
                Selection.activeObject = this;
                isSelected = !isSelected;
            }

            if (e.button == 1) // Right click
            {
                roomNodeGraph.SetNodeConnectionLine(this, e.mousePosition);
            }
        }

        private void HandleMouseUpEvent()
        {
            if (isDragging == true)
            {
                isDragging = false;
            }
        }

        private void HandleMouseDragEvent(Event e)
        {
                rect.position += e.delta;
                isDragging = true;
                e.Use();
                EditorUtility.SetDirty(this);
        }

        


        public bool AddChildID(string childID)
        {
            if (IsChildValid(childID) == true)
            {
                Debug.Log("Child added");
                childrenID.Add(childID);
                return true;
            }
  
            return false;
        }

        public bool IsChildValid(string childID)
        {
            bool isConnectedBossNodeAlready = false;

            foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
            {
                if (roomNode.roomNodeType.isBossRoom && roomNode.childrenID.Count > 0)
                {
                    isConnectedBossNodeAlready = true;
                }
            }

            if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
            {
                    return false;
            }

            if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone || roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            {
                    return false;
            }

            if (childrenID.Contains(childID) || parentsID.Contains(childID))
            {
                    return false;
            }

            if (id == childID || roomNodeGraph.GetRoomNode(childID).parentsID.Count > 0)
            {
                    return false;
            }

            if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor)
            {
                if (roomNodeType.isCorridor || childrenID.Count >= Settings.maxChildCorridors)
                    {
                        return false;
                    }
            }

            if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor)
            {
                if (!roomNodeType.isCorridor || childrenID.Count > 0)
                {
                        return false;
                }
            }
            
            return true;
        }

        public bool AddParentID(string parentId)
        {
            if (!parentsID.Contains(parentId))
            {
                parentsID.Add(parentId);
                return true;
            }

            Debug.Log("Parent already exists");
            return false;
        }
    }
}
