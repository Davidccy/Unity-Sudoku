using UnityEngine;
using UnityEditor;

public class ToolGameConfigGenerator {
    [MenuItem("PokemonTools/GameConfigGenerator")]
    public static void CreateMyAsset() {
        GameConfig asset = ScriptableObject.CreateInstance<GameConfig>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/GameConfig/GameConfig.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
