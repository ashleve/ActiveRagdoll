using UnityEngine;
using System.Collections;

public class ShootObject : MonoBehaviour
{

    public Rigidbody projectile;
    private float speed = 25f;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
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