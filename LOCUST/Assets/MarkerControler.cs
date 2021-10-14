using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerControler : MonoBehaviour
{
    public MeshFilter mFilter;
    public MeshRenderer mRenderer;
    public MeshCollider mCollider;

    public int pyramidFaces = 6;
    public float pyramidHeigth = 0.5f;
    public float pyramidRadius = 0.1f;

    private Mesh mesh;

    private void OnValidate()
    {
        buildMesh();
    }

    private void Start()
    {
        buildMesh();
    }

    private void buildMesh()
    {
        if (mFilter == null || mRenderer == null) return;

        if(mesh==null)
        {
            mesh = new Mesh();
            mFilter.sharedMesh = mesh;
            if(mCollider != null)mCollider.sharedMesh = mesh;
        }
        else
        {
            mesh.Clear();
        }

        Vector3[] vertices = new Vector3[pyramidFaces+2];
        int[] triangles = new int[3 * 2 * pyramidFaces];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 0, -pyramidHeigth);

        float angle = 2*Mathf.PI / pyramidFaces * 1.0f;

        int triIndex = 0;

        for(int i = 0; i < pyramidFaces; ++i)
        {
            vertices[i+2] = new Vector3(Mathf.Cos(angle * i) * pyramidRadius, Mathf.Sin(angle * i) * pyramidRadius, -pyramidHeigth);

            int nextVertexIndex;

            if(i == pyramidFaces - 1)
            {
                nextVertexIndex = 2;
            }
            else
            {
                nextVertexIndex = i+2 + 1;
            }


            triangles[triIndex++] = i+2;
            triangles[triIndex++] = nextVertexIndex;
            triangles[triIndex++] = 0;

            triangles[triIndex++] = i+2;
            triangles[triIndex++] = 1;
            triangles[triIndex++] = nextVertexIndex;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
