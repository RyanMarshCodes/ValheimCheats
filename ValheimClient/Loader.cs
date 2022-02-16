using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ValheimClient
{
    public class Loader
    {
        public static GameObject objLoad;

        public static void Load()
        {
            objLoad = new GameObject();
            objLoad.AddComponent<Main>();
            // objLoad.AddComponent<WaeModMenu>();
            UnityEngine.Object.DontDestroyOnLoad(objLoad);
        }

        public static void Unload()
        {
            _Unload();
        }

        private static void _Unload()
        {
            GameObject.Destroy(objLoad);
        }
    }
}
