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
    public static Stage binarySearchFinalSize(GeneralTargetStage targetStage, float minSize, float maxSize, float precision) {
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
                    binarySearchFinalSize(targetStage, minSize, (minSize + maxSize)/2, precision),
                    binarySearchFinalSize(targetStage, (minSize + maxSize)/2, maxSize, precision)
                )
            );
        }
    }
    public static Stage binarySearchFinalDistance(GeneralTargetStage targetStage, float minDis, float maxDis, float precision) {
        GeneralTargetStage newStage = targetStage.getWithDistance((minDis + maxDis)/2);
        if (maxDis - minDis < precision) {
            return new StageList(
                PlayAudio(StageStatic.Audios["smallestVisualAngleAudio"]),
                PlayAudio(StageStatic.Audios["" + Math.Truncate(maxDis)]),
                PlayAudio(StageStatic.Audios["degreesAudio"])
            );
        } else {
            return new StageList(
                newStage,
                new DecisionStage(
                    () => {return newStage.userSucceeded;},
                    binarySearchFinalDistance(targetStage, minDis, (minDis + maxDis)/2, precision),
                    binarySearchFinalDistance(targetStage, (minDis + maxDis)/2, maxDis, precision)
                )
            );
        }
    }
    public static Stage movingTargetChangingDis(float userViewTime, float maxTime, float stSize, Func<float, float> distance, Func<float, float> xAng, Func<float, float> yAng) {
        GeneralTargetStage mainStage = new GeneralTargetStage(distance(0), stSize, xAng(0), yAng(0), userViewTime, maxTime);
        return new SimulataneousStage(
            mainStage,
            new CodesegStage(() => {}, () => {
                mainStage.setDistance(distance(mainStage.timeElapsedTotal));
                mainStage.setXAng(xAng(mainStage.timeElapsedTotal));
                mainStage.setYAng(yAng(mainStage.timeElapsedTotal));
            }, () => { return mainStage.finished(); })
        );
    }
    public static Stage movingTargetChangingSize(float userViewTime, float maxTime, float stDistance, Func<float, float> size, Func<float, float> xAng, Func<float, float> yAng) {
        GeneralTargetStage mainStage = new GeneralTargetStage(stDistance, size(0), xAng(0), yAng(0), userViewTime, maxTime);
        return new SimulataneousStage(
            mainStage,
            new CodesegStage(() => {}, () => {
                mainStage.setSize(size(mainStage.timeElapsedTotal));
                mainStage.setXAng(xAng(mainStage.timeElapsedTotal));
                mainStage.setYAng(yAng(mainStage.timeElapsedTotal));
            }, () => { return mainStage.finished(); })
        );
    }
}