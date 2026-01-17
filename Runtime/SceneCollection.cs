using System.Collections.Generic;
using UnityEngine;

namespace HTL.ScenesAndStuff
{
    [CreateAssetMenu(fileName = "SceneCollectionData", menuName = "HTL/Scenes/SceneCollection")]
    public class SceneCollection : ScriptableObject
    {
        [field: SerializeField] public List<SceneObject> permanentScenes { get; private set; }
        [field: SerializeField] public List<SceneGroup> sceneGroups { get; private set; }

        // TODO: Make sure there can't be duplicated scene groups in a single scene group

#if UNITY_EDITOR
        void OnValidate()
        {
            if (sceneGroups != null)
            {
                foreach (SceneGroup g in sceneGroups)
                {
                    g.OnValidate();
                }
            }
        }
#endif
    }
}
