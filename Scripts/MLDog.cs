using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MLDog : Agent
{
    // ===================================================================================================
    [SerializeField] private GameObject MainDogBodyGO;
    [SerializeField] private LegControl FrontRightLegController;
    [SerializeField] private LegControl FrontLeftLegController;
    [SerializeField] private LegControl RearRightLegController;
    [SerializeField] private LegControl RearLeftLegController;
   
    // =========
    public float DogPitch, DogRoll, DogYaw;
    public float FrontRightLegAngle, FrontLeftLegAngle, RearRightLegAngle, RearLeftLegAngle;
    public float RunningAverageProgress;
    private RunningAverageCalculator TheRunningAverageCalculator;
    public float UprightAlignment;
    public float CurrentTrackDistance;

    // =========
    private Rigidbody MainDogRigidBody;
    
    private bool Passed1M,Passed2M,Passed5M,Passed10M,Passed25M,PassedNeg1M,PassedNeg2M;
    private float EpisodeDistance;
    private float LongestDistance;
    private int MAXEpisodeStepCount = 6000;
    public int EpisodeStepCount; 
    // =======================================================================================================
   
    // ==============================================================================================================
    public override void Initialize()
    {
        MainDogRigidBody = MainDogBodyGO.GetComponent<Rigidbody>();
        LongestDistance = 0.0f;

        FrontRightLegController.InitialiseLeg();
        FrontLeftLegController.InitialiseLeg();
        RearRightLegController.InitialiseLeg();
        RearLeftLegController.InitialiseLeg();

        TheRunningAverageCalculator = new RunningAverageCalculator(windowSize: 50);

        ResetEpisode();
    } // Initialize
    // ================================================================================================
    public override void OnEpisodeBegin()
    {
        //Debug.Log(" [INFO] Walker Agent Episode Begin");

        ResetEpisode();
    } // OnEpisodeBegin
    // =================================================================================================
    public void ResetEpisode()
    {
        EpisodeStepCount = 0; 

        // Clear down any Velocities and Reset the Agent Start Position
        MainDogRigidBody.angularVelocity = Vector3.zero;
        MainDogRigidBody.velocity = Vector3.zero;

       MainDogBodyGO.transform.localPosition = new Vector3(0.0f,0.65f, 0.0f);
       MainDogBodyGO.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 00.0f);

        // Reset Leg Motions
        FrontRightLegController.ResetLegMotion();
        FrontLeftLegController.ResetLegMotion();
        RearRightLegController.ResetLegMotion();
        RearLeftLegController.ResetLegMotion();

        Passed1M = false;
        Passed2M = false;
        Passed5M = false;
        Passed10M = false;
        Passed25M = false;
        PassedNeg1M = false;
        PassedNeg2M = false;

        EpisodeDistance = 0.0f;

        RunningAverageProgress = 0.0f;
        TheRunningAverageCalculator.ClearDownAverage(); 

    }  // ResetEpisode
    // ==============================================================================================================
    public override void CollectObservations(VectorSensor sensor)
    {
        // Update the Pitch, Roll, Yaw Angles
        Vector3 GlobalEulerAngles = MainDogBodyGO.transform.localRotation.eulerAngles;

        DogPitch = GlobalEulerAngles.x;
        DogRoll = GlobalEulerAngles.z;
        DogYaw = GlobalEulerAngles.y;

        if (DogPitch < -180.0f) DogPitch = DogPitch + 360.0f;
        if (DogPitch > 180.0f) DogPitch = DogPitch - 360.0f;
        if (DogRoll < -180.0f) DogRoll = DogRoll + 360.0f;
        if (DogRoll > 180.0f) DogRoll = DogRoll - 360.0f;
        if (DogYaw < -180.0f) DogYaw = DogYaw + 360.0f;
        if (DogYaw > 180.0f) DogYaw = DogYaw - 360.0f;

        // Running Average of the Dog ML Agent Z position on the Track
        float DeltaTrackDistance = MainDogBodyGO.transform.position.z - transform.parent.position.z;
        RunningAverageProgress = TheRunningAverageCalculator.AddDataPoint(DeltaTrackDistance);

        float DeltaTrackTransverse = MainDogBodyGO.transform.position.x - transform.parent.position.x;

        // Capture The Leg Angles
        FrontRightLegAngle = FrontRightLegController.transform.localEulerAngles.z - 90.0f;
        FrontLeftLegAngle = FrontLeftLegController.transform.localEulerAngles.z - 90.0f;
        RearRightLegAngle = RearRightLegController.transform.localEulerAngles.z - 90.0f;
        RearLeftLegAngle = RearLeftLegController.transform.localEulerAngles.z - 90.0f;

        // =========================================
        // Now Collect the Observations
        // Leg Rotations
        sensor.AddObservation(FrontRightLegAngle / 60.0f);
        sensor.AddObservation(FrontLeftLegAngle / 60.0f);
        sensor.AddObservation(RearRightLegAngle / 60.0f);
        sensor.AddObservation(RearLeftLegAngle / 60.0f);

        // Body Orientations
        sensor.AddObservation(DogPitch / 120.0f);
        sensor.AddObservation(DogRoll / 120.0f);
        sensor.AddObservation(DogYaw / 180.0f);

        // Relative Track X Transerve
        sensor.AddObservation(DeltaTrackTransverse /5.0f);

        // Moving Average Forward Motion
        sensor.AddObservation(RunningAverageProgress * 5.0f); 

        // So a Total of 9 float Observations

    } // CollectObservations
    // ========================================================================================
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Only One Action Branch [0] - Just Apply Single Leg Movements at a Time  13 Discrete Actions as Follows: 

        // 1: FrontRightRotateClockwise
        // 2: FrontRightRotateAntiClockwise
        // 3: FrontLeftRotateClockwise
        // 4: FrontLeftRotateAntiClockwise
        // 5: RearRightRotateClockwise
        // 6: RearRightRotateAntiClockwise
        // 7: RearLeftRotateClockwise
        // 8: RearLeftRotateAntiClockwise

        if (actionBuffers.DiscreteActions[0] == 1) FrontRightLegController.RotateClockwise();
        if (actionBuffers.DiscreteActions[0] == 2) FrontRightLegController.RotateAntiClockwise();
        
        if (actionBuffers.DiscreteActions[0] == 3) FrontLeftLegController.RotateClockwise();
        if (actionBuffers.DiscreteActions[0] == 4) FrontLeftLegController.RotateAntiClockwise();

        if (actionBuffers.DiscreteActions[0] == 5) RearRightLegController.RotateClockwise();
        if (actionBuffers.DiscreteActions[0] == 6) RearRightLegController.RotateAntiClockwise();

        if (actionBuffers.DiscreteActions[0] == 7) RearLeftLegController.RotateClockwise();
        if (actionBuffers.DiscreteActions[0] == 8) RearLeftLegController.RotateAntiClockwise();

        // Assert the Leg Rotations (in lieu of Fixed Update
        FrontRightLegController.AssertLegRotation();
        FrontLeftLegController.AssertLegRotation();
        RearRightLegController.AssertLegRotation();
        RearLeftLegController.AssertLegRotation();

        // =============================
        
        CurrentTrackDistance = MainDogBodyGO.transform.position.z - transform.parent.position.z;
        // Now Review ML Dog Progress and Rewards
        EpisodeStepCount++;
        if(EpisodeStepCount> MAXEpisodeStepCount)
        {
            // Exceeded Training Time 
            SetReward(-5.0f);

            EpisodeDistance = CurrentTrackDistance;
            if (EpisodeDistance > LongestDistance) LongestDistance = EpisodeDistance;
            Debug.Log("[INFO]: Training Time Exceeded: " + EpisodeDistance.ToString() + " Maximum So Far: " + LongestDistance.ToString());

            EndEpisode();
        } // Training Time Exceeded
        // ========================

        UprightAlignment = Vector3.Dot(MainDogBodyGO.transform.up, Vector3.up); 
        if (UprightAlignment<0.6f)
        {
            // Excessive Upward Alignment: Appears to Have Toppled Over 
            SetReward(-2.0f);

            EpisodeDistance = CurrentTrackDistance;
            if (EpisodeDistance> LongestDistance) LongestDistance = EpisodeDistance;
            Debug.Log("[INFO]: Agent Toppled: " + EpisodeDistance.ToString() + " Maximum So Far: " + LongestDistance.ToString());

            EndEpisode();
        } // Excessive Upward Alignment
        // ========================
        if (MainDogBodyGO.transform.position.y < -1.0f)
        {
            // Fallen Off Track
            SetReward(-5.0f);

            EpisodeDistance = CurrentTrackDistance;
            if (EpisodeDistance > LongestDistance) LongestDistance = EpisodeDistance;
            Debug.Log("[INFO]: Agent Fallen: " + EpisodeDistance.ToString() + " Maximum So Far: " + LongestDistance.ToString());

            EndEpisode();
        } //  Fallen Off Track
        // ========================
        if (CurrentTrackDistance > 51.0f)
        {
            // Completed the Full Track Run !
            SetReward(25.0f);

            EpisodeDistance = CurrentTrackDistance;
            if (EpisodeDistance > LongestDistance) LongestDistance = EpisodeDistance;
            Debug.Log("[INFO]: Agent Completed Track !     within" + EpisodeStepCount.ToString() + " Steps" );

            EndEpisode();
        } //  Completed the Full Track Run !
        // ========================
        // Incremental Progress Review
        AddReward(0.001f * RunningAverageProgress);
        // ========================
        // Now Some CheckPoint Progress Reviews
        if ((!Passed1M) && (CurrentTrackDistance > 1.0f))
        {
            Passed1M = true;
            AddReward(0.2f);
        }
        if ((!Passed2M) && (CurrentTrackDistance > 2.0f))
        {
            Passed2M = true;
            AddReward(0.2f);
        }
        if ((!Passed5M) && (CurrentTrackDistance > 5.0f))
        {
            Passed5M = true;
            AddReward(0.25f);
        }
        if ((!Passed10M) && (CurrentTrackDistance > 10.0f))
        {
            Passed10M = true;
            AddReward(1.0f);
        }
        if ((!Passed25M) && (CurrentTrackDistance > 25.0f))
        {
            Passed25M = true;
            AddReward(2.5f);
        }
        if ((!PassedNeg1M) && (CurrentTrackDistance < -1.0f))
        {
            PassedNeg1M = true;
            AddReward(-0.5f);
        }
        if ((!PassedNeg2M) && (CurrentTrackDistance < -2.5f))
        {
            PassedNeg2M = true;
            AddReward(-1.0f);
        }

        // Check Point Progress Rewards
        // ======================

    } // OnActionReceived
    // ==========================================================================================================================
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Manual Control Actions  - Not Sure Know How to Control Anyway !!!!
        // Only One Action Branch [0]   

        var discreteActionsOut = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.H)) discreteActionsOut[0] = 1;
        if (Input.GetKey(KeyCode.L)) discreteActionsOut[0] = 2;

        if (Input.GetKey(KeyCode.U)) discreteActionsOut[0] = 3;
        if (Input.GetKey(KeyCode.P)) discreteActionsOut[0] = 4;

        if (Input.GetKey(KeyCode.A)) discreteActionsOut[0] = 5;
        if (Input.GetKey(KeyCode.F)) discreteActionsOut[0] = 6;

        if (Input.GetKey(KeyCode.Q)) discreteActionsOut[0] = 7;
        if (Input.GetKey(KeyCode.R)) discreteActionsOut[0] = 8;

        if (Input.GetKey(KeyCode.Space))
        {
            // Reset Epside Request
            ResetEpisode();
        }    
        
    } // Heuristic  Controls
    // =========================================================================================

    // ===========================================================================================
}  // Main ML Dog Agent Class
// ==========================================================
public class RunningAverageCalculator
{
    private Queue<float> dataQueue;
    private int windowSize;
    public RunningAverageCalculator(int windowSize)
    {
        this.windowSize = windowSize;
        this.dataQueue = new Queue<float>(windowSize);
    } // RunningAverageCalculator
    // ==========================
    public float AddDataPoint(float newDataPoint)
    {
        float removedDataPoint = 0.0f;
        if (dataQueue.Count >= windowSize)
        {
            // Remove the oldest data point if the queue is full
            removedDataPoint = dataQueue.Dequeue();
        }

        // Add the new data point to the queue
        dataQueue.Enqueue(newDataPoint);

        // Calculate the running average
        float sum = 0.0f;
        foreach (float value in dataQueue)
        {
            sum = sum + (value - removedDataPoint);

        }
        float average = sum / dataQueue.Count;

        return average;
    } // AddDataPoint
    // ==========================
    public void ClearDownAverage()
    {
        this.dataQueue.Clear();

    } // ClearDownAverage
    // ==========================
} // RunningAverageCalculator
  // =======================================================================================
