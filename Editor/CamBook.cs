using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


public class CamBook : EditorWindow
{
    Dictionary<int, Vector3> bookmarksPositions = new Dictionary<int, Vector3>();
    Dictionary<int, Quaternion> bookmarksRotations = new Dictionary<int, Quaternion>();

    public DataValues dataValues;

    const string DataValuePath = "Packages/com.zeroslide.cambook/Editor/CamBookData.asset";
    const string StylePath = "Packages/com.zeroslide.cambook/Editor/CamBook.uxml";

    [MenuItem("Window/CamBook")]
    static void ShowWindow()
    {
        GetWindow<CamBook>("CamBook");
    }

    void OnEnable()
    {
        if (dataValues == null)
            dataValues = AssetDatabase.LoadAssetAtPath<DataValues>(DataValuePath);
    }

    void CreateGUI()
    {
        LoadDataValues();

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(StylePath);
        VisualElement tree = visualTree.Instantiate();
        rootVisualElement.Add(tree);

        var setButtons = tree.Query<Button>(className: "set-button").ToList();
        foreach (var btn in setButtons)
        {
            btn.RegisterCallback<ClickEvent>(OnSetBookmarkClick);
        }

        var jumpButtons = tree.Query<Button>(className: "jump-button").ToList();
        foreach (var btn in jumpButtons)
        {
            btn.RegisterCallback<ClickEvent>(OnJumpBookmarkClick);
        }

        var removeButtons = tree.Query<Button>(className: "remove-button").ToList();
        foreach (var btn in removeButtons)
        {
            btn.RegisterCallback<ClickEvent>(OnRemoveBookmarkClick);
        }
    }

    #region Data Management
    void LoadDataValues()
    {
        if (dataValues != null)
        {
            for (int i = 0; i < dataValues.bmPositions.Length; i++)
            {
                if (!bookmarksPositions.ContainsKey(i))
                {
                    bookmarksPositions.Add(i, dataValues.bmPositions[i]);
                    bookmarksRotations.Add(i, dataValues.bmRotations[i]);
                }
            }
        }
    }

    void SaveDataValues(int i)
    {
        dataValues.bmPositions[i] = bookmarksPositions[i];
        dataValues.bmRotations[i] = bookmarksRotations[i];

        EditorUtility.SetDirty(dataValues);
    }

    void DeleteDataValues(int i)
    {
        dataValues.bmPositions[i] = Vector3.zero;
        dataValues.bmRotations[i] = Quaternion.identity;
        EditorUtility.SetDirty(dataValues);
    }
    #endregion

    #region Button Events
    void OnSetBookmarkClick(ClickEvent evt)
    {
        Button clickedButton = evt.target as Button;
        bookmarksPositions[int.Parse(clickedButton.name[^1].ToString())] = SceneView.lastActiveSceneView.pivot;
        bookmarksRotations[int.Parse(clickedButton.name[^1].ToString())] = SceneView.lastActiveSceneView.camera.transform.rotation;

        SaveDataValues(int.Parse(clickedButton.name[^1].ToString()));
    }

    void OnJumpBookmarkClick(ClickEvent evt)
    {
        Button clickedButton = evt.target as Button;

        if (bookmarksPositions.ContainsKey(int.Parse(clickedButton.name[^1].ToString())) && bookmarksRotations.ContainsKey(int.Parse(clickedButton.name[^1].ToString())))
        {
            SceneView.lastActiveSceneView.pivot = bookmarksPositions[int.Parse(clickedButton.name[^1].ToString())];
            SceneView.lastActiveSceneView.rotation = bookmarksRotations[int.Parse(clickedButton.name[^1].ToString())];
            SceneView.lastActiveSceneView.Repaint();
        }
    }

    void OnRemoveBookmarkClick(ClickEvent evt)
    {
        Button clickedButton = evt.target as Button;
        bookmarksPositions.Remove(int.Parse(clickedButton.name[^1].ToString()));
        bookmarksRotations.Remove(int.Parse(clickedButton.name[^1].ToString()));

        DeleteDataValues(int.Parse(clickedButton.name[^1].ToString()));
    }
    #endregion
}
