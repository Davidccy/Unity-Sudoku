﻿using System.IO;
using UnityEditor;
using UnityEngine;

public class ToolBackgroundGenerator : EditorWindow {

    [MenuItem("PokemonTools/BackgroundGenerator Window")]
    public static void OpenWindow() {
        EditorWindow w = GetWindow<ToolBackgroundGenerator>("BackgroundGenerator Window");
        w.maxSize = Vector2.one * 500;
        w.minSize = Vector2.one * 500;
        w.Show();
    }

    private readonly int _BG_WIDTH = 1024;
    private readonly int _BG_HEIGHT = 1024;
    private int _squareLength = 0;

    private readonly int _COLOR_COUNT = 2;
    private Color _cType1;
    private Color _cType2;

    private string _outputName;

    public void OnGUI() {
        GUIStyle guiStyleCenter = new GUIStyle();
        guiStyleCenter.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("The image will be generated by 1024 * 1024", guiStyleCenter);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Length Settings:");
        _squareLength = EditorGUILayout.IntField("Square Length", _squareLength);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Color Settings:");
        _cType1 = EditorGUILayout.ColorField("Color Type 1", _cType1);
        _cType2 = EditorGUILayout.ColorField("Color Type 2", _cType2);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Output Settings:");
        _outputName = EditorGUILayout.TextField("Out Put Name", _outputName);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate !", GUILayout.Width(100))) {
            BackgroundGenerator();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void BackgroundGenerator() {
        Texture2D t = new Texture2D(_BG_WIDTH, _BG_HEIGHT, TextureFormat.ARGB32, false);
        for (int w = 0; w < _BG_WIDTH; w++) {
            for (int h = 0; h < _BG_HEIGHT; h++) {
                int sectionW = w / _squareLength;
                int sectionH = h / _squareLength;
                int colorType = ((sectionW % _COLOR_COUNT) + (sectionH % _COLOR_COUNT)) % _COLOR_COUNT;

                Color c;
                switch (colorType) {
                    case 0:
                        c = _cType1;
                        break;
                    case 1:
                        c = _cType2;
                        break;
                    default:
                        c = Color.white;
                        break;
                }

                t.SetPixel(w, h, c);
            }
        }
        
        byte[] bytes = t.EncodeToPNG();
        string fileName = string.Format("Assets/{0}.png", _outputName);
        File.WriteAllBytes(fileName, bytes);
    }
}


