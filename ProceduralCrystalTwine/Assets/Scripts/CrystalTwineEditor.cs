using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CrystalTwine))]
public sealed class CrystalTwineEditor : Editor
{
#region Serialized Properties

    private SerializedProperty _twineNodeContainer;
    private SerializedProperty _generationSeed;

    // node properties
    private SerializedProperty _vertexDensityOnUnitCircle;
    private SerializedProperty _maxVerticesPerNode;

    // vertex manipulation properties
    private SerializedProperty _circularVertexOffset;

    // debugging properties
    private SerializedProperty _drawMeshGizmos;

#endregion Serialized Properties

#region Private fields

    private string _currentSeed;

    // node properties
    private float _curVertexDensityOnUnitCircle;
    private int   _curMaxVerticesPerNode;

    // vertex manipulation properties
    private float _curCircularVertexOffset;

    private bool _curDrawMeshGizmos;

#endregion Private fields

    private void OnEnable()
    {
        _twineNodeContainer = serializedObject.FindProperty("twineNodeContainer");
        _generationSeed     = serializedObject.FindProperty("generationSeed");

        _vertexDensityOnUnitCircle = serializedObject.FindProperty("vertexDensityOnUnitCircle");
        _maxVerticesPerNode        = serializedObject.FindProperty("maxVerticesPerNode");

        _circularVertexOffset = serializedObject.FindProperty("circularVertexOffset");

        _drawMeshGizmos = serializedObject.FindProperty("drawMeshGizmos");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        EditorGUILayout.ObjectField(_twineNodeContainer, new GUIContent("Twine Nodes Container"));

        _currentSeed = _generationSeed.stringValue;
        _currentSeed = EditorGUILayout.TextField(new GUIContent("Seed", "An empty field will create a random seed."),
                                                 _currentSeed);
        _generationSeed.stringValue = _currentSeed;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Node Manipulation", EditorStyles.boldLabel);

        _curVertexDensityOnUnitCircle = _vertexDensityOnUnitCircle.floatValue;
        _curVertexDensityOnUnitCircle =
            EditorGUILayout.FloatField(new GUIContent("Unit Circle Vertex Count",
                                                      "Base vertex count placed around a node with radius 1. Minimum is 3."),
                                       _curVertexDensityOnUnitCircle);
        _vertexDensityOnUnitCircle.floatValue = _curVertexDensityOnUnitCircle;

        _curMaxVerticesPerNode = _maxVerticesPerNode.intValue;
        _curMaxVerticesPerNode = EditorGUILayout.IntField(
            new GUIContent("Max Vertices Per Node", "No limit when set to 0."),
            _curMaxVerticesPerNode);
        _maxVerticesPerNode.intValue = _curMaxVerticesPerNode;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Vertex Manipulation", EditorStyles.boldLabel);

        _curCircularVertexOffset = _circularVertexOffset.floatValue;
        _curCircularVertexOffset =
            EditorGUILayout.Slider("Circular Vertex Offset", _curCircularVertexOffset, 0, 1);
        _circularVertexOffset.floatValue = _curCircularVertexOffset;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);

        _curDrawMeshGizmos        = _drawMeshGizmos.boolValue;
        _curDrawMeshGizmos        = EditorGUILayout.Toggle("Draw Mesh Gizmos", _curDrawMeshGizmos);
        _drawMeshGizmos.boolValue = _curDrawMeshGizmos;

        EditorGUILayout.Space();
        if (GUILayout.Button("Create New Crystal Twine"))
        {
            CrystalTwine twine = (CrystalTwine) target;
            twine.CreateNewCrystalTwine();
        }

        serializedObject.ApplyModifiedProperties();
    }
}