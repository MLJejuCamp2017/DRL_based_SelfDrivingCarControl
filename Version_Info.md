# Version Information

### Version 1.7

#### Updated!

- Unity Scripts are provided in **Unity_Scripts** folder. 

---

### Version 1.6

#### Updated!

- Sensor noise option is added 

#### Issues

- Add construction site

---

### Version 1.5

#### Updated!

- Foggy weather option is added 

#### Issues

- Add construction site
- Add sensor noise

---

### Version 1.4

#### Updated!

- Simulator is updated to Unity ML-agents 0.6! 

#### Issues

- Add construction site
- Add sensor noise
- Add fog 

---

### Version 1.3

#### Updated!

- Training speed is much faster!! (Time scale = 100)
- More DRL algorithms are implemented (DQN, QR-DQN)

#### Issues

- Make options (Visualize sensor data, etc...)  

------

### Version 1.2

#### Updated!

- Random action level other vehicles can be changed with slider!!

#### Issues

- Make options (Visualize sensor data, etc...)  

---

### Version 1.1

#### Updated!

- Numbers of other vehicles can be changed with slider!!

#### Issues

- Make options (Visualize sensor data, etc...)  

---

### Version 1.0

#### Updated!

- Clean the RL algorithms

#### Issues

- Game parameters can be changed in the game scene
- Make options (Visualize sensor data, etc...)  

---

### Version 0.8

#### Updated!

- Linux and Mac versions are published
- Front distance problem is fixed

#### Issues

- Performance of algorithm needs to be improved! 

---

### Version 0.7

#### Major Update!!

Simulator is changed to Unity ML agents!!

#### Updated!

- Resolution problem is fixed
- Sensor problem is fixed
- Training is much more stable. 
- Computation speed problem is fixed -> updated by a step

#### Issues

- Performance of algorithm needs to be improved!
- Front distance text should be 0, if there is no front vehicle
- Collision sometimes happens
- Reward should be adjusted 

---

### Version 0.6

#### Updated!

- Recommended resolution = 16:9
- Plot is replaced by `tensorboard`
- Saving model parameter method is changed!
- Tilted heading problem is fixed!
- Ghost vehicle at the front in fixed!

#### Issues

- Text position changes depending on resolution
- Computation speed difference between exploration, training and testing
- Performance of algorithm needs to be improved!
- Left and Right warning during lane change

---

### Version 0.5

#### Updated!

- Mac and Linux version simulator is available!! :) 

#### Issues

- Text position changes depending on resolution
- Plot is not responding
- There is no 'The end' condition
- Sometimes heading of the vehicle is tilted
- Computation speed difference between exploration, training and testing

---

### Version 0.4

#### Updated!

- Null error problem is fixed
- Forward control parameter is fixed
- More DRL algorithms!

#### Issues

- Text position changes depending on resolution
- Linux and Mac version of the simulator
- Plot is not responding
- There is no 'The end' condition
- Sometimes heading of the vehicle is tilted

---

### Version 0.3

#### Updated!

- There are three front warning sensors for vehicles (Front Left, Center, Right)
- Vehicles control their speed same as the speed of the vehicle at the front when there is front warning.
- Vehicle control parameters were changed

#### Issues

- Major Issue!
  - It has Null error. The game doesn't stop but I will try to remove it. 
- Text position changes depending on resolution
- Linux and Mac version of the simulator
- DRL algorithm is needed to be fixed!! 

---

### Version 0.2

#### Updated!

- The major problem is solved! `No disconnection` happens in 150000 steps! Yeah~ 😃
- There is `no error message` in simulator, so it doesn't need to build it as a developer version

#### Issues

- Speed of training and observing / testing is so different
- It needs more front sensors! 
- Forward control is so rapid!!
- Text position changes depending on resolution

---

### version 0.1

#### Updated

- Sometimes connection of simulator and DRL code is disconnected
- Simulator is developer version, sometimes error message can be appeared, so please close the console
- Simulator is window version. After some major problems are solved, I will make Mac and Linux version. 