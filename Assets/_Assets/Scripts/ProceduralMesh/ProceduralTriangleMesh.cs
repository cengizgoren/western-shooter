using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralTriangleMesh : MonoBehaviour
{
    public Transform AimPoint;
    public Vector3 MuzzlePos;
    public Mesh mesh;

    public Vector3[] vertices;
    public int[] triangles;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Start()
    {
        MakeProceduralTriangle();
        UpdateMesh();
    }


    public void Update()
    {
        Vector3[] vertices = mesh.vertices;

        vertices[2] = AimPoint.transform.localPosition;

        mesh.vertices = vertices;

    }
    private void MakeProceduralTriangle()
    {
        vertices = new Vector3[] { new Vector3(0, 0, 0), MuzzlePos, new Vector3(0, 0, 2) };
        triangles = new int[] { 0, 1, 2 };
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

}
