using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float smoothing = 10f;
    public float sizeBuffer = 1f;
    public float zScalar = -2.0f;
	private GameObject[] players;
	private Camera cam;
    private Resolution resolution;

	void Awake()
	{
		players = GameObject.FindGameObjectsWithTag(Tags.Player);
		cam = GetComponent<Camera>();
        resolution = Screen.currentResolution; // may need to do checking to check if resolution has been changed
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

        float xrange = Mathf.Abs(minx - maxx) + sizeBuffer;
        float yrange = Mathf.Abs(miny - maxy) + sizeBuffer;
        float z = cam.transform.position.z;
        if (xrange / resolution.width > yrange / resolution.height)
        {
            z = xrange;
        }
        else
        {
            yrange = resolution.width * yrange / resolution.height;
            z = yrange;
        }
        cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(midx, midy, zScalar* z), smoothing * Time.deltaTime);
	}
}
