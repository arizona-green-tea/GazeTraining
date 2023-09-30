using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViveSR.anipal.Eye;
using System;
using System.Linq;

public static class ImportantStages
{
    private static double binarySearchResult;
    private static double thresholdResult;
    
    /**
     * Plays a piece of audio until it finishes
     */
    public static Stage PlayAudio(AudioSource audio) {
        return new CodesegStage(() => {StageStatic.stopAllAudio(); audio.Play();}, () => {return !audio.isPlaying;});
    }
    
    /**
     * Executes a GeneralTargetStage, and then plays audio based on the result
     */
    public static Stage PlayAudioFromResult(GeneralTargetStage targetStage, AudioSource success, AudioSource failure) {
        return new StageList(
            targetStage,
            new DecisionStage(() => targetStage.userSucceeded, PlayAudio(success), PlayAudio(failure))
        );
    }
    
    /**
     * Conducts binary search on the approximate size where the user can see the the dartboard
     * Parameters: minimum size to check, maximum size to check, and precision of size (visual degrees)
     */
    public static Stage binarySearchFinalSize(GeneralTargetStage targetStage, double minSize, double maxSize, double precision) {
        var newStage = targetStage.getWithSize((minSize + maxSize)/2);
        if (maxSize - minSize < precision) {
            return new StageList(
                PlayAudio(StageStatic.Audios["smallestVisualAngleAudio"]),
                PlayAudio(StageStatic.Audios["" + Math.Truncate(maxSize)]),
                PlayAudio(StageStatic.Audios["degreesAudio"])
            );
        }
        return new StageList(
            newStage,
            new DecisionStage(
                () => newStage.userSucceeded,
                binarySearchFinalSize(targetStage, minSize, (minSize + maxSize)/2, precision),
                binarySearchFinalSize(targetStage, (minSize + maxSize)/2, maxSize, precision)
            )
        );
    }
    
    /**
     * Conducts binary search on the approximate distance where the user can see the the dartboard
     * Parameters: minimum size to check, maximum size to check, and precision of size (visual degrees)
     */
    private static Stage binarySearchFinalDistance(GeneralTargetStage targetStage, double minDis, double maxDis, double precision) {
        var newStage = targetStage.getWithSizeAdjustDistance((minDis + maxDis)/2);
        if (maxDis - minDis < precision)
        {
            return new StageList(
                new CodesegStage(() => binarySearchResult = newStage.getSize())
                // PlayAudio(StageStatic.Audios["smallestVisualAngleAudio"]),
                // PlayAudio(StageStatic.Audios["" + Math.Truncate(newStage.getSize())]),
                // PlayAudio(StageStatic.Audios["degreesAudio"])
            );
        }
        return new StageList(
            newStage,
            new DecisionStage(
                () => newStage.userSucceeded,
                new FutureStage(() => binarySearchFinalDistance(targetStage, minDis, (minDis + maxDis)/2, precision)),
                new FutureStage(() => binarySearchFinalDistance(targetStage, (minDis + maxDis)/2, maxDis, precision))
            )
        );
    }
    
    /**
     * Conducts three down one up for the approximate threshold which the user can see at __% of the time 
     */
    private static Stage threeDownOneUpFinalDistance(GeneralTargetStage targetStage, double currentDistance, 
        double percentage, int currentSuccesses, List<double> failedPlaces) {
        var newStage = targetStage.getWithSizeAdjustDistance(currentDistance);
        const double negativeStep = 1;
        var positiveStep = negativeStep * Math.Pow(percentage, 3) / (1 - Math.Pow(percentage, 3));
        
        if (failedPlaces.Count == 5) {
            var threshold = failedPlaces.Average();
            return new StageList(
                PlayAudio(StageStatic.Audios["smallestVisualAngleAudio"]),
                PlayAudio(StageStatic.Audios["" + Math.Truncate(threshold)]),
                PlayAudio(StageStatic.Audios["degreesAudio"])
            );
        }
        return new StageList(
            newStage,
            new DecisionStage(
                () => newStage.userSucceeded,
                new DecisionStage(
                    () => currentSuccesses == 2,
                    new FutureStage(() => 
                        threeDownOneUpFinalDistance(targetStage, currentDistance - positiveStep, percentage, 0, failedPlaces)
                    ),
                    new FutureStage(() => 
                        threeDownOneUpFinalDistance(targetStage, currentDistance, percentage, currentSuccesses + 1, failedPlaces)
                    )
                ),
                new FutureStage(() =>
                {
                    failedPlaces.Add(currentDistance);
                    return threeDownOneUpFinalDistance(targetStage, currentDistance + negativeStep, percentage, 0,
                        failedPlaces);
                })
            )
        );
    }

    public static Stage findThresholdForChangingDistance(double xAng, double yAng, double minTimeToView,
        double timePerAttempt, double percentageCertainty) {
        return new StageList(
            binarySearchFinalDistance(
                new GeneralTargetStage(100, 10, xAng, yAng, minTimeToView, timePerAttempt),
                1, 50, 1
            ),
            new FutureStage(() => threeDownOneUpFinalDistance(
                new GeneralTargetStage(100, 10, xAng, yAng, minTimeToView, timePerAttempt),
                binarySearchResult, percentageCertainty/100, 0, new List<double>()
            ))
        );
    }
    
    /**
     * Moves the target around while changing distance from the user 
     */
    public static Stage movingTargetChangingDis(double userViewTime, double maxTime, double stSize, Func<double, double> distance, Func<double, double> xAng, Func<double, double> yAng) {
        var mainStage = new GeneralTargetStage(distance(0), stSize, xAng(0), yAng(0), userViewTime, maxTime);
        return new SimulataneousStage(
            mainStage,
            new CodesegStage(() => {}, () => {
                mainStage.setDistance(distance(mainStage.timeElapsedTotal));
                mainStage.setXAng(xAng(mainStage.timeElapsedTotal));
                mainStage.setYAng(yAng(mainStage.timeElapsedTotal));
            }, () => mainStage.finished())
        );
    }
    
    /**
     * Moves the target around while changing size
     */
    public static Stage movingTargetChangingSize(double userViewTime, double maxTime, double stDistance, Func<double, double> size, Func<double, double> xAng, Func<double, double> yAng) {
        var mainStage = new GeneralTargetStage(stDistance, size(0), xAng(0), yAng(0), userViewTime, maxTime);
        return new SimulataneousStage(
            mainStage,
            new CodesegStage(() => {}, () => {
                mainStage.setSize(size(mainStage.timeElapsedTotal));
                mainStage.setXAng(xAng(mainStage.timeElapsedTotal));
                mainStage.setYAng(yAng(mainStage.timeElapsedTotal));
            }, () => mainStage.finished())
        );
    }
}