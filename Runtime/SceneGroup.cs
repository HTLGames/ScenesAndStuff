
using System.Collections.Generic;
using UnityEngine;


namespace HTL.ScenesAndStuff
{
    [System.Serializable]
    public class SceneGroup
    {
        [field: SerializeField] public SceneObject activeScene { get; private set; }
        [field: SerializeField] public List<SceneObject> scenes { get; private set; }
        internal bool activeSceneInList => scenes.Exists(s => s == (string)activeScene); 

        public static bool operator ==(SceneGroup a, SceneGroup b)
        {
            return a.activeScene == b.activeScene;
        }

        public static bool operator !=(SceneGroup a, SceneGroup b)
        {
            return a.activeScene != b.activeScene;
        }

        public override bool Equals(object obj)
        {
            if (obj is not SceneGroup) return false;

            return ((SceneGroup)obj).activeScene == activeScene;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #if UNITY_EDITOR
        internal void OnValidate() {
            if (scenes == null) return;

            if (activeScene != "" && !activeSceneInList)
            {
                scenes.Add(activeScene);
            }

            if (scenes.Count != 0)
            {
                List<string> names = new List<string>();
                for (int i = 0; i < scenes.Count; ++i){
                    if (scenes[i] != null && names.Contains(scenes[i]))
                    {
                        scenes[i] = null;
                    }
                    else
                    {
                        names.Add((string)scenes[i]);
                    } 
                }
            }
        }
        #endif
    }
}
