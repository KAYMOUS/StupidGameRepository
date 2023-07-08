using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Spawner spawn;
    public PlayerMovements player;
    public void RespawnPlayer()
    {
        Vector3 checkpointPosition = GameManager.Instance.GetLastCheckpoint();
        if (checkpointPosition != Vector3.zero)
        {
            
            transform.position = checkpointPosition;
            player.rb.bodyType = RigidbodyType2D.Dynamic;
            player.isAlive = true;
            spawn.SpawnEnemy();
        }
        else
        {
            // If no checkpoint has been reached, respawn at the starting position or a default location.
        }
    }
}
