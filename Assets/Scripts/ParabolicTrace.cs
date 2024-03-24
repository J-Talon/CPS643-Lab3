using System;
using UnityEngine;
using Valve.VR;

public class ParabolicTrace : MonoBehaviour
{
    public LineRenderer line;
    public Material mat;
    public SteamVR_Behaviour_Pose controllerPositionRight;
    public GameObject playerPosition;
    public GameObject floor;
    
    private Vector3 direction;
    private Vector3 startLocation;
    private GameObject cylinder;
    
    private const int NUM_POINTS = 50;
    private const float precision = 0.3f;
    
    
    private float parabolaFactor;
    private long lastTimeTeleport;
    private const long TELEPORT_DELAY = 500; // 1/2 of a second


    private Vector3 nextTeleportPos;
        
    void Awake()
    {
        cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.localScale = new Vector3(0.5f, 0.05f, 0.5f);
        cylinder.GetComponent<Renderer>().material = mat;
        Rigidbody body = cylinder.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.detectCollisions = false;
        cylinder.SetActive(false);
        lastTimeTeleport = 0;
    }


    void nextPosition()
    {
        playerPosition.transform.position = nextTeleportPos;
        SteamVR_Fade.View(Color.clear, 0.25f);
    }

    void FixedUpdate()
    {
        if (!InputActionListener.buttonTeleportRight)
        {
            line.enabled = false;
            cylinder.SetActive(false);
            return;
        }
        
        line.enabled = true;
        cylinder.SetActive(true);
        Transform form = controllerPositionRight.transform;
        Vector3 start = form.position;
        Vector3 direction = form.forward;
        RaycastHit? result = tracePositions(start, direction);

        if (result == null)
            return;

        if (!InputActionListener.buttonGripRight)
            return;
        
        long currentMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (currentMilliseconds - lastTimeTeleport < TELEPORT_DELAY)
            return;

        lastTimeTeleport = currentMilliseconds;
        
        Vector3 hitLocation = result.Value.point;
        Vector3 cameraPos = playerPosition.transform.position;
        float yDiff = cameraPos.y - hitLocation.y;

        nextTeleportPos = new Vector3(hitLocation.x, hitLocation.y + yDiff, hitLocation.z);
        SteamVR_Fade.View(Color.black,0.25f);
        Invoke("nextPosition", 0.25f);
    }
    
    
    public RaycastHit? tracePositions(Vector3 startLocation, Vector3 direction)
    {
        this.direction = direction;
        this.startLocation = startLocation;
        
        parabolaFactor =  Math.Max(0.01f, Math.Abs(direction.y) * 5);
        //  parabolaFactor =  Math.Max(0.01f, Math.Abs(direction.y) / 2); 
        return performTrace();
    }
    
    
    
    private float yLevel(float x) {
        return (-1/parabolaFactor) * (float)Math.Pow( x - parabolaFactor, 2) + parabolaFactor; 
        /// return - (parabolaFactor * x * x) + PARABOLA_Y_SHIFT;  //ax^2 + b
    }
    
    
    private float negativeXRoot()
    {
        return 0;
        //  return (float)-Math.Sqrt(PARABOLA_Y_SHIFT / parabolaFactor);
    }
    
    
    private float getStartingPosition()
    {
        Vector2 horizontal = new Vector2(1, 0);
        float horizontalFactor = (float)Math.Sqrt(direction.x * direction.x + direction.z * direction.z);  //1
        Vector2 angle = new Vector2(horizontalFactor, direction.y); //(1, 1)
        angle.Normalize();
        
        float dotProduct = Math.Abs(horizontal.x * angle.x + horizontal.y * angle.y); ///1
                                                                               
        //change where we start in terms of x on the parabola depending on how close it is to the horizontal vector
        float lowerBound = negativeXRoot();
        return lowerBound; // + (lowerBound * -dotProduct); 
    }


    private RaycastHit? performTrace()
    {
        float x = 0;

            //setting the initial point
        Vector3[] points = new Vector3[NUM_POINTS];
        points[0] = startLocation;
        x += precision;

        RaycastHit hitResult = new RaycastHit();
        int hitIndex = NUM_POINTS;
        bool hasHit = false;
        
        
        //get all other points
        for (int i = 1; i < NUM_POINTS; i ++, x += precision) {
            float y = yLevel(x);
            float xPos = x * direction.x;
            float zPos = x * direction.z;

            //next point
            Vector3 position = new Vector3(xPos, y, zPos) + startLocation;
            Vector3 previous = points[i - 1];
            points[i] = position;

            
            //raycast a given distance
            Ray ray = new Ray(previous, position - previous);
            float deltaX, deltaY, deltaZ;
            deltaX = position.x - previous.x;
            deltaY = position.y - previous.y;
            deltaZ = position.z - previous.z;
            
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
            if (Physics.Raycast(ray, out hitResult, distance))
            {
                hitIndex = i;
                
                Vector3[] collision = new Vector3[hitIndex + 1];
                Array.Copy(points, collision, hitIndex + 1);
                points = collision;
                points[hitIndex-1] = hitResult.point;

                if (hitResult.transform.gameObject.Equals(floor))
                {
                    cylinder.transform.position = hitResult.point;
                    cylinder.transform.up = hitResult.normal;
                    cylinder.SetActive(true);
                    hasHit = true;
                }
                break;
            }
            else
            {
                cylinder.SetActive(false);
            }
        }
        
        line.positionCount = hitIndex;
        line.SetPositions(points);


        if (hasHit)
            return hitResult;
        else
            return null;

    }
    
    
}
