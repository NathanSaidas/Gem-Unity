using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Gem
{
    /// <summary>
    /// Build Manager allows custom management of scene data.
    /// - Adding and Removing Custom scenes
    /// - Opening Scenes / Saving scenes from a separate window
    /// - Saving Custom Build Settings
    /// 
    /// Every Action done in the Build Manager will 
    /// </summary>
    public class BuildManager : EditorWindow
    {
        [MenuItem(EditorConstants.BUILD_MANAGER_MENU_ITEM)]
        static void CreateWindow()
        {
            BuildManager buildManager = GetWindow<BuildManager>(EditorConstants.BUILD_MANAGER_NAME);

        }

        private List<Scene> m_GameScenes = new List<Scene>();
        private List<EditorBuildSettingsScene> m_BuildScenes = new List<EditorBuildSettingsScene>();

        private void OnFocus()
        {
            
        }
        private void OnLostFocus()
        {

        }

        private void LoadScenes()
        {

        }
        private void SaveScenes()
        {
            
        }
    }

}

