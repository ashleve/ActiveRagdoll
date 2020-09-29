using UnityEngine;


public class CollisionDetector : MonoBehaviour
{
    /// <summary>
    /// This script is attached automatically to every ragdoll limb with Rigidbody.
    /// It informs SlaveController if collision occured.
    /// </summary>


    // USEFUL VARIABLES
    private SlaveController slaveController;
    private LayerMask layerMask;


    void Start()
    {
        HumanoidSetUp setUp = this.GetComponentInParent<HumanoidSetUp>();
        slaveController = setUp.slaveController;
        layerMask = setUp.dontLooseStrengthLayerMask;
    }

    private bool CheckIfLayerIsInLayerMask(int layer)
    {
        // from https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
        // 1. I believe a layermask is a series of bools(true, false, false, true) but thirty-two of them
        // 2. Those "<<" are telling us to take 1, and move it left x times
        // 3. Then the "|" symbol actually ADDS that 1 at the spot.So if x = 3, we get(true, TRUE, false true)
        // 4. And then we compare "=="
        return layerMask == (layerMask | (1 << layer));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!CheckIfLayerIsInLayerMask(collision.gameObject.layer))
        {
            slaveController.currentNumberOfCollisions++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!CheckIfLayerIsInLayerMask(collision.gameObject.layer))
        {
            slaveController.currentNumberOfCollisions--;
        }
    }

}
