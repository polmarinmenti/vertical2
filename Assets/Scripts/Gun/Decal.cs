using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decal : MonoBehaviour
{
    [SerializeField] private float lifeSpan;

    // Update is called once per frame
    void Update()
    {
        if (lifeSpan < 0)
        {
            Destroy(this.gameObject);
        }

        lifeSpan -= Time.deltaTime;
    }
}
