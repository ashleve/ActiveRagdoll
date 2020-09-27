using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionDetector : MonoBehaviour
{
    /// <summary>
    /// This script is attached to every limb with Rigidbody.
    /// It informs SlaveController if collision occured.
    /// It informs MasterController if target is attached. 
    /// It also attaches target to limb using HingeJoint if limb has tag "HAND".
    /// </summary>
    

    private SlaveController slaveController;
    private MasterController masterController;



    void Awake()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        slaveController = setUp.slaveController;
        masterController = setUp.masterController;
    }


    void OnCollisionEnter(Collision collision)
    {
/*        if (collision.gameObject.tag != FLOOR_TAG && collision.transform.root != this.transform.root)
        {
                

        }*/
    }

    void OnCollisionExit(Collision collision)
    {
       /* if (collision.gameObject.tag != FLOOR_TAG && collision.transform.root != this.transform.root)
        {
            slaveController.numberOfCollisions--;
        }*/
    }

}
