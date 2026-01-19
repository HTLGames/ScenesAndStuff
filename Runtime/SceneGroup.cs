
using System.Collections.Generic;
using UnityEngine;


namespace HTL.ScenesAndStuff
{
    [System.Serializable]
    internal class SceneGroup
    {
        [field: SerializeField] public SceneObject activeScene { get; private set; }
        [field: SerializeField] public List<SceneObject> scenes { get; private set; }
        internal bool activeSceneInList => scenes.Exists(s => s == (string)activeScene); 

        public static bool operator ==(SceneGroup a, SceneGroup b)
        {
            return (string)a.activeScene == (string)b.activeScene;
        }

        public static bool operator !=(SceneGroup a, SceneGroup b)
        {
            return (string)a.activeScene != (string)b.activeScene;
        }

        public override bool Equals(object obj)
        {
            if (obj is not SceneGroup) return false;

            return (string)((SceneGroup)obj).activeScene == (string)activeScene;
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
                // Replace first empty space with active scene 
                bool replace = false;
                for (int i = 0; i < scenes.Count; ++i)
                {
                    if (string.IsNullOrEmpty(scenes[i]))
                    {
                        scenes[i] = activeScene;
                        replace = true;
                    }
                }

                // Add empty scene if there were no empty spaces
                if (!replace) scenes.Add(activeScene);
            }

            if (scenes.Count != 0)
            {
                // Check for duplicate scenes
                List<string> names = new List<string>();
                for (int i = 0; i < scenes.Count; ++i){

                    // Ignore unassigned scenes
                    if (string.IsNullOrEmpty(scenes[i])) continue;

                    if (scenes[i] != null && names.Contains(scenes[i]))
                    {
                        Debug.LogWarning($"There is already a {(string)scenes[i]} scene added to the list.");
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
