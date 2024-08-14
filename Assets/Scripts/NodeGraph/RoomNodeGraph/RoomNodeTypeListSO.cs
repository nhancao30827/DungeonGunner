using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungGunCore
{
    [CreateAssetMenu(fileName = "RoomNodeTypeList", menuName = "Scriptable Objects/Dungeon/Room Node Type List")]
    public class RoomNodeTypeListSO : ScriptableObject
    {
        [Tooltip("Store all room node types in the game")]
        public List<RoomNodeTypeSO> typeList;
    }
}
