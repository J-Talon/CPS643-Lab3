using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputActionListener : MonoBehaviour
{

    public SteamVR_Action_Boolean actionGrip;
    public SteamVR_Action_Boolean actionTeleport;
    public SteamVR_ActionSet actionSet;

    public static bool buttonGripLeft, buttonGripRight, buttonTeleportLeft, buttonTeleportRight;
  
    

    void Awake()
    {
        //name of the actionset
        actionGrip = SteamVR_Actions.default_GrabGrip;
        actionTeleport = SteamVR_Actions.default_Teleport;
        buttonGripLeft = buttonGripRight = buttonTeleportLeft = buttonTeleportRight = false;
    }
    void Start()
    {   //use false if there are multiple actionsets
        actionSet.Activate(SteamVR_Input_Sources.Any, 0,true);
    }

    void Update()
    {
        if (actionGrip.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            buttonGripRight = true;
        }

        if (actionGrip.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            buttonGripRight = false;
        }

        if (actionGrip.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            buttonGripLeft = true;
        }
        
        if (actionGrip.GetStateUp(SteamVR_Input_Sources.LeftHand))
        {
            buttonGripLeft = false;
        }

        if (actionTeleport.GetLastStateDown(SteamVR_Input_Sources.RightHand))
        {
            buttonTeleportRight = true;
        }
        
        if (actionTeleport.GetLastStateUp(SteamVR_Input_Sources.RightHand))
        {
            buttonTeleportRight = false;
        }
        

    }
}
