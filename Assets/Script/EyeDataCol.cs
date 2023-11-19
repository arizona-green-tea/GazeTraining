﻿//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;


namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class EyeDataCol : MonoBehaviour
            {
                public int LengthOfRay = 25;
                public string Condition = "GazeTraining";
                [SerializeField] public GameObject CenterEye;
                public Camera camL, camR, camC;

                //[SerializeField] private LineRenderer GazeRayRenderer;
                private static EyeData_v2 eyeData = new EyeData_v2();
                private bool eye_callback_registered = false;

                // data to save
                public Vector3 screenPosC;
                public Vector3 screenPosL;
                public Vector3 screenPosR;
                public Vector3 worldPosC;
                public Vector3 worldPosL;
                public Vector3 worldPosR;
                public Vector3 GazeDirectionCombined;
                public Vector3 GazeOriginCombinedLocal;
                private Vector3 GazeDirectionCombinedLocal;
                public RaycastHit hitInfoC, hitInfoL, hitInfoR;
                public Vector3 L_Origin;
                public Vector3 R_Origin;
                public Vector3 L_Direction;
                public Vector3 R_Direction;
                public string lObjectName = "N/A";
                public string rObjectName = "N/A";
                public string cObjectName = "N/A";
                private StreamWriter sw;

                public Camera camera;

                private void Start()
                {
                    

                    sw = File.AppendText(Condition + "_" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + "GazeDataAll.txt");

                    string textAll = "Time" + ", " + "L Pixel X " + " , " + "L Pixel Y " + " , " + "L Pixel Z" +
                    ", " + "L hit point in Z" + " , " + "L Looking At" + ", " + "L World X" + ", " + "L World Y" + ", " + "R Pixel X " + ", " + "R Pixel Y " + " , " + "R Pixel Z" +
                    ", " + "R hit point in Z" + " , " + "R Looking At" + ", " + "R World X" + ", " + "R World Y" + ", " + "C Pixel X " + ", " + "C Pixel Y " + " , " + "C Pixel Z" +
                    ", " + "C hit point in Z" + " , " + "C Looking At" + ", " + "C World X" + ", " + "C World Y" + ", " + "L Origin X " + ", " + "L Origin Y " + " , " + "L Origin Z" +
                    ", " + "R Origin X " + " , " + "R Origin Y " + " , " + "R Origin Z" + "\n";
                    sw.Write(textAll);

                    

                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    //Assert.IsNotNull(GazeRayRenderer);

                    camC = CenterEye.GetComponent<Camera>();


                }

                private void Update()
                {

                    //if (trial < spawnScript.getTrialNumber()) {}
                    GetGazeData();
                    //GazeRayRenderer.SetPosition(0, camC.transform.position - camC.transform.up * 0.05f);
                    //GazeRayRenderer.SetPosition(1, hitInfoC.point);

                }


                

                private void GetGazeData()
                {
                    //await Task.Delay(TimeSpan.FromSeconds(0));


                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }

                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData))
                        {
                            SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out L_Origin, out L_Direction, eyeData);
                            SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out R_Origin, out R_Direction, eyeData);

                        }
                        else
                        {
                            return;
                        }

                    }

                    else
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
                        {
                            SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out L_Origin, out L_Direction);
                            SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out R_Origin, out R_Direction);
                        }
                        else
                        {
                            return;
                        }
                    }

                    

                    camL = camC;
                    Vector3 oldL = camL.transform.position;
                    camL.transform.position = camL.transform.TransformPoint(-StageStatic.IPD, 0, 0);
                    // camL.transform.position += new Vector3(-StageStatic.IPD, 0, 0);
                    Vector3 GazeDirectionTestL = camL.transform.TransformDirection(L_Direction);
                    if (Physics.Raycast(camL.transform.position, GazeDirectionTestL, out hitInfoL, 1000))
                    {
                        Debug.Log("OldL: " + oldL + " NewL: " + camL.transform.position + " Adjusted by: " + -StageStatic.IPD);
                        lObjectName = hitInfoL.collider.gameObject.name;
                        screenPosL = camL.WorldToScreenPoint(hitInfoL.point, Camera.MonoOrStereoscopicEye.Mono);
                        worldPosL = hitInfoL.point;
                        screenPosL.z = hitInfoL.point.z;
                    }

                    camR = camC;
                    Vector3 oldR = camR.transform.position;
                    camR.transform.position = camR.transform.TransformPoint(StageStatic.IPD, 0, 0);
                    // camR.transform.position += new Vector3(StageStatic.IPD, 0, 0);
                    Vector3 GazeDirectionTestR = camR.transform.TransformDirection(R_Direction);
                    if (Physics.Raycast(camR.transform.position, GazeDirectionTestR, out hitInfoR, 1000))
                    {
                        Debug.Log("OldR: " + oldR + " NewR: " + camR.transform.position + " Adjusted by: " + -StageStatic.IPD);
                        rObjectName = hitInfoR.collider.gameObject.name;
                        screenPosR = camR.WorldToScreenPoint(hitInfoR.point, Camera.MonoOrStereoscopicEye.Mono);
                        worldPosR = hitInfoR.point;
                        screenPosR.z = hitInfoR.point.z;
                    }

                    Vector3 GazeDirectionTestC = camC.transform.TransformDirection(GazeDirectionCombinedLocal);
                    if (Physics.Raycast(camC.transform.position, GazeDirectionTestC, out hitInfoC, 1000))
                    {
                        cObjectName = hitInfoC.collider.gameObject.name;
                        screenPosC = camC.WorldToScreenPoint(hitInfoC.point, Camera.MonoOrStereoscopicEye.Mono);
                        worldPosC = hitInfoC.point;
                        screenPosC.z = hitInfoC.point.z;
                    }

                    string textAll = System.DateTime.Now.Ticks.ToString()  + ", " + screenPosL.x + ", " + screenPosL.y + ", " + screenPosL.z +
                    ", " + hitInfoL.distance + ", " + lObjectName + ", " + worldPosL.x + ", " + worldPosL.y + ", " + screenPosR.x + ", " + screenPosR.y + ", " + screenPosR.z +
                    ", " + hitInfoR.distance + ", " + rObjectName + ", " + worldPosR.x + ", " + worldPosR.y + ", " + screenPosC.x + ", " + screenPosC.y + ", " + screenPosC.z +
                    ", " + hitInfoC.distance + ", " + cObjectName + ", " + worldPosC.x + ", " + worldPosC.y + ", " + L_Origin.x + ", " + L_Origin.y + ", " + L_Origin.z +
                     ", " + R_Origin.x + ", " + R_Origin.y + ", " + R_Origin.z + "\n";
                    
                    sw.Write(textAll);

                    
                }

                private void OnDisable()
                {
                    Release();
                }

                void OnApplicationQuit()
                {
                    Release();
                }

                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }
                }

                private static void EyeCallback(ref EyeData_v2 eye_data)
                {
                    eyeData = eye_data;
                }
            }
        }
    }
}
