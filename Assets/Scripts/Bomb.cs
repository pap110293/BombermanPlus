using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float countdown = 2f;
    public bool isExplosed = false;
    [HideInInspector]
    public int playerNumber;


    private int level;
    private MapManager mapDestroyer;

    private void Start()
    {
        mapDestroyer = FindObjectOfType<MapManager>();
    }

    // Update is called once per frame
    void Update () {
        countdown -= Time.deltaTime;

        if(countdown <= 0)
        {
            if (isExplosed == false)
                mapDestroyer.Explode(transform.position, level);
            Destroy(gameObject);
        }
	}

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public int GetLevel()
    {
        return level;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
