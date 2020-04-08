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
    private AnimationFollowing animFollow;

    private LimbState state = LimbState.TARGET_NOT_ATTACHED;

    private static string TARGET_TAG = "BOX";
    private static string ATTACH_TARGET_TO_OBJECT_WITH_TAG = "HAND";
    private static string FLOOR_TAG = "FLOOR";

    private static float FORCE_NEEDED_TO_DIE = 15;
    private static float TIME_OF_BEING_DEAD = 3;

    private static float FORCE_NEEDED_TO_DROP_TARGET = 10000;


    void Awake()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        slaveController = setUp.GetSlaveController();
        masterController = setUp.GetMasterController();
        animFollow = setUp.GetAnimationFollowing();
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != FLOOR_TAG && collision.transform.root != this.transform.root)
        {

            slaveController.numberOfCollisions++;

            if (state != LimbState.TARGET_ATTACHED && collision.gameObject.tag == TARGET_TAG && this.gameObject.tag == ATTACH_TARGET_TO_OBJECT_WITH_TAG) 
            {
                if (!collision.gameObject.GetComponent<Box>().isTaken || collision.gameObject.GetComponent<Box>().hostID == transform.root.GetInstanceID())
                {
                    var hingeJoint = gameObject.AddComponent<HingeJoint>(); 
                    var otherBody = collision.gameObject.GetComponent<Rigidbody>(); 
                    hingeJoint.connectedBody = otherBody;
                    hingeJoint.breakForce = FORCE_NEEDED_TO_DROP_TARGET;
                    hingeJoint.breakTorque = FORCE_NEEDED_TO_DROP_TARGET; 


                    masterController.handsConnected += 1;
                    masterController.DisableIK();

                    collision.gameObject.GetComponent<Box>().isTaken = true;
                    collision.gameObject.GetComponent<Box>().hostID = transform.root.GetInstanceID();

                    state = LimbState.TARGET_ATTACHED;
                }
            }

            // Die hit by enough force
            float collisionSpeed = collision.relativeVelocity.magnitude;
            if (collisionSpeed >= FORCE_NEEDED_TO_DIE)
            {
                slaveController.Die(TIME_OF_BEING_DEAD);
                transform.GetComponent<Rigidbody>().AddForce(Vector3.ClampMagnitude(-1000 * collision.relativeVelocity, 500));

                Debug.DrawRay(transform.position, collision.relativeVelocity, Color.yellow, 2f, true);
                //Debug.Log(Vector3.ClampMagnitude(-1000 * collision.relativeVelocity, 300));
            }

        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != FLOOR_TAG && collision.transform.root != this.transform.root)
        {
            slaveController.numberOfCollisions--;
        }
    }


    public void DropTargets()
    {
        if (state == LimbState.TARGET_NOT_ATTACHED) 
            return;

        HingeJoint[] hjArr = this.GetComponents<HingeJoint>();
        foreach (HingeJoint hj in hjArr)
        {
            Destroy(hj);
            masterController.handsConnected--;
        }

        state = LimbState.TARGET_NOT_ATTACHED;
    }

}

public enum LimbState
{
    TARGET_NOT_ATTACHED,
    TARGET_ATTACHED
}
