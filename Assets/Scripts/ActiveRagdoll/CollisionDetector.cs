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

    public string STATE = "NOT_CONNECTED";
    private string TARGET_TAG = "TARGET";
    private string ATTACH_TARGET_TO_OBJECT_WITH_TAG = "HAND";


    // Start is called before the first frame update
    void Awake()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        slaveController = setUp.GetSlaveController();
        masterController = setUp.GetMasterController();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "FLOOR" && collision.transform.GetComponentInParent<SlaveController>() != slaveController)
        {
           /* if(&& collision.transform.tag == TARGET_TAG)
            slaveController.numberOfCurrentCollisions++;*/

            if (STATE != "CONNECTED"  && collision.transform.tag == TARGET_TAG && this.transform.tag == ATTACH_TARGET_TO_OBJECT_WITH_TAG && collision.gameObject.GetComponent<Target>().numOfAttached < 2)
            {

                collision.gameObject.GetComponent<Target>().numOfAttached += 1;

                if (!masterController.targetAttached)
                {
                    masterController.TargetAttached(true);
                }

                var hingeJoint = this.gameObject.AddComponent<HingeJoint>();
                var otherBody = collision.gameObject.GetComponent<Rigidbody>();
                otherBody.mass = 1f;
                //hingeJoint.breakForce = 1000;
                //hingeJoint.breakTorque = 1000;
                hingeJoint.connectedBody = otherBody;
                STATE = "CONNECTED";

                masterController.numberOfAttachments += 1;
                masterController.DisableIK();
            }

        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag != "FLOOR" && collision.transform.GetComponentInParent<SlaveController>() != slaveController)
        {
            slaveController.numberOfCurrentCollisions--;
        }
    }

    public void dropTarget()
    {
        if (STATE == "NOT_CONNECTED") return;

        HingeJoint[] hjArr = this.GetComponents<HingeJoint>();
        foreach (HingeJoint hj in hjArr)
        {
            Destroy(hj);
        }
        masterController.TargetAttached(false);

        STATE = "NOT_CONNECTED";
    }

}
