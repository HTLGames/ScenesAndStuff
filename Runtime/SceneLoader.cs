using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HTL.ScenesAndStuff
{
    public class SceneLoader : MonoBehaviour
    {
        [field: SerializeField] public SceneCollection[] sceneCollections { get; private set; }
        private Dictionary<string, SceneGroup> currentGroup = new Dictionary<string, SceneGroup>();
        private List<SceneObject> permanentScenes;

        private bool initialized = false;
        private class NotInitializedException : Exception {}
        private class MissingGroupException : Exception {}


        public void Initialize()
        {
            if (sceneCollections.Length == 0)
            {
                Debug.LogError("No scene collections added to the list");
            }

            SetSceneCollection(sceneCollections[0]);
            initialized = true;
        }


        public void SetSceneCollection(SceneCollection collection)
        {
            if (!initialized) throw new NotInitializedException();

            permanentScenes = collection.permanentScenes;

            currentGroup.Clear();
            foreach (SceneGroup s in collection.sceneGroups)
            {
                currentGroup.Add(s.activeScene, s);
            }
        }

        /// <summary>
        /// Loads a given scene.
        /// </summary>
        /// <param name="scene"></param>
        public void LoadScene(SceneObject scene)
        {
            // Error handling
            if (!initialized) throw new NotInitializedException();
            if (!currentGroup.ContainsKey(scene)) throw new MissingGroupException();

            LoadSceneAsync(scene).GetAwaiter();
        }

        public async Awaitable LoadSceneAsync(SceneObject scene)
        {
            // Error handling
            if (!initialized) throw new NotInitializedException();
            if (!currentGroup.ContainsKey(scene)) throw new MissingGroupException();

            // Get reference to the scene group
            SceneGroup group; 
            currentGroup.TryGetValue(scene, out group);

            // Unload unnecesary scenes
            UnloadUnnecessaryScenes();

            // Load all scenes in order
            foreach (SceneObject s in group.scenes)
            {
                if (!IsSceneLoaded(s))
                {
                    await SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive);
                    if (s == (string)group.activeScene)
                    {
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName(s));
                    }
                }
            }
        }

        private bool IsScenePermanent(SceneObject scene) => permanentScenes.Contains(scene);

        private void UnloadUnnecessaryScenes()
        {
            for (int i = 0; i < SceneManager.loadedSceneCount; ++i)
            {
                string scene = SceneManager.GetSceneAt(i).name;
                if (!IsScenePermanent(name))
                {
                    SceneManager.UnloadSceneAsync(scene).GetAwaiter();
                }
            }
        }

        /// <summary>
        /// Checks whether a certain scene is currently loaded or not.
        /// </summary>
        /// <param name="scene">Scene to check.</param>
        /// <returns>True if scene is loaded or currently loading.</returns>
        public static bool IsSceneLoaded(string scene)
        {
            for (int i = 0; i < SceneManager.loadedSceneCount; ++i)
            {
                if (SceneManager.GetSceneAt(i).name == scene)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
