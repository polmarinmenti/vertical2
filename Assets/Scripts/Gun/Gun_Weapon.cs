using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gun_Weapon : MonoBehaviour
{
    public GameObject cam;

    //[SerializeField] private int bulletsPerMag;
    //private int bullets;

    //[SerializeField] private float reloadTime = 1.5f;
    //private bool isRecharging = false;

    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private GameObject paintballPrefab;
    [SerializeField] private GameObject muzzle;

    [SerializeField] private Transform player;

    AudioSource audioSource;
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip paintballSound;

    void Start()
    {
        //bullets = bulletsPerMag;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //if (Input_Manager._INPUT_MANAGER.GetReload() && !isRecharging)
        //{
        //    StartCoroutine(Recargar());
        //    return;
        //}

        if (Input_Manager._INPUT_MANAGER.GetShoot()/* && !isRecharging && bullets > 0*/)
        {
            //Debug.Log(bullets);
            Disparar();
        }
        
        if (Input_Manager._INPUT_MANAGER.GetPaintball())
        {
            Paintball();
        }

        if (Input_Manager._INPUT_MANAGER.GetTeleport())
        {
            Teleport();
        }
    }

    //IEnumerator Recargar()
    //{
    //    isRecharging = true;
    //    yield return new WaitForSeconds(reloadTime);

    //    bullets = bulletsPerMag;
    //    isRecharging = false;
    //}

    void Disparar()
    {
        audioSource.PlayOneShot(shotSound);

        //bullets--;

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
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

            GameObject muzzleFlash = Instantiate(muzzle, this.transform);
            Destroy(muzzleFlash, 0.04f);
        }
    }

    void Paintball()
    {
        audioSource.PlayOneShot(paintballSound);

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, transform.forward, out hit))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.forward, UnityEngine.Color.green, 100f);

            GameObject paintball = Instantiate(paintballPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0));

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
                    tile.ToggleFlag();
                }
            }
        }
    }
    void Teleport()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, transform.forward, out hit))
        {
            //Debug.DrawRay(cam.transform.position, cam.transform.forward, UnityEngine.Color.green, 100f);

            player.position = hit.point;
        }
    }

}
