using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CrystalTwine))]
public sealed class CrystalTwineEditor : Editor
{
#region Serialized Properties

    private SerializedProperty _twineNodeContainer;
    private SerializedProperty _generationSeed;

    // node properties
    private SerializedProperty _minVertexAddPerNode;
    private SerializedProperty _maxVerticesPerNode;

    // vertex manipulation properties
    private SerializedProperty _circularVertexOffset;

    // debugging properties
    private SerializedProperty _drawMeshGizmos;
    private SerializedProperty _generateMesh;

#endregion Serialized Properties

#region Private fields
    
    private string _currentSeed;

    // node properties
    private int _curMinVertexAddPerNode;
    private int _curMaxVerticesPerNode;
    
    // vertex manipulation properties
    private float _curCircularVertexOffset;
    
    private bool _curDrawMeshGizmos;
    private bool _curGenerateMesh;

#endregion Private fields

    private void OnEnable()
    {
        _twineNodeContainer = serializedObject.FindProperty("twineNodeContainer");
        _generationSeed     = serializedObject.FindProperty("generationSeed");

        _minVertexAddPerNode = serializedObject.FindProperty("minVertexAddPerNode");
        _maxVerticesPerNode  = serializedObject.FindProperty("maxVerticesPerNode");

        _circularVertexOffset = serializedObject.FindProperty("circularVertexOffset");

        _drawMeshGizmos = serializedObject.FindProperty("drawMeshGizmos");
        _generateMesh   = serializedObject.FindProperty("generateMesh");
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

        _curMinVertexAddPerNode = _minVertexAddPerNode.intValue;
        _curMinVertexAddPerNode = EditorGUILayout.IntField(new GUIContent("Min Vertex Add Per Node",
                                                                       "Sets the minimum number of additional vertices a node " +
                                                                       "will have in its neighbourhood compared to the node above it. " +
                                                                       "The top node will have one vertex without offset."),
                                                           _curMinVertexAddPerNode);
        _minVertexAddPerNode.intValue = _curMinVertexAddPerNode;

        _curMaxVerticesPerNode = _maxVerticesPerNode.intValue;
        _curMaxVerticesPerNode = EditorGUILayout.IntField(new GUIContent("Max Vertices Per Node", "No limit when set to 0."),
                                                          _curMaxVerticesPerNode);
        _maxVerticesPerNode.intValue = _curMaxVerticesPerNode;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Vertex Manipulation", EditorStyles.boldLabel);

        _curCircularVertexOffset = _circularVertexOffset.floatValue;
        _curCircularVertexOffset = EditorGUILayout.Slider("Circular Vertex Offset", _curCircularVertexOffset, 0, 1);
        _circularVertexOffset.floatValue = _curCircularVertexOffset;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);

        _curDrawMeshGizmos = _drawMeshGizmos.boolValue;
        _curDrawMeshGizmos = EditorGUILayout.Toggle("Draw Mesh Gizmos", _curDrawMeshGizmos);
        _drawMeshGizmos.boolValue = _curDrawMeshGizmos;
        
        _curGenerateMesh = _generateMesh.boolValue;
        _curGenerateMesh = EditorGUILayout.Toggle("Generate Mesh", _curGenerateMesh);
        _generateMesh.boolValue = _curGenerateMesh;

        EditorGUILayout.Space();
        if (GUILayout.Button("Create New Mesh"))
        {
            CrystalTwine twine = (CrystalTwine) target;
            twine.CreateNewMesh();
        }

        serializedObject.ApplyModifiedProperties();
    }
}