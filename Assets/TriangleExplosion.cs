using System.Collections;
using UnityEngine;

public class TriangleExplosion : MonoBehaviour
{
	public IEnumerator SplitMesh(bool destroy)
	{
		if (GetComponent<MeshFilter>() == null || GetComponent<SkinnedMeshRenderer>() == null)
		{
			yield return null;
		}
		if (GetComponent<Collider>())
		{
			GetComponent<Collider>().enabled = false;
		}
		Mesh M = new Mesh();
		if (GetComponent<MeshFilter>())
		{
			M = GetComponent<MeshFilter>().mesh;
		}
		else if (GetComponent<SkinnedMeshRenderer>())
		{
			M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
		}
		Material[] materials = new Material[0];
		if (GetComponent<MeshRenderer>())
		{
			materials = GetComponent<MeshRenderer>().materials;
		}
		else if (GetComponent<SkinnedMeshRenderer>())
		{
			materials = GetComponent<SkinnedMeshRenderer>().materials;
		}
		Vector3[] verts = M.vertices;
		Vector3[] normals = M.normals;
		Vector2[] uvs = M.uv;
		for (int i = 0; i < M.subMeshCount; i++)
		{
			int[] triangles = M.GetTriangles(i);
			for (int j = 0; j < triangles.Length; j += 3)
			{
				Vector3[] array = new Vector3[3];
				Vector3[] array2 = new Vector3[3];
				Vector2[] array3 = new Vector2[3];
				for (int k = 0; k < 3; k++)
				{
					int num = triangles[j + k];
					array[k] = verts[num];
					array3[k] = uvs[num];
					array2[k] = normals[num];
				}
				Mesh mesh = new Mesh();
				mesh.vertices = array;
				mesh.normals = array2;
				mesh.uv = array3;
				mesh.triangles = new int[]
				{
					0,
					1,
					2,
					2,
					1,
					0
				};
				if (j % 2 != 0)
				{
					GameObject gameObject = new GameObject("Triangle " + j / 3);
					gameObject.transform.position = transform.position;
					gameObject.transform.rotation = transform.rotation;
					gameObject.transform.localScale = transform.lossyScale;
					gameObject.AddComponent<MeshRenderer>().material = materials[i];
					gameObject.AddComponent<MeshFilter>().mesh = mesh;
					gameObject.AddComponent<BoxCollider>();
					gameObject.AddComponent<Rigidbody>();
					Destroy(gameObject, 2f + Random.Range(0f, 4f));
				}
			}
		}
		GetComponent<Renderer>().enabled = false;
		yield return new WaitForSeconds(1f);
		if (destroy)
		{
			Destroy(gameObject);
		}
		yield break;
	}
}