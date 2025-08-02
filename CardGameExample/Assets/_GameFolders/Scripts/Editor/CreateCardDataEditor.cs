using System.IO;
using CardGame.Enums;
using CardGame.ScriptableObjects;
using UnityEditor;
using UnityEngine;

public class CreateCardDataEditor : EditorWindow
{
    private string spriteFolderPath = "Assets/";
    private string savePath = "Assets/";

    [MenuItem("Terek Gaming/Create Card Data Objects")]
    private static void OpenWindow()
    {
        GetWindow<CreateCardDataEditor>().Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Card Data", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        spriteFolderPath = EditorGUILayout.TextField("Sprite Folder Path", spriteFolderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Sprite Folder", spriteFolderPath, "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
            {
                spriteFolderPath = "Assets" + path.Substring(Application.dataPath.Length).Replace("\\", "/");
            }
            else if (!string.IsNullOrEmpty(path))
            {
                Debug.LogError("Selected folder must be inside the Assets directory.");
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        savePath = EditorGUILayout.TextField("Save Path for ScriptableObjects", savePath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Save Folder", savePath, "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
            {
                savePath = "Assets" + path.Substring(Application.dataPath.Length).Replace("\\", "/");
            }
            else if (!string.IsNullOrEmpty(path))
            {
                Debug.LogError("Selected folder must be inside the Assets directory.");
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Generate Card Data Objects"))
        {
            GenerateCardDataObjects();
        }
    }

    private void GenerateCardDataObjects()
    {
        if (string.IsNullOrEmpty(spriteFolderPath) || string.IsNullOrEmpty(savePath))
        {
            Debug.LogError("Both paths must be set!");
            return;
        }
        
        string fullSpriteFolderPath = Path.Combine(Application.dataPath, spriteFolderPath.Substring("Assets".Length + 1)).Replace("\\", "/");

        var spriteFiles = Directory.GetFiles(fullSpriteFolderPath, "*.png", SearchOption.TopDirectoryOnly);
        int index = 0;
        foreach (var file in spriteFiles)
        {
            if (index > (int)CardType.Z) break;
            
            string assetFilePath = "Assets" + file.Substring(Application.dataPath.Length).Replace("\\", "/");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetFilePath);
            
            if (sprite != null)
            {
                CreateCardDataObject(sprite, index);
            }

            index++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Card Data Objects created successfully!");
    }

    private void CreateCardDataObject(Sprite sprite, int index)
    {
        var cardData = ScriptableObject.CreateInstance<CardDataContainerSO>();
        cardData.SetEditorBuild(sprite, (CardType)index);

        string assetPath = Path.Combine(savePath, $"{sprite.name}_CardDataContainer.asset").Replace("\\", "/");
        AssetDatabase.CreateAsset(cardData, assetPath);
    }
}