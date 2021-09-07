using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    // cached referece
    private BallZone _ballZone;
    private void Start()
    {
        _ballZone = FindObjectOfType<BallZone>();
    }


    public Vector2 GetMousePosition2D()
    {
        return new Vector2(Input.mousePosition.x / Screen.width * _ballZone.ZoneWidth, Input.mousePosition.y / Screen.height * _ballZone.ZoneHeight);
    }

    public Vector2 GetMousePosition3D()
    {
        return new Vector3(Input.mousePosition.x / Screen.width * _ballZone.ZoneWidth, Input.mousePosition.y / Screen.height * _ballZone.ZoneHeight, 0f);
    }

}
