using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;

public static class ImportantStages {
    public static Stage PlayAudio(AudioSource audio) {
        return new CodesegStage(() => {StageStatic.stopAllAudio(); audio.Play();}, () => {return !audio.isPlaying;});
    }
    public static Stage PlayAudioFromResult(GeneralTargetStage targetStage, AudioSource success, AudioSource failure) {
        return new StageList(
            targetStage,
            new DecisionStage(() => {return targetStage.userSucceeded;}, PlayAudio(success), PlayAudio(failure))
        );
    }
    public static Stage binarySearchFinalDistance(GeneralTargetStage targetStage, float minSize, float maxSize, float precision) {
        GeneralTargetStage newStage = targetStage.getWithSize((minSize + maxSize)/2);
        if (maxSize - minSize < precision) {
            return new StageList(
                PlayAudio(StageStatic.Audios["smallestVisualAngleAudio"]),
                PlayAudio(StageStatic.Audios["" + Math.Truncate(maxSize)]),
                PlayAudio(StageStatic.Audios["degreesAudio"])
            );
        } else {
            return new StageList(
                newStage,
                new DecisionStage(
                    () => {return newStage.userSucceeded;},
                    binarySearchFinalDistance(targetStage, minSize, (minSize + maxSize)/2, precision),
                    binarySearchFinalDistance(targetStage, (minSize + maxSize)/2, maxSize, precision)
                )
            );
        }
    }
}