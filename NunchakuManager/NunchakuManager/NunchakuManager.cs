﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Events;
using StressLevelZero.Interaction;
using System;
using ModThatIsNotMod.MonoBehaviours;
using MelonLoader;

namespace NunchakuMod
{
    public class NunchakuManager : MonoBehaviour
    {
#if !UNITY_EDITOR
        // Allows ModThatIsNotMod to inject this mod
        public NunchakuManager(IntPtr intPtr) : base(intPtr) { }
#endif

        public GameObject chakuPrimary = null;
        public LineRenderer lineRenderer = null;
        public GameObject chakuSecondary = null;
        public ConfigurableJoint chakuJoint = null;

        private Color rgbCycle = Color.red;
                
        private void Start()
        {
            // Get object references
            chakuPrimary = transform.Find("Chaku Primary").gameObject;
            lineRenderer = chakuPrimary.GetComponent<LineRenderer>();
            chakuSecondary = transform.Find("Chaku Secondary").gameObject;
            chakuJoint = chakuPrimary.GetComponent<ConfigurableJoint>();            
        }

        void Awake()
        {
            // Allows ModThatIsNotMod to inject this mod
#if !UNITY_EDITOR
            CustomMonoBehaviourHandler.SetFieldValues(this);
#endif
        }

        void drawLine()
        {            
            if (!lineRenderer.enabled)
                return;
            lineRenderer.SetPosition(0, chakuPrimary.transform.position + (chakuPrimary.transform.forward * -.156f));
            lineRenderer.SetPosition(1, chakuSecondary.transform.position + (chakuSecondary.transform.forward * -.156f));
            lineRenderer.material.SetColor("_EmissionColor", rgbCycle);
        }

        void createLink()
        {
            if (chakuJoint != null)
                return;

            // Create the spring joint
            chakuJoint = chakuPrimary.AddComponent<ConfigurableJoint>();

            // Attach it to the secondary chacku
            chakuJoint.connectedBody = chakuSecondary.GetComponent<Rigidbody>();
            chakuJoint.autoConfigureConnectedAnchor = false;
            chakuJoint.anchor = new Vector3(0, 0, -.156f);
            chakuJoint.connectedAnchor = new Vector3(0, 0, -.156f);
            chakuJoint.breakForce = float.PositiveInfinity;
            chakuJoint.breakTorque = float.PositiveInfinity;
            chakuJoint.massScale = 1;
            chakuJoint.connectedMassScale = 1;
            chakuJoint.enablePreprocessing = true;

            // Configurable Joint only
            chakuJoint.xMotion = ConfigurableJointMotion.Limited;
            chakuJoint.yMotion = ConfigurableJointMotion.Limited;
            chakuJoint.zMotion = ConfigurableJointMotion.Limited;
            SoftJointLimitSpring limitSpring = new SoftJointLimitSpring();
            limitSpring.spring = 800;
            limitSpring.damper = 5;
            SoftJointLimit linearLimit = new SoftJointLimit();
            linearLimit.limit = .006f;
            chakuJoint.linearLimitSpring = limitSpring;
            chakuJoint.linearLimit = linearLimit;

            // Spring joint only
            //chakuJoint.spring = 800;
            //chakuJoint.damper = 5;
            //chakuJoint.maxDistance = .006f;
            //chakuJoint.tolerance = .025f;
            

#if DEBUG
            MelonLogger.Msg("Relinking...");
#endif
        }

        public void DoStow()
        {
            // Disable the secondary chaku on stow so it doesn't flop around
            chakuSecondary.SetActive(false);            
            lineRenderer.enabled = false;            

#if DEBUG
            MelonLogger.Msg("Stowing nunchakus...");
#endif
        }

        public void Unstow()
        {
            // Reparent the primary chaku to the RGB Nunchaku object            
            chakuPrimary.transform.SetParent(chakuSecondary.transform.parent);
            chakuSecondary.transform.localPosition = chakuPrimary.transform.localPosition;

            // Activate the chaku and line renderer
            chakuSecondary.SetActive(true);            
            lineRenderer.enabled = true;

            // Relink them
            createLink();

#if DEBUG
            MelonLogger.Msg("Unstowing nunchakus...");
#endif
        }

        float t = 0;

        void Heartbeat()
        {
            t += Time.deltaTime;
            if (t >= 3)
            {
                t = 0;
                MelonLogger.Msg("Heartbeat...");
            }
        }

        void Update()
        {
#if DEBUG
            Heartbeat();
#endif

            // RGB cycle
            Color.RGBToHSV(rgbCycle, out float H, out float S, out float V);
            H = H + .001f <= 1.0f ? H + .001f : 0.0f;
            rgbCycle = Color.HSVToRGB(H, S, V);

            // Update the lights and colors
            chakuPrimary.transform.Find("Models").Find("Tang").GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", rgbCycle);
            chakuPrimary.transform.Find("COM").GetComponent<Light>().color = rgbCycle;
            chakuSecondary.transform.Find("Models").Find("Tang").GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", rgbCycle);
            chakuSecondary.transform.Find("COM").GetComponent<Light>().color = rgbCycle;                

            // Draw the line between the nunchakus
            drawLine();
        }
    }
}