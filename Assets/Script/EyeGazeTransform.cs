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
    private StageList _phases;
    private int _experimentNum;
    private char _experimentPart;

    private void Start() {
        SetUpInformation();

        // 1 = chinrest
        // 2 = fixed relative to head
        // 3 = fixed relative to world
        SetExperimentNumber(1);
        
        // A = changing size, distance stays constant
        // B = changing distance, size stays constant
        // C = target moving and changing size/distance/speed
        SetExperimentPart('C');

        StartExperiment();
    }

    private void SetUpInformation() {
        var instructions = GameObject.Find("Instruction");
        var cameraRig = GameObject.Find("Camera");
        var audioGameObject = GameObject.Find("AllAudio");
        var allAudio = audioGameObject.GetComponentsInChildren<AudioSource>();
        
        var eyeDataCol = GetComponent<EyeDataCol>();

        var gameObjects = new Dictionary<string, GameObject>{
            {"left", left},
            {"right", right},
            {"camera", cameraRig},
            {"startButton", startButton},
            {"instructions", instructions},
            {"target", target},
        };
        var audios = new Dictionary<string, AudioSource>();
        foreach (var aud in allAudio) {
            audios[aud.name] = aud;
        }
        StageStatic.setInformation(gameObjects, audios, eyeDataCol);
    }
    
    private void SetExperimentNumber(int num) { _experimentNum = num; }
    
    private void SetExperimentPart(char part) { _experimentPart = part; }
    
    private void StartExperiment()
    {
        StageStatic.relativeToWorld = _experimentNum switch {
            1 => false,
            2 => false,
            3 => true,
            _ => throw new Exception("Experiment number out of range")
        };

        _phases = _experimentPart switch {
            'A' => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.findThreshold(0, 0, 2, 5, 80, false)
            ),
            'B' => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.findThreshold(0, 0, 2, 5, 80, true)
            ),
            'C' => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.movingTargetChangingDis(5, 20, 20,
                    t => 50 + 10 * Math.Sin(t * 2 * Math.PI/5),
                    t => -30 + 60 * (t/20),
                    t => 30 - 60 * (t/20)
                )
            ),
            _ => throw new Exception("Experiment part out of range")
        };
        
        _phases.start();
    }

    private void Update() {
        if (!_phases.finished()) _phases.update();
    }
}
