using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class CameraCollider : MonoBehaviour
{

    private Camera _camera;

    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    [Button]
    private void Refresh()
    {
        _camera = GetComponent<Camera>();
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        Vector3[] farPlaneCorners = GetFarPlaneCorners(_camera);
        List<Vector3[]> frustumTriangles = GetFrustumTriangles(farPlaneCorners);
        RefreshCameraMesh(frustumTriangles);
    }

    private Vector3[] GetFarPlaneCorners(Camera camera)
    {
        if (camera == null)
            return new Vector3[0];

        Vector3[] frustumCorners = new Vector3[4];
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        return frustumCorners;
    }

    private List<Vector3[]> GetFrustumTriangles(Vector3[] farPlaneCorners)
    {
        if (farPlaneCorners == null || farPlaneCorners.Length == 0)
            return new List<Vector3[]>();

        List<Vector3[]> frustumTriangles = new List<Vector3[]>();

        // Sides

        frustumTriangles.Add(new[]
        {
            Vector3.zero,
            farPlaneCorners[1],
            farPlaneCorners[0]
        });

        frustumTriangles.Add(new[]
        {
            Vector3.zero,
            farPlaneCorners[2],
            farPlaneCorners[1]
        });

        frustumTriangles.Add(new[]
        {
            Vector3.zero,
            farPlaneCorners[3],
            farPlaneCorners[2]
        });

        frustumTriangles.Add(new[]
        {
            Vector3.zero,
            farPlaneCorners[0],
            farPlaneCorners[3]
        });

        // Far plane

        frustumTriangles.Add(new[]
        {
            farPlaneCorners[0],
            farPlaneCorners[1],
            farPlaneCorners[2]
        });

        frustumTriangles.Add(new[]
        {
            farPlaneCorners[0],
            farPlaneCorners[2],
            farPlaneCorners[3]
        });

        return frustumTriangles;
    }

    private void RefreshCameraMesh(List<Vector3[]> frustumTriangles)
    {
        if (frustumTriangles == null || frustumTriangles.Count == 0)
            return;

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < frustumTriangles.Count; i++)
        {
            for (int j = 0; j < frustumTriangles[i].Length; j++)
            {
                vertices.Add(frustumTriangles[i][j]);
                triangles.Add((i * 3) + j);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        _meshFilter.sharedMesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }

}