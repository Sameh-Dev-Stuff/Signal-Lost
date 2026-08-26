using System;
using UnityEngine;

namespace MyUtilities
{
    public class Utilities : MonoBehaviour
    {
        private static float time; 
        
        public static void RepeatAction(float repeatTime,Action fanc)
        {
            time += Time.deltaTime;
            if (time >= repeatTime)
            {
                fanc();
                time = 0f;
            }
        }
    }
}