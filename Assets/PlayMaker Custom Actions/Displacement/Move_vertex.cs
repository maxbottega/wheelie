using UnityEngine;
using System.Collections;


using UnityEngine;
using System.Collections;



public class Move_vertex : MonoBehaviour {
	
	public float m_Displacement = 5;
	
    void Awake() {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int i = 0;
        while (i < vertices.Length) {
            vertices[i] += normals[i] * m_Displacement;
            i++;
        }
        mesh.vertices = vertices;
    }
}