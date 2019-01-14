using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public float countdown = 2f;
    public bool isExplosed = false;

    private int level;
    private MapDestroyer mapDestroyer;

    private void Start()
    {
        mapDestroyer = FindObjectOfType<MapDestroyer>();
    }

    // Update is called once per frame
    void Update () {
        countdown -= Time.deltaTime;

        if(countdown <= 0)
        {
            Destroy(gameObject);
            if(isExplosed == false)
                mapDestroyer.Explode(transform.position, level);
            //BombManager.DestroyABomb(this);
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
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
