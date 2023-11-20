using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Weapon : MonoBehaviour
{
    public Camera cam;

    [SerializeField] private int bulletsPerMag;
    private int bullets;
    [SerializeField] private float reloadTime = 1.5f;
    private bool isRecharging = false;

    void Start()
    {
        bullets = bulletsPerMag;
    }

    void Update()
    {
        if (Input_Manager._INPUT_MANAGER.GetReload() && !isRecharging)
        {
            Debug.Log("got recargar");
            StartCoroutine(Recargar());
            return;
        }

        if (Input_Manager._INPUT_MANAGER.GetShoot() && !isRecharging)
        {
            Debug.Log("got disparar");
            Disparar();
        }
    }

    IEnumerator Recargar()
    {
        isRecharging = true;
        Debug.Log("Recargando...");
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
            Debug.Log(hit.transform.name);
        }
    }
}
