using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HTL.ScenesAndStuff
{
    /// <summary>
    /// Holds a list of scene collections assets to load scene groups under different configurations.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool initializeOnAwake = true;

        // TODO: There is nothing stopping you from adding one sceneCollection twice, maybe we should do somethiing about it...
        [field: SerializeField] internal SceneCollection[] sceneCollections { get; private set; }
        private Dictionary<string, SceneGroup> currentGroup = new Dictionary<string, SceneGroup>();
        private List<SceneObject> permanentScenes;

        private bool initialized = false;
        private class NotInitializedException : Exception {}
        private class MissingGroupException : Exception {}
        private class EmptyGroupException : Exception
        {
            public EmptyGroupException(string msg) : base(msg) {}
        }

        void Awake()
        {
            if (initializeOnAwake) Initialize();
        }

        /// <summary>
        /// Initializes the scene loader script.<br/>
        /// The first collection to load will always be at index 0
        /// </summary>
        public void Initialize()
        {
            if (initialized)
            {
                Debug.LogError("<b>You are initialising the SceneLoader twice</b><br/>Maybe you "
                            +   "should have a look at the <i>Initialize On Awake</i> field on "
                            +   "your SceneLoader if you want to initialize manually.");
                return;
            }

            if (sceneCollections.Length == 0)
            {
                Debug.LogError("No scene collections added to the list");
                return;
            }

            initialized = true;
            SetSceneCollection(sceneCollections[0]);
        }

        public void SetSceneCollection(int index)
        {
            SetSceneCollection(sceneCollections[index]);
        }


        public void SetSceneCollection(SceneCollection collection)
        {
            if (!initialized) throw new NotInitializedException();

            permanentScenes = collection.permanentScenes;

            currentGroup.Clear();
            foreach (SceneGroup s in collection.sceneGroups)
            {
                // We don't want nulls here >:(
                if (string.IsNullOrEmpty(s.activeScene)) 
                    throw new EmptyGroupException($"The active scene in a group of {collection.name} has not been assigned.");

                currentGroup.Add(s.activeScene, s);
            }
        }

        /// <summary>
        /// Loads a group associated with the scene. <br/>
        /// The provided scene will be marked as active.
        /// </summary>
        /// <param name="scene">Active scene of the group.</param>
        public void LoadScene(string scene)
        {
            // Error handling
            if (!initialized) throw new NotInitializedException();
            if (!currentGroup.ContainsKey(scene)) throw new MissingGroupException();

            LoadSceneAsync(scene).GetAwaiter();
        }

        // /// <summary>
        // /// Loads a group associated with the scene. <br/>
        // /// The provided scene will be marked as active. <br/>
        // /// </summary>
        // /// <param name="scene">Active scene of the group.</param>
        // public async Awaitable LoadSceneAsync(SceneObject scene)
        // {
        //     await LoadSceneAsync((string)scene);
        // }

        /// <summary>
        /// Loads a group associated with the scene. <br/>
        /// The provided scene will be marked as active. <br/>
        /// </summary>
        /// <param name="scene">Active scene of the group.</param>
        /// <exception cref="NotInitializedException">The scene was not initialized</exception>
        /// <exception cref="MissingGroupException">The scene you are trying to load isn't added to the group</exception>
        public async Awaitable LoadSceneAsync(string scene)
        {
            // Error handling
            if (!initialized) throw new NotInitializedException();
            if (!currentGroup.ContainsKey(scene)) throw new MissingGroupException();

            // Get reference to the scene group
            SceneGroup group; 
            currentGroup.TryGetValue(scene, out group);

            // Unload unnecesary scenes
            await UnloadUnnecessaryScenes();

            // Load permanent scenes (if not loaded yet)
            foreach (SceneObject s in permanentScenes)
            {
                if (!IsSceneLoaded(s))
                {
                    await SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive);
                }
            }

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

        private bool IsScenePermanent(string scene) => permanentScenes.Exists(s => s == scene);

        private async Awaitable UnloadUnnecessaryScenes()
        {
            List<string> scenesToUnload = new List<string>(SceneManager.loadedSceneCount);
            for (int i = 0; i < SceneManager.loadedSceneCount; ++i)
            {
                if (!IsScenePermanent(SceneManager.GetSceneAt(i).name))
                {
                    scenesToUnload.Add(SceneManager.GetSceneAt(i).name);
                }
            }

            foreach(string s in scenesToUnload)
            {
                await SceneManager.UnloadSceneAsync(s);
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
