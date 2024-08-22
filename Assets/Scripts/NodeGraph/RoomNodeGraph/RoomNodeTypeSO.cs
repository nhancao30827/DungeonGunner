using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungGunCore
{
    [CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
    public class RoomNodeTypeSO : ScriptableObject
    {
        public string roomNodeTypeName;

        public bool displayInEditor = true;

        [Header("Select node type")]
        [Space(10)]

        public bool isCorridor;

        public bool isCorridorHorizontal;

        public bool isCorridorVertical;

        public bool isEntrance;

        public bool isBossRoom;

        public bool isNone;

        #region CHECK VALID
#if UNITY_EDITOR  //Make codes in this #if wont be compiled when game build
        private void OnValidate()  // Excecute on value changes
        {
            HelperUtilities.CheckEmptyString(this, "Room Node Type Name", roomNodeTypeName);
        }
#endif 
        #endregion
    }
}
