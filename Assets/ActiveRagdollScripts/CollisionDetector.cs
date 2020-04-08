using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionDetector : MonoBehaviour
{

    private SlaveController slaveController;
    private MasterController masterController;
    private int deadTime = 1;
    private int forceNeedeToKnockDown = 500;
    private int connectedBoxId = -1;
    private bool isConnected = false;

    // Start is called before the first frame update
    void Start()
    {
        slaveController = transform.root.GetComponentInChildren<SlaveController>();
        masterController = transform.root.GetComponentInChildren<MasterController>();
        isConnected = false;
        connectedBoxId = -1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name != "FLOOR" && collision.transform.root != this.transform.root)
        {
            if (!isConnected && collision.gameObject.tag == "BOX" && this.gameObject.tag == "HAND") 
            {
                if (!collision.gameObject.GetComponent<Box>().isTaken || collision.gameObject.GetComponent<Box>().hostID == transform.root.GetInstanceID())
                {
                    Debug.Log("Collided"); 
                    var hingeJoint = gameObject.AddComponent<HingeJoint>(); 
                    var otherBody = collision.gameObject.GetComponent<Rigidbody>(); 
                    hingeJoint.breakForce = 4500;
                    hingeJoint.breakTorque = 4500; 
                    hingeJoint.connectedBody = otherBody;

                    isConnected = true;

                    masterController.handsConnected += 1;
                    masterController.DisableIK();

                    collision.gameObject.GetComponent<Box>().isTaken = true;
                    collision.gameObject.GetComponent<Box>().hostID = transform.root.GetInstanceID();

                    return;
                }
            }
/*
            slaveController.numberOfCollisions++;
            float collisionSpeed = collision.relativeVelocity.magnitude;

            if (collisionSpeed > forceNeedeToKnockDown)
            {
                slaveController.GoRagdoll(deadTime);
                transform.GetComponent<Rigidbody>().AddForce(Vector3.ClampMagnitude(-1000 * collision.relativeVelocity, 300));

                //if(transform.name)
                //Debug.DrawRay(transform.position, collision.relativeVelocity, Color.red, 2f, true);
                //Debug.Log(Vector3.ClampMagnitude(-1000 * collision.relativeVelocity, 300));
            }
*/
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.name != "FLOOR" && collision.transform.root != this.transform.root)
        {
            slaveController.numberOfCollisions--;
        }
    }

}
