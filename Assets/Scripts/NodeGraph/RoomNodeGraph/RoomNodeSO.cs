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
    }
}
