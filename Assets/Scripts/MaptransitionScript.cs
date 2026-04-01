using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
public class MaptransitionScript : MonoBehaviour
{

[SerializeField] PolygonCollider2D mapBoundry;
CinemachineConfiner confiner;

[SerializeField] Direction direction;
[SerializeField] float Add_distance = 2f;



enum Direction {Up, Down, Left, Right}


    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner.m_BoundingShape2D = mapBoundry;
            UpdatePlayerPosition(collision.gameObject);

            MapController_Manual.Instance?. HighlightArea(mapBoundry.name);
            MapController_Dynamic.Instance?.UpdateCurrentArea(mapBoundry.name);
        }
    }

private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up: newPos.y +=Add_distance;
            break;
       

       
            case Direction.Down: newPos.y -=Add_distance;
            break;
      
            case Direction.Right: newPos.x +=Add_distance;
            break;
    
            case Direction.Left: newPos.x -=Add_distance;
            break;
        }
    player.transform.position = newPos;

    }



}
