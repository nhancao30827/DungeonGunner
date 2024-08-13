using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungGunCore
{
    [CreateAssetMenu(fileName = "RoomNodeType", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
    public class RoomNodeTypeSO : ScriptableObject
    {
        public string roomNodeTypeName;

        [Header("Select node type")]
        [Space(10)]

        public bool displayInEditor = true;

        public bool cooridor;

        public bool cooridorHorizontal;

        public bool cooridorVertical;

        public bool entrance;

        public bool bossRoom;

        public bool none;

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
