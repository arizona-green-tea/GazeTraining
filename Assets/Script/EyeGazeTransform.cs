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
    private DartboardPositioning _dartboardPositioning;
    private DartboardMovementType _dartboardMovementType;
    private bool _testIpd;

    private void Start() {
        SetUpInformation();
        
        // BEGIN: INSTRUCTIONS SPECIFIC TO EXPERIMENT
        SetExperiment(
            true, // <-- This is whether or not there we are using the VR headset right now; false if remote testing
            DartboardPositioning.Chinrest, 
            DartboardMovementType.TargetMovingAndChangingDistance,
            true, // <-- Input whether or not you want to use the IPD calculator program.
            -1 // <-- This is the IPD. If known, input here. Otherwise, put -1.
        );
        // END: INSTRUCTIONS SPECIFIC TO EXPERIMENT

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

    private void SetExperiment(bool usingVRHeadset, DartboardPositioning positioning, DartboardMovementType movementType, bool testIpd, double ipd) {
        StageStatic.hasActiveUser = usingVRHeadset;
        _dartboardPositioning = positioning;
        _dartboardMovementType = movementType;
        if (ipd > 0) StageStatic.IPD = (float) ipd;
        _testIpd = testIpd;
    }
    
    private void StartExperiment()
    {
        StageStatic.relativeToWorld = _dartboardPositioning switch {
            DartboardPositioning.Chinrest => false,
            DartboardPositioning.FixedRelativeToHead => false,
            DartboardPositioning.FixedRelativeToWorld => true,
            _ => throw new Exception("Experiment Positioning is Invalid")
        };

        _phases = _dartboardMovementType switch {
            DartboardMovementType.ChangingSize => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.findThreshold(0, 0, 2, 5, 80, false)
            ),
            DartboardMovementType.ChangingDistance => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.findThreshold(0, 0, 2, 5, 80, true)
            ),
            DartboardMovementType.TargetMovingAndChangingDistance => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.movingTargetChangingDis(5, 20, 20,
                    t => 50 + 10 * Math.Sin(t * 2 * Math.PI/5),
                    t => -30 + 60 * (t/20),
                    t => 30 - 60 * (t/20)
                )
            ),
            DartboardMovementType.TargetMovingAndChangingSize => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.movingTargetChangingSize(5, 20, 20,
                    t => 50 + 10 * Math.Sin(t * 2 * Math.PI/5),
                    t => -30 + 60 * (t/20),
                    t => 30 - 60 * (t/20)
                )
            ),
            _ => throw new Exception("Experiment Movement is Invalid")
        };

        if (_testIpd) {
            _phases = new StageList(
                new IPDSetupStage(),
                _phases
            );
        }

        _phases.start();
    }

    private void Update() {
        if (!_phases.finished()) _phases.update();
    }

    private enum DartboardPositioning {
        Chinrest,
        FixedRelativeToHead,
        FixedRelativeToWorld
    }

    private enum DartboardMovementType {
        ChangingSize,
        ChangingDistance,
        TargetMovingAndChangingSize,
        TargetMovingAndChangingDistance
    }
}
