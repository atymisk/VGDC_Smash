using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float smoothing = 10f;
    public float sizeBuffer = 1f;
    public float zScalar = -2.0f;
	private GameObject[] players;
	private Camera cam;
    private Resolution resolution;
    private Transform holder; //move the holder for normal movement, not the camera

    private int screenShakeFramesLeft = 0;
    private float screenShakeIntensity = 0;
	void Awake()
	{
		players = GameObject.FindGameObjectsWithTag(Tags.Player);
		cam = GetComponent<Camera>();
        resolution = Screen.currentResolution; // may need to do checking to check if resolution has been changed
        holder = this.transform.parent;
	}

	void FixedUpdate()
	{
		float minx = holder.position.x;
        float maxx = holder.position.x;
        float miny = holder.position.y;
        float maxy = holder.position.y;

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
        float z = holder.position.z;
        if (xrange / resolution.width > yrange / resolution.height)
        {
            z = xrange;
        }
        else
        {
            yrange = resolution.width * yrange / resolution.height;
            z = yrange;
        }
        holder.position = Vector3.Lerp(holder.position, new Vector3(midx, midy, zScalar * z), smoothing * Time.deltaTime);

        //now for screen shake
        if (screenShakeFramesLeft > 0)
        {
            screenShakeFramesLeft = screenShakeFramesLeft - 1;
            cam.transform.localPosition = Random.insideUnitSphere * screenShakeIntensity;

        }
        else if (screenShakeFramesLeft == 0) //cleanup
        {
            screenShakeFramesLeft = -1;
            cam.transform.localPosition = new Vector3(0, 0, 0); //reset back to normal position
        }
	}
    public void ScreenShake(int frames, float intensity)
    {
        screenShakeFramesLeft = frames;
        screenShakeIntensity = intensity;
    }

}
