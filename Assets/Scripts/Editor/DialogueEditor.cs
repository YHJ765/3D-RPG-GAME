using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using System;

[CustomEditor(typeof(DialogueData_SO))]
public class DialogueCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Open in Editor"))
        {
            DialogueEditor.InitWindow((DialogueData_SO)target);
        }
        base.OnInspectorGUI();
    }
}
public class DialogueEditor : EditorWindow 
{
    DialogueData_SO currentData;

    ReorderableList piecesList = null;

    [MenuItem("New Tools/Dialogue Editor")]
    
    public static void Init()
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        editorWindow.autoRepaintOnSceneChange = true;
    }

    public static void InitWindow(DialogueData_SO data)
    {
        DialogueEditor editorWindow = GetWindow<DialogueEditor>("Dialogue Editor");
        editorWindow.currentData = data;
    }

    [OnOpenAsset]
    public static bool OpenAsset(int instanceID, int line)
    {
        DialogueData_SO data = EditorUtility.InstanceIDToObject(instanceID) as DialogueData_SO;

        if(data != null)
        {
            DialogueEditor.InitWindow(data);
            return true;
        }
        return false;
    }

    void OnGUI()
    {
        if(currentData != null)
        {
            EditorGUILayout.TextField(currentData.name, EditorStyles.boldLabel);
            GUILayout.Space(10);

            if(piecesList == null)
                SetupReorderableList();

            piecesList.DoLayoutList();
        }
        else
        {
            GUILayout.Label("NO DATA SELECTED!", EditorStyles.boldLabel);
        }
    }

    private void SetupReorderableList()
    {
        piecesList = new ReorderableList(currentData.dialoguePieces, typeof(DialoguePiece), true, true, true, true);

        piecesList.drawHeaderCallback += OnDrawPieceHeader;
        piecesList.drawElementCallback += OnDrawPieceListElement;
        piecesList.elementHeightCallback += OnHeightChanged;
    }

    private float OnHeightChanged(int index)
    {
       return GetPieceHeight(currentData.dialoguePieces[index]);
    }

    float GetPieceHeight(DialoguePiece piece)
    {
        var height = EditorGUIUtility.singleLineHeight;

        height += EditorGUIUtility.singleLineHeight * 9;

        return height;
    }

    private void OnDrawPieceListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        if(index < currentData.dialoguePieces.Count)
        {
            var currentPiece = currentData.dialoguePieces[index];

            var tempRect = rect;

            tempRect.height = EditorGUIUtility.singleLineHeight;

            tempRect.width = 30;
            EditorGUI.LabelField(tempRect, "ID");

            tempRect.x += tempRect.width;
            tempRect.width = 100;
            currentPiece.ID = EditorGUI.TextField(tempRect, currentPiece.ID);

            tempRect.x += tempRect.width + 10;
            EditorGUI.LabelField(tempRect, "Quest");

            tempRect.x += 45;
            currentPiece.quest = (QuestData_SO)EditorGUI.ObjectField(tempRect, currentPiece.quest, typeof(QuestData_SO), false);

            tempRect.y += EditorGUIUtility.singleLineHeight + 5;
            tempRect.x = rect.x;
            tempRect.height = 60;
            tempRect.width = tempRect.height;
            currentPiece.image = (Sprite)EditorGUI.ObjectField(tempRect, currentPiece.image, typeof(Sprite), false);

            //文本框作业
            tempRect.x += tempRect.width + 5;
            tempRect.width = rect.width - tempRect.x;
            currentPiece.text = (string)EditorGUI.TextField(tempRect, currentPiece.text);
        }
    }

    private void OnDrawPieceHeader(Rect rect)
    {
        GUI.Label(rect, "Dialogue Pieces");
    }
}
