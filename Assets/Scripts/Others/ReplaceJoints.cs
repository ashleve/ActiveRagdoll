using UnityEngine;
using System.Collections;

namespace AnimFollow
{
	[ExecuteInEditMode]
	public class ReplaceJoints : MonoBehaviour
	{
		// Drag and drop this on a ragdoll to replace all charater joints with configurable joints set up with the same rotational limits aas the character joints had.

		void Start ()
		{
			CharacterJoint[] charJoints = GetComponentsInChildren<CharacterJoint>();
			int i = 0;
			foreach(CharacterJoint charJoint in charJoints)
			{
				ConfigurableJoint confJoint;
				if (!charJoint.transform.GetComponent<ConfigurableJoint>())
				{
					i++;
					confJoint = charJoint.gameObject.AddComponent<ConfigurableJoint>() as ConfigurableJoint;
	//				confJoint.autoConfigureConnectedAnchor = false;
					confJoint.connectedBody = charJoint.connectedBody;
					confJoint.anchor = charJoint.anchor;
					confJoint.axis = charJoint.axis;
	//				confJoint.connectedAnchor = charJoint.connectedAnchor;
					confJoint.secondaryAxis = charJoint.swingAxis;
					confJoint.xMotion = ConfigurableJointMotion.Locked;
					confJoint.yMotion = ConfigurableJointMotion.Locked;
					confJoint.zMotion = ConfigurableJointMotion.Locked;
					confJoint.angularXMotion = ConfigurableJointMotion.Limited;
					confJoint.angularYMotion = ConfigurableJointMotion.Limited;
					confJoint.angularZMotion = ConfigurableJointMotion.Limited;
					confJoint.lowAngularXLimit = charJoint.lowTwistLimit;
					confJoint.highAngularXLimit = charJoint.highTwistLimit;
					confJoint.angularYLimit = charJoint.swing1Limit;
					confJoint.angularZLimit = charJoint.swing2Limit;
					confJoint.rotationDriveMode = RotationDriveMode.Slerp;

	//				JointDrive temp = confJoint.slerpDrive; // These are left here to remind us how to set the drive
	//				temp.mode = JointDriveMode.Position;
	//				temp.positionSpring = 0f;
	//				confJoint.slerpDrive = temp;
	//				confJoint.targetRotation = Quaternion.identity;
				}
				DestroyImmediate(charJoint);
			}
			Debug.Log("Replaced " + i + " CharacterJoints with ConfigurableJoints on " + this.name);
			DestroyImmediate(this);
		}
	}
}
