using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class ImportantStages {
    private static double _binarySearchResult, _thresholdResult;
    
    /// <summary>
    /// Plays a piece of audio until it finishes
    /// </summary>
    /// <param name="audio">The piece of audio to play</param>
    /// <returns>A stage that plays the piece of audio</returns>
    public static Stage PlayAudio(AudioSource audio) {
        return new CodeSegStage(() => {StageStatic.stopAllAudio(); audio.Play();}, () => {return !audio.isPlaying;});
    }
    
    /// <summary>
    /// Executes a GeneralTargetStage, and then plays audio based on the result
    /// </summary>
    /// <param name="targetStage">The target stage to get the result from</param>
    /// <param name="success">The audio to play if the stage is a success</param>
    /// <param name="failure">The audio to play if the stage is a fail</param>
    /// <returns>The stage with the GeneralTargetStage first and the audio after</returns>
    public static Stage PlayAudioFromResult(GeneralTargetStage targetStage, AudioSource success, AudioSource failure) {
        return new DecisionStage(
            () => targetStage.UserSucceeded, 
            PlayAudio(success), 
            PlayAudio(failure)
        );
    }
    
    /// <summary>
    /// Conducts binary search on the approximate size where the user can see the the dartboard
    /// </summary>
    /// <param name="targetStage">The base stage</param>
    /// <param name="minSize">minimum size to check</param>
    /// <param name="maxSize">maximum size to check</param>
    /// <param name="precision">precision of size (visual degrees)</param>
    /// <param name="changeDistance">Changes distance if true, size if false</param>
    /// <returns>The binary search stage</returns>
    private static Stage BinarySearch(GeneralTargetStage targetStage, double minSize, double maxSize, double precision, bool changeDistance) {
        var newStage = changeDistance ? targetStage.GetWithSizeAdjustDistance((minSize + maxSize)/2) : targetStage.GetWithSize((minSize + maxSize)/2);
        if (maxSize - minSize < precision) {
            return new CodeSegStage(() => _binarySearchResult = newStage.GetSize());
        }
        return new StageList(
            newStage,
            new DecisionStage(
                () => newStage.UserSucceeded,
                new FutureStage(() => BinarySearch(targetStage, minSize, (minSize + maxSize)/2, precision, changeDistance)),
                new FutureStage(() => BinarySearch(targetStage, (minSize + maxSize)/2, maxSize, precision, changeDistance))
            )
        );
    }
    
    /// <summary>
    /// Conducts three down one up for the approximate threshold which the user can see at __% of the time
    /// </summary>
    /// <param name="targetStage">The base stage</param>
    /// <param name="currentSize">Current dartboard size</param>
    /// <param name="percentage">Percentage threshold for user to see</param>
    /// <param name="currentSuccesses">The current number of successes in a row</param>
    /// <param name="failedPlaces">All of the distances/sizes where the user failed</param>
    /// <param name="changeDistance">True if the distance should change, false if size should change</param>
    /// <returns>The three down one up stage</returns>
    private static Stage ThreeDownOneUp(GeneralTargetStage targetStage, double currentSize, double percentage, int currentSuccesses, List<double> failedPlaces, bool changeDistance) {
        var newStage = changeDistance ? targetStage.GetWithSizeAdjustDistance(currentSize) : targetStage.GetWithSize(currentSize);
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
                () => newStage.UserSucceeded,
                new DecisionStage(
                    () => currentSuccesses == 2,
                    new FutureStage(() => 
                        ThreeDownOneUp(targetStage, currentSize - positiveStep, percentage, 0, failedPlaces, changeDistance)
                    ),
                    new FutureStage(() => 
                        ThreeDownOneUp(targetStage, currentSize, percentage, currentSuccesses + 1, failedPlaces, changeDistance)
                    )
                ),
                new FutureStage(() =>
                {
                    failedPlaces.Add(currentSize);
                    return ThreeDownOneUp(targetStage, currentSize + negativeStep, percentage, 0,
                        failedPlaces, changeDistance);
                })
            )
        );
    }

    /// <summary>
    /// Finds the overall threshold for how small the user can see
    /// </summary>
    /// <param name="xAng">The angle to place the dartboard at (x)</param>
    /// <param name="yAng">The angle to place the dartboard at (y)</param>
    /// <param name="minTimeToView">The minimum time the user should view in order for it to be a success</param>
    /// <param name="timePerAttempt">The maximum time the user can take</param>
    /// <param name="percentageCertainty">The percentage threshold for how small the user can see</param>
    /// <param name="changingDistance">True if distance should change, false if size should change</param>
    /// <returns>The stage with binary search and three down one up</returns>
    public static Stage FindThreshold(double xAng, double yAng, double minTimeToView, double timePerAttempt, 
        double percentageCertainty, bool changingDistance) {
        return new StageList(
            BinarySearch(
                new GeneralTargetStage(100, 10, xAng, yAng, minTimeToView, timePerAttempt),
                1, 50, 1, changingDistance
            ),
            new FutureStage(() => ThreeDownOneUp(
                new GeneralTargetStage(100, 10, xAng, yAng, minTimeToView, timePerAttempt),
                Math.Min(3 + _binarySearchResult, 50), percentageCertainty/100, 0, new List<double>(),
                changingDistance
            ))
        );
    }
    
    /// <summary>
    /// Moves the target around while changing distance from the user
    /// </summary>
    /// <param name="userViewTime">The minimum time the user should view in order for it to be a success</param>
    /// <param name="maxTime">The maximum time the user can take</param>
    /// <param name="stSize">The size to start the dartboard at</param>
    /// <param name="distance">The function for distance</param>
    /// <param name="xAng">The function for x angle</param>
    /// <param name="yAng">The function for y angle</param>
    /// <returns>The stage with the target moving around</returns>
    public static Stage MovingTargetChangingDis(double userViewTime, double maxTime, double stSize, Func<double, double> distance, Func<double, double> xAng, Func<double, double> yAng) {
        var mainStage = new GeneralTargetStage(distance(0), stSize, xAng(0), yAng(0), userViewTime, maxTime);
        return new StageList(
            new SimultaneousStage(
                mainStage,
                new CodeSegStage(() => {}, () => {
                    mainStage.SetDistance(distance(mainStage.TimeElapsedTotal));
                    mainStage.SetXAng(xAng(mainStage.TimeElapsedTotal));
                    mainStage.SetYAng(yAng(mainStage.TimeElapsedTotal));
                }, () => mainStage.Finished())
            ),
            new DecisionStage(
                () => mainStage.UserSucceeded,
                new FutureStage(() => new StageList(
                    PlayAudio(StageStatic.Audios["movingCompletionAudio"]),
                    PlayAudio(StageStatic.Audios["" + Math.Truncate(mainStage.TimeElapsedTotal)]),
                    PlayAudio(StageStatic.Audios["secondsAudio"])
                )),
                new FutureStage(() => PlayAudio(StageStatic.Audios["failedAudio"]))
            )
        );
    }
    
    /// <summary>
    /// Moves the target around while changing size
    /// </summary>
    /// <param name="userViewTime">The minimum time the user should view in order for it to be a success</param>
    /// <param name="maxTime">The maximum time the user can take</param>
    /// <param name="stDistance">The distance to start the dartboard at</param>
    /// <param name="size">The function for size</param>
    /// <param name="xAng">The function for x angle</param>
    /// <param name="yAng">The function for y angle</param>
    /// <returns>The stage with the target moving around</returns>
    public static Stage MovingTargetChangingSize(double userViewTime, double maxTime, double stDistance, Func<double, double> size, Func<double, double> xAng, Func<double, double> yAng) {
        var mainStage = new GeneralTargetStage(stDistance, size(0), xAng(0), yAng(0), userViewTime, maxTime);
        return new StageList(
            new SimultaneousStage(
                mainStage,
                new CodeSegStage(() => {}, () => {
                    mainStage.SetSize(size(mainStage.TimeElapsedTotal));
                    mainStage.SetXAng(xAng(mainStage.TimeElapsedTotal));
                    mainStage.SetYAng(yAng(mainStage.TimeElapsedTotal));
                }, () => mainStage.Finished())
            ),
            new DecisionStage(
                () => mainStage.UserSucceeded,
                new FutureStage(() => new StageList(
                    PlayAudio(StageStatic.Audios["movingCompletionAudio"]),
                    PlayAudio(StageStatic.Audios["" + Math.Truncate(mainStage.TimeElapsedTotal)]),
                    PlayAudio(StageStatic.Audios["secondsAudio"])
                )),
                new FutureStage(() => PlayAudio(StageStatic.Audios["failedAudio"]))
            )
        );
    }
}