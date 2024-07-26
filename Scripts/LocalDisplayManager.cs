using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalDisplayManager : MonoBehaviour
{
    // ========================================================================================

    [SerializeField] TMP_Text CurrentDogDistanceTB;
    
    [SerializeField] TMP_Text RewardValueTB;

    [SerializeField] TMP_Text StepCountValueTB;  
    [SerializeField] TMP_Text LegAngleValuesTB;
    [SerializeField] TMP_Text PitchRollYawTB;
    [SerializeField] TMP_Text UprightAlignmentValueTB;

    [SerializeField] MLDog TheLocalMLDogAgent; 
    // ========================================================================================
    void Start()
    {


        
    }
    // ========================================================================================
    // Update is called once per frame
    void Update()
    {

        // Update the Display
        CurrentDogDistanceTB.text = TheLocalMLDogAgent.CurrentTrackDistance.ToString("F2");

        PitchRollYawTB.text = TheLocalMLDogAgent.DogPitch.ToString("F2") + " : " + TheLocalMLDogAgent.DogRoll.ToString("F2") + " : " + TheLocalMLDogAgent.DogYaw.ToString("F2");

        LegAngleValuesTB.text = TheLocalMLDogAgent.FrontRightLegAngle.ToString("F1") + " : " + TheLocalMLDogAgent.FrontLeftLegAngle.ToString("F1") + " : " + TheLocalMLDogAgent.RearRightLegAngle.ToString("F1") + " : " + TheLocalMLDogAgent.RearLeftLegAngle.ToString("F1");

        StepCountValueTB.text = TheLocalMLDogAgent.EpisodeStepCount.ToString(); 

        RewardValueTB.text = TheLocalMLDogAgent.RunningAverageProgress.ToString("F2");

        UprightAlignmentValueTB.text = TheLocalMLDogAgent.UprightAlignment.ToString("F2"); 

    }
    // ========================================================================================

}
// ========================================================================================
