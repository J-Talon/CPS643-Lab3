
using UnityEngine;



//Dynamic object is an object which listens for the controller to enter it's collider
public class DynamicObject : MonoBehaviour
{
    //extern
    public GameObject controllerLeft;
    public GameObject controllerRight;
  
    private ControllerHolding controllerManagerLeft;
    private ControllerHolding controllerManagerRight;


 
    private void Awake()
    {
       this.controllerManagerRight = controllerRight.GetComponent<ControllerHolding>();
       this.controllerManagerLeft = controllerLeft.GetComponent<ControllerHolding>();
    }
    
    //other must the controller
    void OnTriggerEnter(Collider other)
    {
        
        if (InputActionListener.buttonGripRight)
        {
            if (!other.gameObject.Equals(controllerRight))
                return;
            
            if (controllerManagerRight.isHolding())
                return;
        
            controllerManagerRight.startHolding(gameObject);
            
        }
        else
        if (InputActionListener.buttonGripLeft)
        {
            if (!other.gameObject.Equals(controllerLeft))
                return;

            if (controllerManagerLeft.isHolding())
                return;
            
            controllerManagerLeft.startHolding(gameObject);

        }
        
    }

    
    
    private void OnTriggerStay(Collider other)
    {

        if (InputActionListener.buttonGripRight)
        {
            if (controllerManagerRight.isHolding())
                return;
            
            controllerManagerRight.startHolding(gameObject);
        }
        else
        if (InputActionListener.buttonGripLeft)
        {
            if (controllerManagerLeft.isHolding())
                return;
            
            controllerManagerLeft.startHolding(gameObject);
        }
        
        
    }

    private void OnJointBreak(float breakForce)
    {
        if (gameObject.Equals(controllerManagerRight.getHolding()))
            controllerManagerRight.release();

        if (gameObject.Equals(controllerManagerLeft.getHolding()))
            controllerManagerLeft.release();
        
    }
}
