# DRL Based Self Driving Car Control

## Version 1.3

[Version information](https://github.com/MLJejuCamp2017/DRL_based_SelfDrivingCarControl/blob/master/Version_Info.md) of this project

---

## Introduction

Paper of this Project is accepted to Intelligent Vehicle Symposium 2018!! :smile: 

[IV2018 PPT](https://www.dropbox.com/s/tzv2dfo1qcmp64z/Kyushik_IEEE%20IV%202018%20presentation.pdf?dl=0)

![IV2018](./Images/IV2018.PNG)



This repository is for `Deep Reinforcement Learning Based Self Driving Car Control` project in [ML Jeju Camp 2017](http://mlcampjeju.com/)

There are 2 main goals for this project.

* Making vehicle simulator with [Unity ML-Agents](https://unity3d.com/kr/machine-learning).
* Control self driving car in the simulator with some safety systems.

  As a self driving car engineer, I used lots of `vehicle sensors`(e.g. RADAR, LIDAR, ...) to perceive environments around host vehicle. Also, There are a lot of `Advanced Driver Assistant Systems (ADAS)` which are already commercialized. I wanted to combine these things with my deep reinforcement learning algorithms to control self driving car.

Simple overview of my project is as follows. 

<img src="./Images/Overview.png" alt="Snesor data plotting" style="width: 700px;"/>

I will use sensor data and camera image as inputs of DRL algorithm. DRL algorithm decides action according to the inputs. If the action may cause dangerous situation, ADAS controls the vehicle to avoid collision. 



### Environment of this project

**Software** 
* Windows10 (64bit)
* Python 3.6.5
* Anaconda 5.2.0
* Tensorflow 1.8.0

**Hardware**
* CPU: Intel(R) Core(TM) i7-4790K CPU @ 4.00GHZ

* GPU: GeForce GTX 1080 Ti

* Memory: 8GB



### How to Run this Project

1. download the github repo
2. download the simulator and put all the files into the environment folder
3. open the ipynb file in the RL_algorithm folder and run it!

<br>


### Description of files
* Dueling_Image.ipynb: Dueling network using only image of vehicle.
* Dueling_sensor.ipynb: Dueling network using only sensor data of vehicle.
* Dueling_image_sensor.ipynb: Dueling network using both image and sensor of vehicle


I also upload the other DQN codes which I tested with the games that I made. Check out [my DRL github repo](https://github.com/Kyushik/DRL) 

This is my [PPT file](https://www.dropbox.com/s/3t4jruqtzgvi4gv/Kyushik_Final.pptx?dl=0) of `final presentation(Jeju Camp)`

<br>

### Link of the Simulators

Also, this are the links for my Driving Simulators. 

[Simulator](https://www.dropbox.com/s/7xti37jv3d28u1z/environment_windows.zip?dl=0) - Windows

[Simulator](https://www.dropbox.com/s/eurh69orthwx2hv/environment_mac.zip?dl=0) - Mac

[Simulator](https://www.dropbox.com/s/lehnq7e1zm8r6rg/environment_linux.zip?dl=0) - Linux

Unzip the simulator into the `environment` folder.

<br>

Specific explanation of my simulator and model is as follows.  

---

## Simulator 

<img src="./Images/Simulator_sample.PNG" alt="Snesor data plotting" style="width: 700px;"/>

  I made this simulator to test my DRL algorithms. Also, to test my algorithms, I need `sensor data` and `Camera images` as inputs, but there was no driving simulators which provides both sensor data and camera images. Therefore, I tried to make one by myself. 

  The simulator is made by [Unity ML-agents](https://unity3d.com/kr/machine-learning) 



### Inputs

As, I mentioned simulator provides 2 inputs to DRL algorithm. `Forward camera`, `Sensor data`. The example of those inputs are as follows. 

|                      Front Camera Image                      |                     Sensor data Plotting                     |
| :----------------------------------------------------------: | :----------------------------------------------------------: |
| <img src="./Images/Front.gif" alt="Snesor data plotting" style="width: 300px;"/> | <img src="./Images/Sensor.gif" alt="Snesor data plotting" style="width: 300px;"/> |



Also, vehicles of this simulator have some safety functions. This functions are applied to the other vehicles and host vehicle of ADAS version. The sensor overview is as follows. 

<img src="./Images/Sensor_overview.png" alt="Snesor data plotting" style="width: 700px;"/>

The safety functions are as follows. 

- Forward warning
  - Control the velocity of host vehicle equal to velocity of the vehicle at the front. 
  - If distance between two vehicles is too close, rapidly drop the velocity to the lowest velocity
- Side warning: No lane change 
- Lane keeping: If vehicle is not in the center of the lane, move vehicle to the center of the lane. 

<br>

#### Vector Observation information

In this simulator, size of vector observation is **373**.

0 ~ 359: LIDAR Data (1 particle for 1 degree)

360 ~ 362: Left warning, Right Warning, Forward Warning (0: False, 1: True)

363: Normalized forward distance

364: Forward vehicle Speed

365: Host Vehicle Speed

**0 ~  365 are used as input data for sensor**

**366 ~ 372 are used for sending information**

366: Number of Overtake in a episode

367: Number of lane change in a episode

368 ~ 372: Longitudinal reward, Lateral reward, Overtake reward, Violation reward, collision reward

(Specific information of rewards are as follows)

<br>

### Actions


The action of the vehicle is as follows.

- Do nothing
- Acceleration
- Deceleration
- Lane change to left lane
- Lane change to right lane

<br>

### Rewards

In this simulator, 5 different kinds of rewards are used. 

`Longitudinal reward`: ((vehicle_speed - vehicle_speed_min) / (vehicle_speed_max - vehicle_speed_min));

- 0: Minimum speed, 1: Maximum speed

`Lateral reward`: - 0.5 

- During the lane change it continuously get lateral reward

`Overtake reward`: 0.5* (num_overtake - num_overtake_old)

- 0.5 / overtake

` Violation reward`: -0.1

- example: If vehicle do left lane change at left warning, it gets violation reward (Front and right warning also)

`Collision reward`: -10

- If collision happens, it gets collision reward



Sum of these 5 rewards is final reward of this simulator 

<br>

## Sliders

<img src="./Images/slider.PNG" alt="Slider" style="width: 400px;"/>

You can change some parameters with the Slider on the left side of simulator

- Number of Vehicles (0 ~ 32) : Change the number of other vehicles
- Random Action (0 ~ 6): Change the random action level of other vehicles (Higher value, more random action)

---

## DRL Model

For this project, I read papers as follows.

1. [Human-level Control Through Deep Reinforcement Learning](https://storage.googleapis.com/deepmind-media/dqn/DQNNaturePaper.pdf)


2. [Deep Reinforcement Learning with Double Q-Learning](https://arxiv.org/abs/1509.06461)
3. [Prioritized Experience Replay](https://arxiv.org/abs/1511.05952)
4. [Dueling Network Architecture for Deep Reinforcement Learning](https://arxiv.org/abs/1511.06581)

   ​

You can find the code of those algorithms at [my DRL github](https://github.com/Kyushik/DRL). 

I applied algorithms 1 ~ 4 to my DRL model. The `network model` is as follows.  

<img src="./Images/Network_model.png" alt="Snesor data plotting" style="width: 700px;"/>

---

## Result 

#### Graphs

|                        Average Speed                         |                   Average # of Lane Change                   |                    Average # of Overtake                     |
| :----------------------------------------------------------: | :----------------------------------------------------------: | :----------------------------------------------------------: |
| <img src="./Images/speed.png" alt="Graph(Lane Change)" style="width: 480px;"/> | <img src="./Images/lanechange.png" alt="Graph(Lane Change)" style="width: 480px;"/> | <img src="./Images/overtake.png" alt="Graph(Lane Change)" style="width: 480px;"/> |

| Input Configuration | Speed (km/h) | Number of Lane Change | Number of Overtaking |
| :-----------------: | :----------: | :-------------------: | :------------------: |
|     Camera Only     |   71.0776    |          15           |       35.2667        |
|     LIDAR Only      |   71.3758    |      **14.2667**      |       38.0667        |
|     Multi-Input     | **75.0212**  |         19.4          |       **44.8**       |

#### Before Training 

<img src="./Images/BeforeTraining.gif" alt="Result(Before Learning)" style="width: 480px;"/>



#### After Training

<img src="./Images/After_training.gif" alt="Result(After Learning)" style="width: 480px;"/>



After training, host vehicle drives mush faster (almost at the maximum speed!!!) with little lane change!! Yeah! :happy: