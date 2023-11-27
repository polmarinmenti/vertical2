using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gun_Weapon : MonoBehaviour
{
    public Camera cam;

    [SerializeField] private int bulletsPerMag;
    private int bullets;
    [SerializeField] private float reloadTime = 1.5f;
    private bool isRecharging = false;

    [SerializeField] private GameObject decalPrefab;

    void Start()
    {
        bullets = bulletsPerMag;
    }

    void Update()
    {
        if (Input_Manager._INPUT_MANAGER.GetReload() && !isRecharging)
        {
            StartCoroutine(Recargar());
            return;
        }

        if (Input_Manager._INPUT_MANAGER.GetShoot() && !isRecharging && bullets > 0)
        {
            Debug.Log(bullets);
            Disparar();
        }
    }

    IEnumerator Recargar()
    {
        isRecharging = true;
        yield return new WaitForSeconds(reloadTime);

        bullets = bulletsPerMag;
        isRecharging = false;
    }

    void Disparar()
    {
        bullets--;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, transform.forward, out hit))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.forward, UnityEngine.Color.green, 100f);

            GameObject decal = Instantiate(decalPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0));

            // Verificar si el objeto impactado es el muro falso
            FakeTiles fakeTiles = hit.collider.GetComponent<FakeTiles>();
            if (fakeTiles != null)
            {
                fakeTiles.StartGame();
            }
            else
            {
                // Manejar otros tipos de impactos, como golpear un Tile
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    tile.ClickedTile();
                }
            }
        }
    }

}
