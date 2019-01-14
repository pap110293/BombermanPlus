using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("Destroy", 0.5f);
	}

    // Update is called once per frame
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
