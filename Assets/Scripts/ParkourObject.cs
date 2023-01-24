using System;
using UnityEngine;

public enum AiDirectionType {Left, Right}
public class ParkourObject : MonoBehaviour
{
    
    [SerializeField] private Mesh baseMesh;
    public Transform targetTransform;
    
    public Transform startTransform => transform;
    
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    public void Deform(float positionX)
    {
        var localPosition = targetTransform.localPosition;
        if (Math.Abs(localPosition.x - positionX) < 0.1f) return;
        
        localPosition = new Vector3(positionX, localPosition.y, localPosition.z);
        targetTransform.localPosition = localPosition;

        _meshFilter.mesh = baseMesh;
        Mesh mesh = _meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            float newPositionX = vertices[i].x + vertices[i].z / localPosition.z * localPosition.x;
            vertices[i] = new Vector3 (newPositionX, vertices[i].y, vertices[i].z);
        }
        
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        _meshCollider.sharedMesh = mesh;
        _meshFilter.mesh = mesh;
    }
}
