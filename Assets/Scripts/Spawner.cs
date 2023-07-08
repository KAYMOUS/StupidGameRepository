using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private PlayerMovements player;
    [SerializeField] private GameObject Enemy;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyEnemy()
    {
        Destroy(Enemy);
    }

    public void SpawnEnemy()
    {
        Instantiate(Enemy, transform.position, Quaternion.identity);
    }
}
