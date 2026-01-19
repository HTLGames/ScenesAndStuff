using System.Collections.Generic;
using UnityEngine;

namespace HTL.ScenesAndStuff
{
    [CreateAssetMenu(fileName = "SceneCollectionData", menuName = "HTL/Scenes/SceneCollection")]
    public class SceneCollection : ScriptableObject
    {
        [field: SerializeField] public List<SceneObject> permanentScenes { get; private set; }
        [field: SerializeField] internal List<SceneGroup> sceneGroups { get; private set; }

        // TODO: Make sure there can't be duplicated scene groups in a single scene group

#if UNITY_EDITOR
        void OnValidate()
        {
            if (sceneGroups != null && sceneGroups.Count != 0)
            {
                // Check for duplicates in groups
                List<SceneGroup> groups = new List<SceneGroup>();
                for (int i = 0; i < sceneGroups.Count; ++i)
                {

                    // Ignore unassigned groups
                    if (string.IsNullOrEmpty(sceneGroups[i].activeScene)) continue;

                    if (groups.Exists(s => s == sceneGroups[i]))
                    {
                        Debug.LogWarning($"There is already a group with {(string)sceneGroups[i].activeScene} as active scene.");
                        sceneGroups[i] = null;
                    }
                    else
                    {
                        sceneGroups[i].OnValidate();
                        groups.Add(sceneGroups[i]);
                    }
                }
            }
        }
#endif
    }
}
