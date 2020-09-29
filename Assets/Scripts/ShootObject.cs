using UnityEngine;
using System.Collections;

public class ShootObject : MonoBehaviour
{

    public Rigidbody projectile;
    private float speed = 21f;


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        Rigidbody instantiatedProjectile = Instantiate(
            projectile,
            transform.position + transform.forward * 1.5f,
            transform.rotation)
        as Rigidbody;

        instantiatedProjectile.velocity = transform.forward * speed;
    }
}