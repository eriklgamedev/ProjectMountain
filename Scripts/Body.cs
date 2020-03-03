using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
	public Transform head;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (head.position.x != transform.position.x || head.position.z != transform.position.z)
		{
			transform.position = new Vector3(head.transform.position.x, transform.position.y, head.transform.position.z);
		}

		transform.TransformDirection(head.transform.forward);
	}


}
