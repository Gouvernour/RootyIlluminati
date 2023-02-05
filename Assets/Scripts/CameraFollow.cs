using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraFollow : MonoBehaviour
{

    public float smoothSpeed;

    public float minSize;
    public float maxSize;
    public float borderX;
    public float borderY;

   

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.players.Count == 0)
            return;


        if (GameManager.instance.players.Count == 1)
        {
            Vector3 pos = GameManager.instance.players[0].transform.position;
            pos.z = -10; 
            transform.position = pos;
            Camera.main.orthographicSize = maxSize;

            return;
        }

        Bounds bounds = new Bounds(GameManager.instance.players[0].transform.position, Vector2.zero);

        foreach (GameObject p in GameManager.instance.players)
        {
            bounds.Encapsulate(p.transform.position);
        }


        Vector3 desiredPosition = bounds.center;
        desiredPosition.y *= 2; // Mathf.Min(Mathf.Max(smoothPosition.y, -borderY), borderY);
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        smoothPosition.x = Mathf.Min(Mathf.Max(smoothPosition.x, -borderX), borderX);
        smoothPosition.y = Mathf.Min(Mathf.Max(smoothPosition.y, 0), borderY);
        smoothPosition.z = -10;
        transform.position = smoothPosition;

        float mag = Mathf.Sqrt(bounds.size.x * bounds.size.x + bounds.size.y * bounds.size.y);
        float newSize = Mathf.Lerp(minSize, maxSize, mag / 25);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, newSize, Time.deltaTime);
    }
}
