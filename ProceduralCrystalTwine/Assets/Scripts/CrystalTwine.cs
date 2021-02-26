using System;
using System.Collections.Generic;
using DefaultNamespace.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CrystalTwine : MonoBehaviour
{
#region Serialized Fields

    [SerializeField]
    private Transform twineNodeContainer;

    [SerializeField]
    private string generationSeed;

    [SerializeField]
    private float vertexDensityOnUnitCircle;

    [SerializeField]
    private int maxVerticesPerNode;

    [SerializeField]
    private float circularVertexOffset;

    [SerializeField]
    private bool drawMeshGizmos;

#endregion Serialized Fields

#region Private variables

    private Mesh _twineMesh;

    private Vector3[] _vertices;
    private int[]     _verticesPerNode;

#endregion Private variables

#region MonoBevahiour

    private void Start()
    {
        CreateNewCrystalTwine();
    }

    private void OnDrawGizmos()
    {
        if (_vertices == null || _vertices.Length == 0 || !drawMeshGizmos)
        {
            return;
        }

        Gizmos.color = Color.red;
        foreach (Vector3 vertex in _vertices)
        {
            Gizmos.DrawSphere(vertex, 0.075f);
        }
    }

#endregion MonoBevahiour

#region Mesh Generation

    public void CreateNewCrystalTwine()
    {
        if (generationSeed.Length == 0)
        {
            string randomSeed = Helpers.RandomString(10);
            generationSeed = randomSeed;
        }

        GenerateCrystalTwine(generationSeed.GetHashCode());
    }

    private void GenerateCrystalTwine(int seed)
    {
        Random.InitState(seed);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.Clear();

        _twineMesh      = new Mesh {name = "Crystal Twine"};
        meshFilter.mesh = _twineMesh;

        TwineNode[] twineNodes = twineNodeContainer.GetComponentsInChildren<TwineNode>();

        _verticesPerNode = new int[twineNodes.Length];
        int vertexCountUnderTop = CalculateVertexCountPerNode(_verticesPerNode, twineNodes,
                                                              maxVerticesPerNode < 1
                                                                  ? int.MaxValue
                                                                  : maxVerticesPerNode);

        _vertices           = CalculateVertexPositions(twineNodes, vertexCountUnderTop);
        _twineMesh.vertices = _vertices;

        GenerateMesh(vertexCountUnderTop);
    }

    private void GenerateMesh(int vertexCountUnderTop)
    {
        int   nodeVertexIndex = 0;
        int[] triangleIndices = new int[3 * vertexCountUnderTop];

        for (int i = 1; i < _verticesPerNode.Length; i++)
        {
            int nodeVertexCount = _verticesPerNode[i];
            for (int j = 0; j < nodeVertexCount; j++)
            {
                triangleIndices[3 * (j + nodeVertexIndex)]     = 0;
                triangleIndices[3 * (j + nodeVertexIndex) + 1] = nodeVertexIndex + j + 1;
                triangleIndices[3 * (j + nodeVertexIndex) + 2] = nodeVertexIndex + (j + 1) % nodeVertexCount + 1;
            }

            nodeVertexIndex += nodeVertexCount;
        }

        _twineMesh.triangles = triangleIndices;
        _twineMesh.RecalculateTangents();
        _twineMesh.RecalculateNormals();
    }

    private Vector3[] CalculateVertexPositions(TwineNode[] twineNodes, int vertexCountUnderTop)
    {
        int       nodeVertexIndex        = 0;
        Vector3[] vertices               = new Vector3[vertexCountUnderTop];
        float[]   vertexCircularRotation = new float[vertexCountUnderTop];

        // top node has only one vertex at its center
        vertices[nodeVertexIndex++] = twineNodes[0].transform.position;

        // calculate positions for vertices near other nodes
        for (int i = 1; i < twineNodes.Length; i++)
        {
            int vertexRangeIndex = nodeVertexIndex;

            TwineNode curNode                     = twineNodes[i];
            int       nodeVertexCount             = _verticesPerNode[i];
            float     circularOffsetToPredecessor = 360f / nodeVertexCount;
            float     circularOffsetPerVertex     = circularVertexOffset * circularOffsetToPredecessor / 2;

            // TODO: rotate axis and first vertex relatively to orientation of predecessor and successor nodes
            vertices[nodeVertexIndex++] = curNode.transform.position + Vector3.forward * curNode.BaseRadius;
            for (int j = 1; j < nodeVertexCount; j++)
            {
                float circularOffset = circularOffsetToPredecessor +
                                       Random.Range(-circularOffsetPerVertex, circularOffsetPerVertex);
                vertices[nodeVertexIndex] = vertices[nodeVertexIndex - 1].Rotated(0, circularOffset, 0, Vector3.up);

                vertexCircularRotation[nodeVertexIndex] = circularOffset;
                nodeVertexIndex++;
            }

            // TODO: apply vertex offset towards/from node based on radius
            // TODO: apply vertex offset based on node up vector
        }

        return vertices;
    }

    /// <summary>
    /// Calculates the vertex count for each twine node from top to bottom by adding a set minimum number of vertices
    /// and adding a random between 0 and 3 amount for each node on top.
    /// </summary>
    /// <param name="vertexCountPerNode">Array for a vertex count for each twine node. The first node will get
    /// initialized to 1.</param>
    /// <param name="twineNodes">Array of twine nodes with radius (might be negative for rng)</param>
    /// <param name="maximumVerticesPerNode">Maximum of vertices per node</param>
    /// <returns>Sum of all vertices</returns>
    private int CalculateVertexCountPerNode
        (IList<int> vertexCountPerNode, TwineNode[] twineNodes, int maximumVerticesPerNode)
    {
        vertexCountPerNode[0] = 1;
        int vertexCount = 1;

        for (int i = 1; i < twineNodes.Length; i++)
        {
            int   nodeVertexCount;
            float nodeRadius = twineNodes[i].BaseRadius;

            if (nodeRadius < 0)
            {
                nodeRadius               = Random.Range(0.1f, 1);
                twineNodes[i].BaseRadius = nodeRadius;
            }

            if (Math.Abs(nodeRadius) < float.Epsilon)
            {
                nodeVertexCount = 1;
            } else
            {
                nodeVertexCount = (int) (twineNodes[i].BaseRadius * vertexDensityOnUnitCircle);

                nodeVertexCount = nodeVertexCount > maximumVerticesPerNode
                    ? maximumVerticesPerNode
                    : nodeVertexCount;

                nodeVertexCount = nodeVertexCount < 3
                    ? 3
                    : nodeVertexCount;
            }

            vertexCountPerNode[i] =  nodeVertexCount;
            vertexCount           += nodeVertexCount;
        }

        return vertexCount;
    }

#endregion Mesh Generation
}