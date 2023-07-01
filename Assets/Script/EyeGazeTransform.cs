using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public class EyeGazeTransform : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject left;
    public GameObject right;
    public GameObject startButton;
    public GameObject target;
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;
    public GameObject target5;
    public GameObject[] targets;
    public GameObject instruction;
    public GameObject cameraRig;
    public AudioSource[] allAudio;
    private ViveSR.anipal.Eye.EyeDataCol eyeDataCol;
    public float timeRemaining = 5.1f;
    public float timeRemainingInstruction = 10.0f;
    public bool timerOn = false;
    private Vector3 leftPos;
    private Vector3 rightPos;
    string lObjectName = "N/A";
    string rObjectName = "N/A";
    int phaseNum = 1; // 1 = startButton has not been pressed yet, 2 = startButton pressed and instructions on, 3 = instructions done and testing
    int stage = 1; // the current dartboard level the user is on
    Color target1Default;
    Color target2Default;
    Color target3Default;
    Color target4Default;
    Color target5Default;

    public bool vrConnected = false;
    public bool hasActiveUser = false;

    void Start()
    {
        // instructionsAudio = AudioSource.Find("instructionsAudio");
        targets = new GameObject[]{target1, target2, target3, target4, target5};
        instruction = GameObject.Find("Instruction");
        cameraRig = GameObject.Find("Camera");

        if (vrConnected)
        {
            target.transform.localScale = Vector3.Scale(target.transform.localScale, (new Vector3(1, 1, -1)));
            instruction.transform.localScale = Vector3.Scale(instruction.transform.localScale, (new Vector3(-1, 1, 1)));
            startButton.transform.localScale = Vector3.Scale(startButton.transform.localScale, (new Vector3(-1, 1, 1)));
        }
        
        eyeDataCol = GetComponent<ViveSR.anipal.Eye.EyeDataCol>();
        target1Default = target1.GetComponent<Renderer>().material.color;
        target2Default = target2.GetComponent<Renderer>().material.color;
        target3Default = target3.GetComponent<Renderer>().material.color;
        target4Default = target4.GetComponent<Renderer>().material.color;
        target5Default = target5.GetComponent<Renderer>().material.color;

        allAudio = target5.GetComponentsInChildren<AudioSource>();

        startButton.SetActive(true);
        instruction.SetActive(false);
        target.SetActive(false);
        phaseNum = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = cameraRig.transform.position;
        Vector3 cameraAng = cameraRig.transform.localRotation.eulerAngles;

        moveToView(cameraPos, cameraAng, startButton, new Vector3(0, 0, -60));
        moveToView(cameraPos, cameraAng, instruction, new Vector3(0, 0, -50));
        moveToView(cameraPos, cameraAng, target, new Vector3(0, 0, -200), -15, 15);


        // first phase, with the user not yet viewing the start button
        if (phaseNum == 1 && ((vrConnected && (eyeDataCol.lObjectName == "StartButton" || eyeDataCol.rObjectName == "StartButton")) || Input.GetKeyDown(KeyCode.A))) {
            Debug.Log("Starting game...");
            startButton.SetActive(false);
            target.SetActive(false);
            instruction.SetActive(true);
            phaseNum = 2;
            timeRemainingInstruction = 10.0f;
        }

        // second phase, with the user reading the instructions
        if (phaseNum == 2)
        {
            if (timeRemainingInstruction > 0)
            {
                timeRemainingInstruction -= Time.deltaTime;

            }
            else
            {
                instruction.SetActive(false);
                target.SetActive(true);
                phaseNum = 3;
                timeRemaining = 5.1f;
                stage = 1;
                timerOn = false;
                GetAudio("stage1Audio").Play();
            }
        }

        // third and final phase, with the user trying to look at the dartboard
        if (phaseNum == 3)
        {
            if (eyeDataCol.worldPosL != new Vector3(0, 0, 0))
            {
                left.SetActive(true);
                left.transform.position = eyeDataCol.worldPosL;
            }
            if (eyeDataCol.worldPosR != new Vector3(0, 0, 0))
            {
                right.SetActive(true);
                right.transform.position = eyeDataCol.worldPosR;
            }

            if (timeRemaining > 0 && timerOn)
            {
                timeRemaining -= Time.deltaTime;
                for (int i = 1; i <= 5; i++) {
                    if (timeRemaining < i && Time.deltaTime + timeRemaining >= i) {
                        // play the corresponding audio
                        string[] audios = new string[] {"oneAudio", "twoAudio", "threeAudio", "fourAudio", "fiveAudio"};
                        for (int j = 1; j <= 5; j++) {
                            GetAudio(audios[j-1]).Stop();
                            if(i == j) GetAudio(audios[j-1]).Play();
                        }
                    }
                }
            }
            else if (timerOn)
            {
                //lObjectName = eyeDataCol.hitInfoL.collider.gameObject.name;
                //rObjectName = eyeDataCol.hitInfoR.collider.gameObject.name
                timeRemaining = 5.1f;
                stage += 1;
                if (stage == 6) {
                    phaseNum = 4;
                    GetAudio("completionAudio").Play();
                    return;
                }
                targets[stage-2].SetActive(false);

                GetAudio("stage" + stage.ToString() + "Audio").Play();

                timerOn = false;

                Debug.Log("Current stage: " + stage);
            }
            int leftNum = -1;
            int rightNum = -1;
            if (eyeDataCol.lObjectName.Length > 6)
            {
                int.TryParse(eyeDataCol.lObjectName.Substring(6), out leftNum);
            }
            if (eyeDataCol.rObjectName.Length > 6)
            {
                int.TryParse(eyeDataCol.rObjectName.Substring(6), out rightNum);
            }
            //Debug.Log(Vector3.Distance(leftPos, eyeDataCol.worldPosL));

            // TODO: ADD THIS BACK
            if ((vrConnected && hasActiveUser && (leftNum < stage || rightNum < stage)) || Input.GetKeyDown(KeyCode.A))
            {
                timeRemaining = 5.1f;
                timerOn = false;
            } else if (!timerOn && !GetAudio("stage" + stage.ToString() + "Audio").isPlaying) {
                timerOn = true;
            }
        }
    }

    AudioSource GetAudio(string name) {
        foreach (AudioSource a in allAudio) {
            if (a.name == name) return a;
        }
        return null;
    }

    void moveToView(Vector3 cameraPos, Vector3 cameraAng, GameObject obj, Vector3 distance) {
        obj.transform.eulerAngles = cameraAng;
        distance = Quaternion.Euler(-cameraAng.x, cameraAng.y + 180, cameraAng.z) * distance;
        obj.transform.position = cameraPos + distance;
    }

    void moveToView(Vector3 cameraPos, Vector3 cameraAng, GameObject obj, Vector3 distance, float xAng, float yAng) {
        if (xAng == 0 && yAng == 0) {
            moveToView(cameraPos, cameraAng, obj, distance);
        }
        
        obj.transform.LookAt(cameraPos);

        distance += new Vector3((float)(Math.Abs(distance.z) * Math.Tan(xAng * Math.PI/180)), (float)(Math.Abs(distance.z) * Math.Tan(yAng * Math.PI/180)), 0);
        distance *= Math.Abs(distance.z)/distance.magnitude;
        distance = Quaternion.Euler(-cameraAng.x, cameraAng.y + 180, -cameraAng.z) * distance;
        obj.transform.position = cameraPos + distance;
    }
}
