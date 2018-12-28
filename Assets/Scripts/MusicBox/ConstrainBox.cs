using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEngine;

public class ConstrainBox : MonoBehaviour
{
    [SerializeField]
    private InteractionBehaviour _intObj;

    void OnEnable() {
        _intObj = GetComponent<InteractionBehaviour>();
        // Prevent double subscription.
        _intObj.manager.OnPostPhysicalUpdate -= applyXAxisWallConstraint;
        _intObj.manager.OnPostPhysicalUpdate += applyXAxisWallConstraint;
        _intObj.AddAngularAcceleration(new Vector3(1,0,0));
    }
    void OnDisable() {
        _intObj.manager.OnPostPhysicalUpdate -= applyXAxisWallConstraint;
    }
    private void applyXAxisWallConstraint() {
        // This constraint forces the interaction object to have a positive X coordinate.
        Vector3 objPos = _intObj.rigidbody.position;
        if (objPos.x < 0F) {
            objPos.x = 0F;
            _intObj.rigidbody.position = objPos;
            // Zero out any negative-X velocity when the constraint is applied.
            Vector3 objVel = _intObj.rigidbody.velocity;
            if (objVel.x < 0F) {
                objVel = new Vector3(0F, 0F, 0F);
                _intObj.rigidbody.velocity = objVel;
            }
        }
    }
}
