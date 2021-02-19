using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CrystalTwine : MonoBehaviour
{
#region Serialized Fields

    [SerializeField] private Transform twineNodeContainer;

    [SerializeField, Tooltip("Seed lower than 0 gets set to -1.")]
    private int generationSeed;

    [Header("Node Manipulation"),
     SerializeField,
     Tooltip("Sets the minimum number of additional vertices a node will have in its neighbourhood compared to the " +
             "node above it. The top node will have one vertex without offset.")]
    private int minVertexAddPerNode;

    [SerializeField, Tooltip("No limit when set to 0.")]
    private int maxVerticesPerNode;

    [Header("Vertex Manipulation")] [SerializeField]
    private float maxVertexOffsetX;

    [SerializeField] private float maxVertexOffsetY;
    [SerializeField] private float maxVertexOffsetZ;

    [Header("Debugging")] [SerializeField] private bool drawMeshGizmos;

#endregion Serialized Fields

#region Private variables

    private Mesh _twineMesh;

    private Vector3[] _vertices;
    private int[]     _verticesPerNode;

#endregion Private variables

    private void Start()
    {
        Generate(generationSeed < 0 ? -1 : generationSeed);
    }

    private void Generate(int seed)
    {
        Random.InitState(seed);

        GetComponent<MeshFilter>().mesh = _twineMesh = new Mesh();
        _twineMesh.name                 = "Crystal Twine";

        TwineNode[] twineNodes = twineNodeContainer.GetComponentsInChildren<TwineNode>();

        _verticesPerNode = new int[twineNodes.Length];
        int vertexCountUnderTop = CalculateVertexCountPerNode(_verticesPerNode, minVertexAddPerNode,
                                                              maxVerticesPerNode == 0
                                                                  ? int.MaxValue
                                                                  : maxVerticesPerNode);

        int v = 0;
        _vertices      = new Vector3[vertexCountUnderTop];
        _vertices[v++] = twineNodes[0].transform.position;

        // calculate base positions for all vertices near the other nodes
        for (int i = 1; i < twineNodes.Length; i++)
        {
            int vertexRangeIndex = v;

            TwineNode curNode         = twineNodes[i];
            int       nodeVertexCount = _verticesPerNode[i];
            float     offsetDegree    = 360f / nodeVertexCount;

            // TODO: rotate axis and first vertex relatively to orientation of predecessor and successor nodes
            _vertices[v++] = curNode.transform.position + Vector3.forward * curNode.BaseRadius;
            for (int j = 1; j < nodeVertexCount; j++)
            {
                _vertices[v] = _vertices[v - 1].Rotated(0, offsetDegree, 0, Vector3.up);
                v++;
            }

            // the bottom vertices will be plane
            if (i >= twineNodes.Length - 1)
            {
                continue;
            }

            // TODO: apply offsets in local space
            // apply random vertex offsets
            for (int k = 0; k < nodeVertexCount; k++)
            {
                // TODO: maybe apply offset values relatively to node radius
                _vertices[vertexRangeIndex + k] += new Vector3(Random.Range(-maxVertexOffsetX, maxVertexOffsetX),
                                                               Random.Range(-maxVertexOffsetY, maxVertexOffsetY),
                                                               Random.Range(-maxVertexOffsetZ, maxVertexOffsetZ));
            }
        }
    }

    /// <summary>
    /// Calculates the vertex count for each twine node from top to bottom by adding a set minimum number of vertices
    /// and adding a random between 0 and 3 amount for each node on top.
    /// </summary>
    /// <param name="vertexCountPerNode">Array for a vertex count for each twine node. The first node will get
    /// initialized to 1.</param>
    /// <param name="addedVerticesPerNode">Minimum number of vertices to be added to </param>
    /// <param name="maximumVerticesPerNode">Maximum of vertices per node</param>
    /// <returns>Sum of all vertices</returns>
    private int CalculateVertexCountPerNode(IList<int> vertexCountPerNode, int addedVerticesPerNode,
                                            int        maximumVerticesPerNode)
    {
        vertexCountPerNode[0] = 1;
        int vertexCount = 1;

        for (int i = 1; i < vertexCountPerNode.Count; i++)
        {
            // TODO: use delta between two node radius to determine if less vertices should be used instead of more
            int nodeVertexCount = vertexCountPerNode[i - 1] + addedVerticesPerNode + Random.Range(0, 3);
            nodeVertexCount = nodeVertexCount > maximumVerticesPerNode
                ? maximumVerticesPerNode
                : nodeVertexCount;
            vertexCountPerNode[i] =  nodeVertexCount;
            vertexCount           += nodeVertexCount;
        }

        return vertexCount;
    }

    private void OnDrawGizmos()
    {
        if (_vertices == null || _vertices.Length <= 0 || !drawMeshGizmos)
        {
            return;
        }

        Gizmos.color = Color.red;
        foreach (Vector3 vertex in _vertices)
        {
            Gizmos.DrawSphere(vertex, 0.075f);
        }
    }
}