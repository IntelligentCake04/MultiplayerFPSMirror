using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15;

    public bool isAutomatic = false;
    public Camera fpsCam;

    private float nextTimeToFire = 0f;

    private void Start()
    {
        fpsCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAutomatic == false)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        
        if(isAutomatic == true)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }
}
