using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DungGunCore
{
    public class RoomNodeSO : ScriptableObject
    {
        public string id;
        public List<string> parentsID = new List<string>();
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

            if (parentsID.Count > 0 || roomNodeType.isEntrance)
            {               
                EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
            }
            else
            {
                int selected = roomNodeTypeList.typeList.FindIndex(t => t == roomNodeType);
                //int selection = EditorGUILayout.Popup(selected, roomNodeTypeList.typeList.FindAll(t => t.displayInEditor).ConvertAll(t => t.roomNodeTypeName).ToArray());
                int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
                roomNodeType = roomNodeTypeList.typeList[selection];

                bool isCorridorChanged = roomNodeTypeList.typeList[selected].isCorridor != roomNodeTypeList.typeList[selection].isCorridor;
                bool isBossRoomChanged = !roomNodeTypeList.typeList[selected].isBossRoom && roomNodeTypeList.typeList[selection].isBossRoom;
                
                if (isCorridorChanged || isBossRoomChanged)
                {
                    RemoveChildParentLinks();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }
            GUILayout.EndArea();
        }

        public string[] GetRoomNodeTypesToDisplay()
        {
            string[] roomArray = new string[roomNodeTypeList.typeList.Count];

            for (int i = 0; i < roomNodeTypeList.typeList.Count; i++)
            {
                if (roomNodeTypeList.typeList[i].displayInEditor)
                {
                    roomArray[i] = roomNodeTypeList.typeList[i].roomNodeTypeName;
                }
            }

            return roomArray;
        }

        private void RemoveChildParentLinks()
        {
            if (childrenID.Count > 0)
            {
                for (int i = childrenID.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO childNode = roomNodeGraph.GetRoomNode(childrenID[i]);

                    if (childNode != null)
                    {
                        childNode.parentsID.Remove(id);
                        childrenID.Remove(childrenID[i]);
                    }
                }

            }
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
            if (e.button == 0) // Left click
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

        public bool AddParentID(string parentID)
        {
            if (!parentsID.Contains(parentID))
            {
                parentsID.Add(parentID);
                return true;
            }

            return false;
        }

    }
}
