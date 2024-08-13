using System.Collections;
using System.Collections.Generic;
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

        public RoomNodeTypeSO roomNodeType;
    }
}
