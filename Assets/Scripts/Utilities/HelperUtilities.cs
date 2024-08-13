using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungGunCore
{
    
    public static class HelperUtilities 
    {
        /// <summary>
        /// Check empty string fields in an object.
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="fieldName"></param>
        /// <param name="stringCheck"></param>
        /// <returns></returns>
        public static bool CheckEmptyString(Object thisObj, string fieldName, string stringCheck)
        {

           if (string.IsNullOrEmpty(stringCheck)) {
                Debug.LogError("Field " + fieldName + " in " + thisObj.name + " is empty.");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check empty enumerable variable in an object.
        /// </summary>
        /// <param name="thisObj"></param>
        /// <param name="fieldName"></param>
        /// <param name="enumCheck"></param>
        /// <returns></returns>
        public static bool CheckEmptyEnum(Object thisObj, string fieldName, IEnumerable enumCheck)
        {
            bool error = false;
            int count = 0;  

            foreach (var item in enumCheck)
            {
                if (item == null)
                {
                    Debug.LogError("Field " + fieldName + " in " + thisObj.name + " is empty.");
                    error = true;
                }

                else count++;
            }

            if (count == 0)
            {
                Debug.LogError("Field " + fieldName + " in " + thisObj.name + " is empty.");
                error = true;
            }

            return error;
        }
    }


}
