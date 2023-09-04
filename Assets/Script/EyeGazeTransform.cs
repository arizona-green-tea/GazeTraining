using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public class EyeGazeTransform : MonoBehaviour {
    // Start is called before the first frame update
    public GameObject left;
    public GameObject right;
    public GameObject startButton;
    public GameObject target;
    StageList phases;
    int experimentNum;
    char experimentPart;

    void Start() {
        setUpInformation();

        // 1 = chinrest
        // 2 = fixed relative to head
        // 3 = fixed relative to world
        setExperimentNumber(1);
        // A = changing size, distance stays constant
        // B = changing distance, size stays constant
        // C = target moving and changing size/distance/speed
        setExperimentPart('B');

        startExperiment();
    }

    void setUpInformation() {
        GameObject instructions = GameObject.Find("Instruction");
        GameObject cameraRig = GameObject.Find("Camera");
        GameObject audioGameObject = GameObject.Find("AllAudio");
        AudioSource[] allAudio = audioGameObject.GetComponentsInChildren<AudioSource>();
        
        ViveSR.anipal.Eye.EyeDataCol eyeDataCol = GetComponent<ViveSR.anipal.Eye.EyeDataCol>();

        Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>{
            {"left", left},
            {"right", right},
            {"camera", cameraRig},
            {"startButton", startButton},
            {"instructions", instructions},
            {"target", target},
        };
        Dictionary<string, AudioSource> audios = new Dictionary<string, AudioSource>();
        foreach (AudioSource aud in allAudio) {
            audios[aud.name] = aud;
        }
        StageStatic.setInformation(gameObjects, audios, eyeDataCol);
    }
    void setExperimentNumber(int num) { experimentNum = num; }
    void setExperimentPart(char part) { experimentPart = part; }
    void startExperiment() {
        if (experimentNum == 1) {
            StageStatic.relativeToWorld = false;
        } else if (experimentNum == 2) {
            StageStatic.relativeToWorld = false;
        } else if (experimentNum == 3) {
            StageStatic.relativeToWorld = true;
        } else {
            throw new Exception("Experiment number out of range");
        }

        if (experimentPart == 'A') {
            phases = new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.binarySearchFinalSize(
                    new GeneralTargetStage(100, 10, -15, 15, 2, 5), 1, 50, 1
                )
            );
        } else if (experimentPart == 'B') {
            phases = new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.threeDownOneUpFinalDistance(
                    new GeneralTargetStage(100, 10, -15, 15, 2, 5), 1, 50, 1, 0
                )
                // ImportantStages.threeDownOneUpFinalDistance(
                //     new GeneralTargetStage(100, 10, -15, 15, 2, 5), 10, 50, 1, 0
                // )
            );
        } else if (experimentPart == 'C') {
            phases = new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.movingTargetChangingDis(5, 20, 20,
                    (float t) => {return 50 + 10 * (float)Math.Sin(t * 2 * Math.PI/5);},
                    (float t) => {return -30 + 60 * (t/20);},
                    (float t) => {return 30 - 60 * (t/20);}
                )
            );
        } else {
            throw new Exception("Experiment part out of range");
        }
        phases.start();
    }

    // Update is called once per frame
    void Update() {
        if (!phases.finished()) phases.update();
    }
}
