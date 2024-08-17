using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungGunCore
{
    [CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
    public class RoomNodeGraphSO : ScriptableObject
    {
        [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
        [HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
        [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDict = new Dictionary<string, RoomNodeSO>();

        [HideInInspector] public RoomNodeSO startNode;
        [HideInInspector] public Vector2 endOfLinePosition;

        private void Awake()
        {
            LoadNodeDictionary();
        }

        private void LoadNodeDictionary()
        {
            roomNodeDict.Clear();

            foreach (RoomNodeSO node in roomNodeList)
            {
                roomNodeDict.Add(node.id, node);
            }
        }

        public void SetNodeConnectionLine(RoomNodeSO startNode, Vector2 endOfLine)
        {
            this.startNode = startNode;
            this.endOfLinePosition = endOfLine;
        }

        //Reload the dictionary when the node list is changed in the editor
        public void OnValidate()
        {
            LoadNodeDictionary();
        }
    }
}
