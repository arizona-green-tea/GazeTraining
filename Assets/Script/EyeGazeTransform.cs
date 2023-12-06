using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public class EyeGazeTransform : MonoBehaviour {
    public GameObject left;
    public GameObject right;
    public GameObject startButton;
    public GameObject target;

    public GameObject setupEnvironment;
    private StageList _phases;
    private DartboardPositioning _dartboardPositioning;
    private DartboardMovementType _dartboardMovementType;
    private bool _testIpd, _usingLeftEye;

    private void Start() {
        SetUpInformation();
        
        // BEGIN: INSTRUCTIONS SPECIFIC TO EXPERIMENT
        SetExperiment(
            true, // <-- true if using the VR headset (in-person), false otherwise (remote testing)
            DartboardPositioning.Chinrest, 
            DartboardMovementType.TargetMovingAndChangingDistance,
            true, // <-- Input whether or not you want to use the IPD calculator program.
            true, // <-- true if user will use left eye for the IPD test, false otherwise
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
            {"setupenvironment", setupEnvironment}
        };
        var audios = new Dictionary<string, AudioSource>();
        foreach (var aud in allAudio) {
            audios[aud.name] = aud;
        }
        StageStatic.setInformation(gameObjects, audios, eyeDataCol);
    }

    private void SetExperiment(bool usingVRHeadset, DartboardPositioning positioning, 
        DartboardMovementType movementType, bool testIpd, bool usingLeftEyeForIpdTest, double ipd) {
        StageStatic.HasActiveUser = usingVRHeadset;
        _dartboardPositioning = positioning;
        _dartboardMovementType = movementType;
        _usingLeftEye = usingLeftEyeForIpdTest;
        if (ipd > 0) {
            StageStatic.leftIPD = (float)ipd;
            StageStatic.rightIPD = (float)ipd;
        }
        _testIpd = testIpd;
    }
    
    private void StartExperiment() {
        StageStatic.RelativeToWorld = _dartboardPositioning switch {
            DartboardPositioning.Chinrest => false,
            DartboardPositioning.FixedRelativeToHead => false,
            DartboardPositioning.FixedRelativeToWorld => true,
            _ => throw new Exception("Experiment Positioning is Invalid")
        };

        _phases = _dartboardMovementType switch {
            DartboardMovementType.ChangingSize => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.FindThreshold(0, 0, 2, 5, 80, false)
            ),
            DartboardMovementType.ChangingDistance => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.FindThreshold(0, 0, 2, 5, 80, true)
            ),
            DartboardMovementType.TargetMovingAndChangingDistance => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.MovingTargetChangingDis(5, 20, 20,
                    t => 50 + 10 * Math.Sin(t * 2 * Math.PI/5),
                    t => -30 + 60 * (t/20),
                    t => 30 - 60 * (t/20)
                )
            ),
            DartboardMovementType.TargetMovingAndChangingSize => new StageList(
                new StartButtonStage(),
                new InstructionStage(),
                ImportantStages.MovingTargetChangingSize(5, 20, 20,
                    t => 50 + 10 * Math.Sin(t * 2 * Math.PI/5),
                    t => -30 + 60 * (t/20),
                    t => 30 - 60 * (t/20)
                )
            ),
            _ => throw new Exception("Experiment Movement is Invalid")
        };

        if (_testIpd) {
            _phases = new StageList(
                new IPDSetupStage(_usingLeftEye, 0.001f),
                _phases
            );
        }

        _phases.Start();
    }

    private void Update() {
        if (!_phases.Finished()) _phases.Update();
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
