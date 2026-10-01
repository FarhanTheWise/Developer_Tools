using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static System.IO.Directory;
using static UnityEditor.AssetDatabase;

namespace FarhanAfzal
{

    public static class ToolsMenu
    {
        private static List<FolderStructureCreator> folderStructureCreators = new()
        {
            new() {
                rootFolderName = "Project Files",
                subFolders = new List<string>
                {
                    "Scripts",
                    "Prefabs",
                    "Materials",
                    "Textures",
                    "Audio",
                    "Animations",
                    "Fonts",
                    "ScriptableObjects",
                }
            },
            new() {
                rootFolderName = "Scenes",
                subFolders = new List<string>
                {
                    "MainMenu",
                    "Environments",
                    "ControlScene",
                    "UIScenes"
                }
            },
            new() {
                rootFolderName = "Plugins",
                subFolders = new List<string>()
            },
            new() {
                rootFolderName = "GameAssets",
                subFolders = new List<string>()
                {
                    "Models",
                    "UI",
                    "AudioAssets"
                }
            },
        };

        [MenuItem("MyTools/Create Folder Structure")]
        public static void CreateFolderStructure()
        {
            var rootPath = Application.dataPath;
            foreach (var folderStructureCreator in folderStructureCreators)
            {
                var rootFolderPath = Path.Combine(rootPath, folderStructureCreator.rootFolderName);
                CreateDirectory(rootFolderPath);

                foreach (var subFolder in folderStructureCreator.subFolders)
                {
                    var subFolderPath = Path.Combine(rootFolderPath, subFolder);
                    CreateDirectory(subFolderPath);
                }
            }

            Refresh();
        }
    }
}

public class FolderStructureCreator
{
    public string rootFolderName;
    public List<string> subFolders;
}
