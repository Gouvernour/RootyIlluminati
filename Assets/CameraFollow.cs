using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public float smoothSpeed;

    public float minSize;
    public float maxSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.players.Count == 0)
            return;

        Bounds bounds = new Bounds();

        foreach (GameObject p in GameManager.instance.players)
        {
            bounds.Encapsulate(p.transform.position);
        }



        Vector3 desiredPosition = bounds.center;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothPosition;

        float mag = Mathf.Sqrt(bounds.size.x * bounds.size.x + bounds.size.y * bounds.size.y);
        float newSize = Mathf.Lerp(minSize, maxSize, mag / 25);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, newSize, Time.deltaTime);
    }
}