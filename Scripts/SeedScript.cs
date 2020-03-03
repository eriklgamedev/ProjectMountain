using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedScript : MonoBehaviour
{
	public List<GameObject> flowerList = new List<GameObject>();


	Rigidbody rb = null;

    [HideInInspector]
    public PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroySelf(gameObject));

        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * 10, ForceMode.Impulse);

		//int flowerIndex = Random.Range(0, flowerList.Count - 1);
	}

	void OnCollisionEnter(Collision collision)
    {
        Vector3 spawnLocation = this.transform.position;

        if (collision.gameObject.tag == "Checkpoint")
        {
            collision.transform.GetComponent<Checkpoint>().Activate();
            Instantiate(flowerList[Random.Range(0, flowerList.Count - 1)], spawnLocation, Quaternion.identity);

            //Debug.Log("seed: " + collision.transform.GetComponent<Checkpoint>().player.GetComponent<Player>().spawnPointPosition);

            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Enemy")
        {
            if (collision.transform.GetComponent<StationaryEnemy>() != null)
            {
                collision.transform.GetComponent<StationaryEnemy>().TakeDamage();
                MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
                colorBlock.SetColor("_BaseColor", Color.red);
                collision.gameObject.GetComponent<Renderer>().SetPropertyBlock(colorBlock);
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Gardenable")
        {
            // Only if we hit something, do we continue
            RaycastHit hit;
            GameObject flower = Instantiate(flowerList[Random.Range(0, flowerList.Count - 1)], spawnLocation, Quaternion.identity);
			// Extract local space normals of the triangle we hit
			Ray ray = new Ray(flower.transform.position, -Vector3.up);
            //Debug.DrawRay(flower.transform.position, -Vector3.up, Color.red, 2);
            if (!Physics.Raycast(ray, out hit))
            {
                return;
            }
            // Just in case, also make sure the collider also has a renderer
            // material and texture
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                return;
            }

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;

            // Extract local space normals of the triangle we hit
            Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

            // interpolate using the barycentric coordinate of the hitpoint
            Vector3 baryCenter = hit.barycentricCoordinate;

            // Use barycentric coordinate to interpolate normal
            Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
            // normalize the interpolated normal
            interpolatedNormal = interpolatedNormal.normalized;

            // Transform local space normals to world space
            Transform hitTransform = hit.collider.transform;
            interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);

            // Display with Debug.DrawLine
            //Debug.DrawRay(hit.point, interpolatedNormal);

            flower.transform.rotation.SetLookRotation(interpolatedNormal);

            Destroy(gameObject);
        }
    }


    IEnumerator DestroySelf(GameObject desGameObject)
    {
        yield return new WaitForSecondsRealtime(3);

        Destroy(desGameObject);
    }
}
