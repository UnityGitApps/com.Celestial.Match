using Infrastructure.Services;
using Infrastructure.Settings;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class EditorTools
    {
        [MenuItem("Tools/Clear saves")]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
        
        [MenuItem("Tools/Unlock levels")]
        public static void UnlockLevels()
        {
            var guids = AssetDatabase.FindAssets("t:CommonSettings");
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var commonSettings = AssetDatabase.LoadAssetAtPath<CommonSettings>(path);
            
            SaveLoadService.UnlockAllLevels(commonSettings.LevelsCount);
        }

        [MenuItem("Tools/Show progress")]
        public static void ShowProgress()
        {
            SaveLoadService.ShowSavedProgress();
        }
    }
}