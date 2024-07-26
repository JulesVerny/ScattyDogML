# ScattyDogML

This is a very basic experiment with an erratic physics based dog mouse model, using Unity ML to attempt to learn some leg controls to enable the dog to run along a Track. This uses some very rough Physics model, and Hinged Legs which are very difficult and erratic to control by hand.  So interesting to see if the Unity ML machine learning methods can acheive what is awkard for a human to control.

This project was executed to reaquant with the latest Unity ML-Agents, and could be considered used as a beginner project.   


![ScreenShot](DogMain.PNG)

## Video Demonstration ##
Please see this You Tube Video Demonstration [Lucy In Sky with OPEN AI Services Video demonstration here](https://www.youtube.com/watch?v=8Y7ENoYFzZM)  

## Implementation Details ##
This experiment is based Unity Game Environment, and the Unity ML framework to acheive some PPO based Reinforcement learning. 

![ScreenShot](Design.PNG)

The Dog is very simple creature consisting of a Rigid Body Body, with a Capsule Collider, and four Legs. Each of the Leg is a Rigid Body, attached to the Body via a Hinge Point. The only Actions are to Rotate each leg 10 degrees Clockwise, or 10 degrees Anticlockewise.  These are achieved through explcit local Rotational chnages, rather than by Torque etc. The Leg Box colliders and the Track, have a Physical Friction material, to avoid excessive sliding.  

The Unity Package is provided to set up and execute this experiment. You will need to import ML-Agents. 
I had to Use the Development Branch of ML-Agents (circa July 2024) as the last formal Release 21, had an issue with numpy incompatibility. 

There are only two scripts to this very basic experiment:
- MLDog.CS      :  This implements the core Dog ML Agent class and provides the interface into ML-Agents methods 
- LegControl.CS         :  This Script provides the leg control to each leg, and so implements each Leg rotation Action request.  


## Unity Training ##
Training through ML- Agents code. With 10 Replicated environments of Dog and Tracks to speed up Training. 
The PPO Config file is provided. The essential Training, did not require much hyper paramter tunning, for this very rough and quick experiment. So after only three Training Runs, with some acceptable training, ended up with the following hyper parameters:

![ScreenShot](Run3Reward.PNG)

## Observations and Discussion ##

Very Little Hyper parameter tuning (was attempted) to get some basic Training, and to achieve the main objectives of Dogs getting failrly consistently to traverse along the Track.

The final behaviours are still pretty erratic leg motion, with no obvious or consistent leg motion to acheive forward motion. 

The reward performance grew fairly consistently in the early to mid training session (Up to 4.5 Million epochs)  However the Reward function deteriotated with no more consistent growth or steady reward perfomance in the later training from 5 million to 10 million epochs.  


## Acknowledgements ##

- 


