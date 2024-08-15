using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungGunCore
{
    public class RoomNodeSO : ScriptableObject
    {
        [HideInInspector] public string id;
        [HideInInspector] public List<string> parentsID= new List<string>();
        [HideInInspector] public List<string> childrenID = new List<string>();
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

            roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }

        /// <summary>
        /// Draw the node
        /// </summary>
        /// <param name="style">Defined style in graph editor script</param>
        public void Draw(GUIStyle style)
        {
            GUILayout.BeginArea(rect, style);
            EditorGUI.BeginChangeCheck();

            int selected = roomNodeTypeList.typeList.FindIndex(t => t == roomNodeType);
            int selection = EditorGUILayout.Popup(selected, roomNodeTypeList.typeList.ConvertAll(t => t.roomNodeTypeName).ToArray());
            roomNodeType = roomNodeTypeList.typeList[selection];

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
            Selection.activeObject = this;

            if (e.button == 0 ) // Left click
            {
                isSelected = !isSelected;
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
    }
}
