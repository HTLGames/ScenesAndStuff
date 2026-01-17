# ScenesAndStuff
Package to handle additive scene loading in unity. Perfect if you need to handle multiple permanent scenes across all your project, or only on certain sections of it.

<h2>Getting started</h2>
First of all, all scenes you add must be in the Build Profiles scenes list (which is not very convenient but it's how it is).
You can create a SceneCollection asset from the project window at Create -> HTL -> Scenes -> Scene Collection
<img width="843" height="613" alt="imagen" src="https://github.com/user-attachments/assets/bc394b6c-ad21-4eba-8c03-c7eb5d47073b" />

Once you have a Scene Collection you have two lists:
 * <b>Permanent Scenes:</b> All scenes which won't be unloaded at any time when using the Scene Collection
 * <b>Scene Gropus:</b> Groups of scenes, which are identified by the active scene and a set of scenes that will be loaded whith that scene. All scene groups are identified by the active scene.
<img width="485" height="673" alt="imagen" src="https://github.com/user-attachments/assets/8a0bafe4-54e7-4d69-b9d0-d275b0b50069" />
You can have as many Scene Collections as you want and add them to the SceneLoader component to be able to use it.
<img width="483" height="189" alt="imagen" src="https://github.com/user-attachments/assets/992186b2-cac0-4e1d-9482-13696ae708fb" />

<h2>Scene Loader</h2>
This page is not complete, give me some more time and i'll correctly doccument the page. 
If you don't enable initializeOnAwake you will have to initialize the scene loader manually with the Initialize() method.
The default scene collection is the first element of the list.
Use LoadScene(SceneObject activeScene) to load a certain scene group given the active scene that identifies it. There is an asyncronous variant of this method for asyncronous workflow.
Use SetSceneCollection(SceneCollectoin collection) to change the scene collection to use when loading a scene, it's still work in progress.
The scene collection list is accesible and read only for now.
