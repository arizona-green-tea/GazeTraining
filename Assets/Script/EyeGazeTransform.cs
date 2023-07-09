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
    // public GameObject target1;
    // public GameObject target2;
    // public GameObject target3;
    // public GameObject target4;
    // public GameObject target5;
    
    StageList phases;

    void Start() {
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

        // phases = new StageList(
        //     new StartButtonStage(),
        //     new InstructionStage(),
        //     new CodesegStage(() => {audios["stage1Audio"].Play();}, () => {return !audios["stage1Audio"].isPlaying;}),
        //     new GeneralTargetStage(100, 10, -15, 15, 5, 30),
        //     new CodesegStage(() => {audios["completionAudio"].Play();}, () => {return !audios["completionAudio"].isPlaying;})
        // );
        phases = new StageList(
            new StartButtonStage(),
            new InstructionStage(),
            ImportantStages.binarySearchFinalDistance(
                new GeneralTargetStage(100, 10, -15, 15, 2, 5), 5, 45, 1
            )
        );

        phases.start();
    }

    // Update is called once per frame
    void Update() {
        if (!phases.finished()) phases.update();
    }
}
