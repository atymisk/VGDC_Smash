using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float smoothing = 10f;

	private GameObject[] players;
	private Camera cam;

	void Awake()
	{
		players = GameObject.FindGameObjectsWithTag(Tags.Player);
		cam = GetComponent<Camera>();
	}

	void Update()
	{
		float minx = cam.transform.position.x;
		float maxx = cam.transform.position.x;
		float miny = cam.transform.position.y;
		float maxy = cam.transform.position.y;

		foreach (GameObject player in players)
		{
			Vector3 playerPos = player.transform.position;
			if (minx > playerPos.x)
				minx = playerPos.x;
			if (maxx < playerPos.x)
				maxx = playerPos.x;
			if (miny > playerPos.y)
				miny = playerPos.y;
			if (maxy < playerPos.y)
				maxy = playerPos.y;
		}

		float midx = (minx + maxx) * 0.5f;
		float midy = (miny + maxy) * 0.5f;

		cam.transform.position = Vector3.Lerp (cam.transform.position, new Vector3(midx, midy, cam.transform.position.z), smoothing * Time.deltaTime);
	}
}
