using UnityEngine;
using System.Collections;
/// <summary>
/// See catlikecoding: 
/// http://catlikecoding.com/unity/tutorials/rounded-cube/
/// </summary>
[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
public class DeformableMesh : MonoBehaviour {
    public int xSize, ySize, zSize;
    Mesh mesh;
    Vector3[] vertices;

	// Use this for initialization
	private void Awake () {

        StartCoroutine(GenerateMesh());
	}


    private IEnumerator GenerateMesh()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Deformable Mesh";
        WaitForSeconds wait= new WaitForSeconds(0.05f);
        int cornerVertices = 8;
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        int v = 0;
        for (int x = 0; x <= xSize; x++)
        {
            vertices[v++] = new Vector3(x, 0, 0);
        }
            yield return wait;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }


	// Update is called once per frame
	void Update () {
	
	}
}
