using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungGunCore
{
    [CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
    public class RoomNodeGraphSO : ScriptableObject
    {
        [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
        [HideInInspector] public List<RoomNodeTypeSO> roomNodeTypes = new List<RoomNodeTypeSO>();
        [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDict = new Dictionary<string, RoomNodeSO>();
    }
}
