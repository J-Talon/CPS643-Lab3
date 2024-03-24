
using UnityEngine;
using Valve.VR;

public class ControllerHolding : MonoBehaviour
{
    public SteamVR_Behaviour_Pose position;
    public bool isRight;
    
    private GameObject holdingObject;
    private Rigidbody controllerRigidbody;
    private DynamicObject script;
    

    void Awake()
    {
        controllerRigidbody = gameObject.GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        if (isRight)
        {
            if (!InputActionListener.buttonGripRight && isHolding())
                release();   
        }
        else
        {
            if (!InputActionListener.buttonGripLeft && isHolding())
                release();
        }
    }

    private void SetHoldingObject(GameObject mesh)
    {
        this.holdingObject = mesh;
    }

    public GameObject getHolding()
    {
        return holdingObject;
    }

    public bool isHolding()
    {
        return this.holdingObject != null;
    }
    
    public void startHolding(GameObject mesh)
    {
       DynamicObject scriptDynamicObject = mesh.GetComponent<DynamicObject>();
       if (!scriptDynamicObject)
           return;

       this.script = scriptDynamicObject;
            
        SetHoldingObject(mesh);
        Rigidbody body = mesh.GetComponent<Rigidbody>();
        
        if ( body == null)
         mesh.AddComponent<Rigidbody>();
        
        FixedJoint joint; 
        joint = holdingObject.AddComponent<FixedJoint>();
        joint.breakForce = 1000;  //newtons
        joint.breakTorque = 1000;
        
        ///gameobject is the controller
        joint.connectedBody = controllerRigidbody;
    }
    
    
    public void release()
    {

        if (holdingObject == null)
            return;
        
        FixedJoint joint = holdingObject.GetComponent<FixedJoint>();
        Destroy(joint);
        
        //we need to use our own velocity since isKinematic makes the controller velocity {0,0,0}
        Rigidbody heldRigidBody = holdingObject.GetComponent<Rigidbody>();
        if (heldRigidBody != null && script != null)
        {
            heldRigidBody.velocity = position.GetVelocity();
            heldRigidBody.angularVelocity = position.GetAngularVelocity();
        }


        SetHoldingObject(null);
    }
}
