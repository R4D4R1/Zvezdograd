using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
public class PlaneGenerator : MonoBehaviour
{
    Mesh _mesh;
    MeshFilter _meshFilter;

    [SerializeField] private Vector2 planeSize;
    [SerializeField] private int planeResolution;

    private List<Vector3> _vertices;
    private List<int> _triangles;

    private void Awake()
    {
        _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;

        planeResolution = Mathf.Clamp(planeResolution, 1, 50);

        GeneratePlane(planeSize, planeResolution);
        AsignMesh();
    }

    //private void Update()
    //{
    //    planeResolution = Mathf.Clamp(planeResolution, 1, 50);

    //    GeneratePlane(planeSize, planeResolution);
    //    AsignMesh();
    //}

    private void AsignMesh()
    {
        _mesh.Clear();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
    }

    private void GeneratePlane(Vector2 size, int resolution)
    {
        _vertices = new List<Vector3>();
        float xPerStep = size.x / resolution;
        float yPerStep = size.y / resolution;
        for (int y = 0; y < resolution + 1; y++)
        {
            for (int x = 0; x < resolution + 1; x++)
            {
                _vertices.Add(new Vector3(x*xPerStep,0,y*yPerStep));
            }
        }

        _triangles = new List<int>();
        for(int row = 0; row < resolution; row++)
        {
            for(int col = 0; col < resolution; col++)
            {
                int i = (row * resolution) + row + col;

                _triangles.Add(i);
                _triangles.Add(i + (resolution) + 1);
                _triangles.Add(i + (resolution) + 2);

                _triangles.Add(i);
                _triangles.Add(i + resolution + 2);
                _triangles.Add(i +1);
            }
        }
    }
}
