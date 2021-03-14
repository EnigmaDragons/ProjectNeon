using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FXV
{
    public class FXVShooter : MonoBehaviour
    {
        public GameObject[] guns;
        public GameObject bulletPrefab;

        public bool autoShoot = false;
        public float autoShootRate = 1.0f;
        public float autoShootDispersion = 15.0f;

        private int currentGun = 0;

        private Vector3 baseShootDir;
        private float timeToNextShoot = 0.0f;

        void Start()
        {
            baseShootDir = transform.forward;
        }

        void Update()
        {
            if (autoShoot)
            {
                timeToNextShoot -= Time.deltaTime;
                if (timeToNextShoot <= 0.0f)
                {
                    Vector3 dir = Quaternion.Euler(Random.Range(-autoShootDispersion, autoShootDispersion), Random.Range(-autoShootDispersion, autoShootDispersion), Random.Range(-autoShootDispersion, autoShootDispersion)) * baseShootDir;
                    gameObject.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
                    Shoot(dir);

                    timeToNextShoot = 1.0f / autoShootRate;
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit rhi;

                if (Physics.Raycast(ray, out rhi))
                {
                    Vector3 dirShip = rhi.point - transform.position;
                    dirShip.Normalize();
                    gameObject.transform.rotation = Quaternion.LookRotation(dirShip, Vector3.up);

                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Vector3 dirBullet = rhi.point - guns[currentGun].transform.position;
                            dirBullet.Normalize();

                            Shoot(dirBullet);
                        }
                    }
                }
            }
        }

        void Shoot(Vector3 dir)
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, guns[currentGun].transform.position, guns[currentGun].transform.rotation);

            bullet.GetComponent<FXVBullet>().Shoot(dir);

            currentGun++;
            if (currentGun >= guns.Length)
            {
                currentGun = 0;
            }
        }
    }

}