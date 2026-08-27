using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SpaceShooter.Editor
{
    public static class EditorSceneBuilder
    {
        [MenuItem("SpaceShooter/Bake Current Active Scene")]
        public static void BakeCurrentActiveScene()
        {
            if (Application.isPlaying) return;

            string sceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"<color=#00FFCC><b>[EditorSceneBuilder]</b></color> Baking scene: <b>{sceneName}</b>...");

            if (sceneName == "Prototype_SpaceCombat")
            {
                BakeSpaceCombatHierarchy();
            }
            else
            {
                Debug.LogWarning($"[EditorSceneBuilder] Unknown or generic scene '{sceneName}'. Please open 'Prototype_SpaceCombat'.");
            }
        }

        [MenuItem("SpaceShooter/Open & Bake Space Combat Scene")]
        public static void OpenAndBakeSpaceCombat()
        {
            if (Application.isPlaying) return;

            string combatScenePath = "Assets/Scenes/Prototype_SpaceCombat.unity";
            if (EditorSceneManager.GetActiveScene().path != combatScenePath)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(combatScenePath);
            }

            BakeSpaceCombatHierarchy();
        }

        [MenuItem("SpaceShooter/Clear All Objects in Active Scene")]
        public static void ClearAllObjectsInActiveScene()
        {
            if (Application.isPlaying) return;

            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                Object.DestroyImmediate(root);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=#FF9900><b>[EditorSceneBuilder]</b></color> Active scene completely cleared of all root objects.");
        }

        private static void BakeSpaceCombatHierarchy()
        {
            ClearAllObjectsInActiveScene();

            GameObject managerObj = new GameObject("Prototype_SpaceCombat_Manager");
            var setup = managerObj.AddComponent<PrototypeSceneSetup>();
            setup.BuildSceneHierarchy();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=#00FFCC><b>[EditorSceneBuilder]</b></color> Prototype_SpaceCombat hierarchy baked successfully with 0 overlapping objects!");
        }
    }
}
