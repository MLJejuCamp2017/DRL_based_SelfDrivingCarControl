# DRL Based Self Driving Car Control

## Version 0.6

[Version information](https://github.com/MLJejuCamp2017/DRL_based_SelfDrivingCarControl/blob/master/Version_Info.md) of this project

**Recommended resolution = 16:9**

---

## Introduction

This repository is for `Deep Reinforcement Learning Based Self Driving Car Control` project in [ML Jeju Camp 2017](http://mlcampjeju.com/)

There are 2 main goals for this project.

* Making vehicle simulator with Unity.
* Control self driving car in the simulator with some safety systems.

  As a self driving car engineer, I used lots of `vehicle sensors`(e.g. RADAR, LIDAR, ...) to perceive environments around host vehicle. Also, There are a lot of `Advanced Driver Assistant Systems (ADAS)` which are already commercialized. I wanted to combine these things with my deep reinforcement learning algorithms to control self driving car.

Simple overview of my project is as follows. 

<img src="./Images/Overview.png" alt="Snesor data plotting" style="width: 700px;"/>

I will use sensor data and camera image as inputs of DRL algorithm. DRL algorithm decides action according to the inputs. If the action may cause dangerous situation, ADAS controls the vehicle to avoid collision. 



### Environment of this project

**Software** 
* Windows7 (64bit)
* Python 3.5.2
* Anaconda 4.2.0
* Tensorflow 1.0.1

**Hardware**
* CPU: Intel(R) Core(TM) i7-4790K CPU @ 4.00GHZ

* GPU: GeForce GTX 1080

* Memory: 8GB

  ​



### Description of files
* DQN.py: This is basic DQN model for the simulation.
* Duel_DQN.py: Dueling architecture DQN model for the simulation.
* Final_Model.py: The Proposed DQN model (Double + Prioritized Experience Replay + Dueling) for the simulation


I also upload the other DQN codes which I tested with the games that I made. Check out [my DRL github repo](https://github.com/Kyushik/DRL) 

This is my [PPT file](https://www.dropbox.com/s/3t4jruqtzgvi4gv/Kyushik_Final.pptx?dl=0) of `final presentation`

Also, this are the links for my Driving Simulators.

**Recommended Resolution = 16:9!!**

[ADAS Version](https://www.dropbox.com/s/33iscinb81n2uue/ADAS_Windows.zip?dl=0) - Windows

[ADAS Version](https://www.dropbox.com/s/fmdld7sjw9epxot/ADAS_mac.zip?dl=0) - Mac

[ADAS Version](https://www.dropbox.com/s/38yb77sc56xusen/ADAS_linux.zip?dl=0) - Linux



[No ADAS Version](https://www.dropbox.com/s/kfc7a45jcte1ozp/NoADAS_Windows.zip?dl=0) - Windows

[No ADAS Version](https://www.dropbox.com/s/1f2v6pqgjhyp8b7/NoADAS_mac.zip?dl=0) - Mac

[No ADAS Version](https://www.dropbox.com/s/tfry7v227y1kw85/NoADAS_linux.zip?dl=0) - Linux



Specific explanation of my simulator and model is as follows.  

---

## Simulator 

<img src="./Images/Simulator_sample.PNG" alt="Snesor data plotting" style="width: 700px;"/>

  I made this simulator to test my DRL algorithms. Also, to test my algorithms, I need `sensor data` and `Camera images` as inputs, but there was no driving simulators which provides both sensor data and camera images. Therefore, I tried to make one by myself. 

​  The simulator is made by [Unity](https://unity3d.com/) which is widely used for making games. There were so many errors which I had to fix for making this simulator. :weary: It still has some minor issues and one major issue. The major issue is that sometimes connection between DRL code and simulator is disconnected. I am not sure about the reason, but I will keep fixing those errors.  

  Deep reinforcement learning code is made by `Python`, I connected this code and unity game by `SocketIO`.  For using SocketIO, I referred to the unity project of [Driving simulator (Udacity)](https://github.com/udacity/self-driving-car-sim).  



As, I mentioned simulator provides 3 inputs to DRL algorithm. `Forward camera`, `Backward camera`, `Sensor data`. The example of those inputs are as follows. 

|            Front Camera Image            |            Rear Camera Image             |           Sensor data Plotting           |
| :--------------------------------------: | :--------------------------------------: | :--------------------------------------: |
| <img src="./Images/Front.gif" alt="Snesor data plotting" style="width: 300px;"/> | <img src="./Images/Rear.gif" alt="Snesor data plotting" style="width: 300px;"/> | <img src="./Images/Sensor.gif" alt="Snesor data plotting" style="width: 300px;"/> |



Also, vehicles of this simulator have some safety functions. This functions are applied to the other vehicles and host vehicle of ADAS version. The sensor overview is as follows. 

<img src="./Images/Sensor_overview.png" alt="Snesor data plotting" style="width: 700px;"/>

The safety functions are as follows. 

- Forward warning
  - Control the velocity of host vehicle equal to velocity of the vehicle at the front. 
  - If distance between two vehicles is too close, rapidly drop the velocity to the lowest velocity
- Side warning: No lane change 
- Lane keeping: If vehicle is not in the center of the lane, move vehicle to the center of the lane. 




As a result, the action of the vehicle is as follows.

- Do nothing
- Acceleration
- Deceleration
- Lane change to left lane
- Lane change to right lane

---

## DRL Model

For this project, I read papers as follows.

1. [Human-level Control Through Deep Reinforcement Learning](https://storage.googleapis.com/deepmind-media/dqn/DQNNaturePaper.pdf)


2. [Deep Reinforcement Learning with Double Q-Learning](https://arxiv.org/abs/1509.06461)
3. [Prioritized Experience Replay](https://arxiv.org/abs/1511.05952)
4. [Dueling Network Architecture for Deep Reinforcement Learning](https://arxiv.org/abs/1511.06581)
5. [Deep Recurrent Q-Learning for Partially Observable MDPs](https://arxiv.org/abs/1507.06527)
6. [Deep Attention Recurrent Q-Network](https://arxiv.org/abs/1512.01693)
7. [Playing FPS Games with Deep Reinforcement Learning](https://arxiv.org/abs/1609.05521)

  I wrote the codes 1 ~ 5. I checked performance of 1 ~ 4 and still trying to check the performance of 5. Lastly, I am studying 6 and 7. As I mentioned you can find the code of those algorithms at [my DRL github](https://github.com/Kyushik/DRL). (I should write markdown of this repository...)

  Therefore, I applied algorithms 1 ~ 4 to my DRL model. The `network model` is as follows.  

<img src="./Images/Network_model.png" alt="Snesor data plotting" style="width: 700px;"/>

Also, I used `Prioritized Experience Replay` when I choose mini batch and I used `Double Q Learning` technique when I calculate target value.

---

## Result 

This is graph of step - average reward (1000 steps)

![Average_reward_graph](C:\Users\Q\Desktop\JejuGithub\Images\Average_reward_graph.png)

The average reward increases! (2.4 ~ 4.2)



#### Before Training 

<img src="./Images/BeforeTraining.gif" alt="Result(Before Learning)" style="width: 600px;"/>



#### After Training

<img src="./Images/AfterTraining.gif" alt="Result(After Learning)" style="width: 600px;"/>



After training, host vehicle drives mush faster (almost at the maximum speed!!!) with little lane change!! Yeah! :happy: