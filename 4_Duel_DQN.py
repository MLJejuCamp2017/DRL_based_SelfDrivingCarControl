# This is DQN code for vehicle simulator.

# The actions of the car
# 5 actions: 0: no move, 1: accel, 2: decel, 3: left change, 4: right change

# Implementation
# -> Just implement this code and implement Vehicle simulator!

# Modules for unity
import argparse
import base64
import json

import socketio
import eventlet
import eventlet.wsgi
import time
from PIL import Image
from PIL import ImageOps
from flask import Flask, render_template
from io import BytesIO

# Modules for DQN
import tensorflow as tf
import math
import cv2
import random
import numpy as np
import copy
import matplotlib.pyplot as plt
import datetime

# Unity connection
sio = socketio.Server()
app = Flask(__name__)

# DQN Parameters
Num_action = 5
Gamma = 0.99
Learning_rate = 0.00025

First_epsilon = 1.0
Final_epsilon = 0.01
Epsilon = First_epsilon

Num_replay_memory = 50000
Num_start_training = 25000
Num_training = 500000
Num_update = 5000
Num_batch = 32
Num_skipFrame = 4
Num_stackFrame = 4
Num_colorChannel = 1
Num_MapChannel = 1

img_size = 80
map_size = 81

Num_step_save = 50000
Num_step_plot = 10000

# Parameters for Network
first_conv_img = [8,8, Num_colorChannel * Num_stackFrame * 2,32]
first_conv_map = [8, 8, Num_stackFrame, 32]
second_conv  = [4,4,32,64]
third_conv   = [3,3,64,64]
first_dense_img = [10*10*64, 1024]
first_dense_map = [11*11*64, 1024]
first_dense = [10*10*64 + 11*11*64, 512]
second_dense_state  = [512, 1]
second_dense_action = [512, Num_action]

# Initialize weights and bias
def weight_variable(shape):
    return tf.Variable(xavier_initializer(shape))

def bias_variable(shape):
	return tf.Variable(xavier_initializer(shape))

# Xavier Weights initializer
def xavier_initializer(shape):
	dim_sum = np.sum(shape)
	if len(shape) == 1:
		dim_sum += 1
	bound = np.sqrt(2.0 / dim_sum)
	return tf.random_uniform(shape, minval=-bound, maxval=bound)

# Convolution and pooling
def conv2d(x,w, stride):
	return tf.nn.conv2d(x,w,strides=[1, stride, stride, 1], padding='SAME')

def max_pool_2x2(x):
	return tf.nn.max_pool(x, ksize=[1,2,2,1], strides=[1,2,2,1], padding='SAME')


# Assign network variables to target networks
def assign_network_to_target():
	update_wconv1_img = tf.assign(w_conv1_target_img, w_conv1_img)
	update_wconv2_img = tf.assign(w_conv2_target_img, w_conv2_img)
	update_wconv3_img = tf.assign(w_conv3_target_img, w_conv3_img)
	update_bconv1_img = tf.assign(b_conv1_target_img, b_conv1_img)
	update_bconv2_img = tf.assign(b_conv2_target_img, b_conv2_img)
	update_bconv3_img = tf.assign(b_conv3_target_img, b_conv3_img)

	update_wconv1_map = tf.assign(w_conv1_target_map, w_conv1_map)
	update_wconv2_map = tf.assign(w_conv2_target_map, w_conv2_map)
	update_wconv3_map = tf.assign(w_conv3_target_map, w_conv3_map)
	update_bconv1_map = tf.assign(b_conv1_target_map, b_conv1_map)
	update_bconv2_map = tf.assign(b_conv2_target_map, b_conv2_map)
	update_bconv3_map = tf.assign(b_conv3_target_map, b_conv3_map)

	update_wfc1_1 = tf.assign(w_fc1_1_target, w_fc1_1)
	update_wfc1_2 = tf.assign(w_fc1_2_target, w_fc1_2)
	update_wfc2_1 = tf.assign(w_fc2_1_target, w_fc2_1)
	update_wfc2_2 = tf.assign(w_fc2_2_target, w_fc2_2)
	update_bfc1_1 = tf.assign(b_fc1_1_target, b_fc1_1)
	update_bfc1_2 = tf.assign(b_fc1_2_target, b_fc1_2)
	update_bfc2_1 = tf.assign(b_fc2_1_target, b_fc2_1)
	update_bfc2_2 = tf.assign(b_fc2_2_target, b_fc2_2)

	sess.run(update_wconv1_img)
	sess.run(update_wconv2_img)
	sess.run(update_wconv3_img)
	sess.run(update_bconv1_img)
	sess.run(update_bconv2_img)
	sess.run(update_bconv3_img)

	sess.run(update_wconv1_map)
	sess.run(update_wconv2_map)
	sess.run(update_wconv3_map)
	sess.run(update_bconv1_map)
	sess.run(update_bconv2_map)
	sess.run(update_bconv3_map)

	sess.run(update_wfc1_1)
	sess.run(update_wfc1_2)
	sess.run(update_wfc2_1)
	sess.run(update_wfc2_2)
	sess.run(update_bfc1_1)
	sess.run(update_bfc1_2)
	sess.run(update_bfc2_1)
	sess.run(update_bfc2_2)

# Input
x_img = tf.placeholder(tf.float32, shape = [None, img_size, img_size, 2 * Num_colorChannel * Num_stackFrame])
x_map = tf.placeholder(tf.float32, shape = [None, map_size, map_size, Num_stackFrame])

# Normalize input
x_img = (x_img - (255.0/2)) / (255.0/2)
###################################### Image Network ######################################
# Convolution variables
w_conv1_img = weight_variable(first_conv_img)
b_conv1_img = bias_variable([first_conv_img[3]])

w_conv2_img = weight_variable(second_conv)
b_conv2_img = bias_variable([second_conv[3]])

w_conv3_img = weight_variable(third_conv)
b_conv3_img = bias_variable([third_conv[3]])

# Densely connect layer variables
w_fc1_1 = weight_variable(first_dense)
b_fc1_1 = bias_variable([first_dense[1]])

w_fc1_2 = weight_variable(first_dense)
b_fc1_2 = bias_variable([first_dense[1]])

w_fc2_1 = weight_variable(second_dense_state)
b_fc2_1 = bias_variable([second_dense_state[1]])

w_fc2_2 = weight_variable(second_dense_action)
b_fc2_2 = bias_variable([second_dense_action[1]])

# Network
h_conv1_img = tf.nn.relu(conv2d(x_img, w_conv1_img, 4) + b_conv1_img)
h_conv2_img = tf.nn.relu(conv2d(h_conv1_img, w_conv2_img, 2) + b_conv2_img)
h_conv3_img = tf.nn.relu(conv2d(h_conv2_img, w_conv3_img, 1) + b_conv3_img)

h_pool3_flat_img = tf.reshape(h_conv3_img, [-1, first_dense_img[0]])

###################################### Map Network ######################################
# Convolution variables
w_conv1_map = weight_variable(first_conv_map)
b_conv1_map = bias_variable([first_conv_map[3]])

w_conv2_map = weight_variable(second_conv)
b_conv2_map = bias_variable([second_conv[3]])

w_conv3_map = weight_variable(third_conv)
b_conv3_map = bias_variable([third_conv[3]])

# Network
h_conv1_map = tf.nn.relu(conv2d(x_map, w_conv1_map, 4) + b_conv1_map)
h_conv2_map = tf.nn.relu(conv2d(h_conv1_map, w_conv2_map, 2) + b_conv2_map)
h_conv3_map = tf.nn.relu(conv2d(h_conv2_map, w_conv3_map, 1) + b_conv3_map)

h_pool3_flat_map = tf.reshape(h_conv3_map, [-1, first_dense_map[0]])

h_flat = tf.concat([h_pool3_flat_img, h_pool3_flat_map], 1)

h_fc1_state  = tf.nn.relu(tf.matmul(h_flat, w_fc1_1)+b_fc1_1)
h_fc1_action = tf.nn.relu(tf.matmul(h_flat, w_fc1_2)+b_fc1_2)

h_fc2_state  = tf.matmul(h_fc1_state,  w_fc2_1)+b_fc2_1
h_fc2_action = tf.matmul(h_fc1_action, w_fc2_2)+b_fc2_2

h_fc2_advantage = tf.subtract(h_fc2_action, tf.reduce_mean(h_fc2_action))

output = tf.add(h_fc2_state, h_fc2_advantage)

###################################### Image Target Network ######################################
# Convolution variables target
w_conv1_target_img = weight_variable(first_conv_img)
b_conv1_target_img = bias_variable([first_conv_img[3]])

w_conv2_target_img = weight_variable(second_conv)
b_conv2_target_img = bias_variable([second_conv[3]])

w_conv3_target_img = weight_variable(third_conv)
b_conv3_target_img = bias_variable([third_conv[3]])

# Densely connect layer variables target
w_fc1_1_target = weight_variable(first_dense)
b_fc1_1_target = bias_variable([first_dense[1]])

w_fc1_2_target = weight_variable(first_dense)
b_fc1_2_target = bias_variable([first_dense[1]])

w_fc2_1_target = weight_variable(second_dense_state)
b_fc2_1_target = bias_variable([second_dense_state[1]])

w_fc2_2_target = weight_variable(second_dense_action)
b_fc2_2_target = bias_variable([second_dense_action[1]])

# Target Network
h_conv1_target_img = tf.nn.relu(conv2d(x_img, w_conv1_target_img, 4) + b_conv1_target_img)
h_conv2_target_img = tf.nn.relu(conv2d(h_conv1_target_img, w_conv2_target_img, 2) + b_conv2_target_img)
h_conv3_target_img = tf.nn.relu(conv2d(h_conv2_target_img, w_conv3_target_img, 1) + b_conv3_target_img)

h_pool3_flat_target_img = tf.reshape(h_conv3_target_img, [-1, first_dense_img[0]])

###################################### Map Target Network ######################################
# Convolution variables target
w_conv1_target_map = weight_variable(first_conv_map)
b_conv1_target_map = bias_variable([first_conv_map[3]])

w_conv2_target_map = weight_variable(second_conv)
b_conv2_target_map = bias_variable([second_conv[3]])

w_conv3_target_map = weight_variable(third_conv)
b_conv3_target_map = bias_variable([third_conv[3]])

# Target Network
h_conv1_target_map = tf.nn.relu(conv2d(x_map, w_conv1_target_map, 4) + b_conv1_target_map)
h_conv2_target_map = tf.nn.relu(conv2d(h_conv1_target_map, w_conv2_target_map, 2) + b_conv2_target_map)
h_conv3_target_map = tf.nn.relu(conv2d(h_conv2_target_map, w_conv3_target_map, 1) + b_conv3_target_map)

h_pool3_flat_target_map = tf.reshape(h_conv3_target_map, [-1, first_dense_map[0]])

h_flat_target = tf.concat([h_pool3_flat_img, h_pool3_flat_map], 1)

h_fc1_state_target  = tf.nn.relu(tf.matmul(h_flat_target, w_fc1_1_target)+b_fc1_1_target)
h_fc1_action_target = tf.nn.relu(tf.matmul(h_flat_target, w_fc1_2_target)+b_fc1_2_target)

h_fc2_state_target  = tf.matmul(h_fc1_state_target,  w_fc2_1_target)+b_fc2_1_target
h_fc2_action_target = tf.matmul(h_fc1_action_target, w_fc2_2_target)+b_fc2_2_target

h_fc2_advantage_target = tf.subtract(h_fc2_action_target, tf.reduce_mean(h_fc2_action_target))

output_target = tf.add(h_fc2_state_target, h_fc2_advantage_target)

###################################### Calculate Loss & Train ######################################
# Loss function and Train
action_target = tf.placeholder(tf.float32, shape = [None, Num_action])
y_prediction = tf.placeholder(tf.float32, shape = [None])

y_target = tf.reduce_sum(tf.multiply(output, action_target), reduction_indices = 1)
Loss = tf.reduce_mean(tf.square(y_prediction - y_target))
train_step = tf.train.AdamOptimizer(learning_rate = Learning_rate, epsilon = 1e-02).minimize(Loss)

# Initialize variables
config = tf.ConfigProto()
config.gpu_options.per_process_gpu_memory_fraction = 0.4

sess = tf.InteractiveSession(config=config)
init = tf.global_variables_initializer()
sess.run(init)

# Load the file if the saved file exists
saver = tf.train.Saver()
# check_save = 1
check_save = input('Is there any saved data?(1=y/2=n): ')

if check_save == 1:
    checkpoint = tf.train.get_checkpoint_state("./saved_networks")
    if checkpoint and checkpoint.model_checkpoint_path:
        saver.restore(sess, checkpoint.model_checkpoint_path)
        print("Successfully loaded:", checkpoint.model_checkpoint_path)
    else:
        print("Could not find old network weights")

# Initial parameters
Replay_memory = []
step = 1
Init = 0
state = 'Observing'
episode = 0

# date - hour - minute of training time
date_time = str(datetime.date.today()) + '_' + str(datetime.datetime.now().hour) + '_' + str(datetime.datetime.now().minute)


observation_in_img = 0
observation_in_map = 0
img_front_old = 0

Is_connect = False
terminal_connect = 0

reward_x = []
reward_y = []

observation_set_img = []
observation_set_map = []

action_old = np.array([1, 0, 0, 0, 0])
speed_old = 20
Was_left_changing = False
Was_right_changing = False

Vehicle_z_old = 0
# Communication with Unity
@sio.on('telemetry')
def telemetry(sid, data):
    global step, Replay_memory, observation_in_img, observation_in_map, Epsilon, terminal_connect, img_front_old, reward_x, reward_y, \
            observation_set_img, observation_set_map, TD_list, action_old, speed_old, Init, Was_left_changing, Was_right_changing, Vehicle_z_old, episode

    current_time = time.time()

    Is_right_lane_changing = float(data["Right_Changing"])
    Is_left_lane_changing = float(data["Left_Changing"])

    Is_lane_changing = False

    if Is_right_lane_changing == 1 or Is_left_lane_changing == 1:
        Is_lane_changing = True
    else:
        Is_lane_changing = False

    # # Plotting Sensor data
    # plt.figure(1)
    # plt.plot(Lidar_x, Lidar_y, 'r.',  markersize = 5)
    # plt.plot(0, 0, 'b*', markersize = 10)

    # for i in range(len(Lane_relX)):
    # 	lane_plot_x = np.array([Lane_relX[i], Lane_relX[i]])
    # 	lane_plot_y = np.array([40, -40])
    # 	plt.plot(lane_plot_x, lane_plot_y, 'g', linewidth = 1.0)

    # plt.ylim([-40, 40])
    # plt.xlim([-40, 40])
    # plt.xlabel('Y (m)')
    # plt.ylabel('X (m)')
    # plt.title('LIDAR data plotting')
    # plt.grid(True)
    # plt.draw()

    # ## Saving plot image
    # #plt.savefig("./savePlot/" + str(step) + ".png")

    # plt.pause(0.000001)
    # plt.cla()

    # terminal state
    # 0: false, 1: true

    # Make Gridmap
    Grid_map = np.zeros([81, 81])

    # Host vehicle position
    Grid_map[40, 40] = 1

    # Lane on gridmap
    Vehicle_x = float(data["Vehicle_X"])
    Vehicle_z = float(data["Vehicle_Z"])

    Lane_x = np.array([-21.5, -14.5, -7.5, -0.5, 6.5, 13.5])
    Lane_relX = Lane_x - Vehicle_x

    Lane_relX_int = np.round(Lane_relX).astype(int)

    for i in range(len(Lane_relX_int)):
        Grid_map[:, 40 + Lane_relX_int[i]] = -1

    # Process LIDAR data
    Lidar_data = []

    for i in range(360):
        Lidar_data.append(float(data[str(i)]))

    Lidar_x = np.array([])
    Lidar_y = np.array([])

    for i in range(len(Lidar_data)):
        if Lidar_data[i] != 0:
            Lidar_x = np.append(Lidar_x, (Lidar_data[i] * math.sin(i * 2 * math.pi / 360)))
            Lidar_y = np.append(Lidar_y, (Lidar_data[i] * math.cos(i * 2 * math.pi / 360)))

    for i in range(len(Lidar_x)):
        x_coord_grid = np.round(Lidar_x[i])
        y_coord_grid = np.round(Lidar_y[i])
        Grid_map[int(y_coord_grid + 40), int(x_coord_grid + 40)] = 1

    Grid_map = np.reshape(Grid_map, (81, 81, 1))

    # The current image from the camera of the car (front)
    imgString_front = data["front_image"]
    image_front = Image.open(BytesIO(base64.b64decode(imgString_front)))
    image_array_front = np.asarray(image_front)
    # ---------------------- Image transformation ----------------------
    #image_array_front = image_array_front[55:130, 60:260,:]
    image_trans_front = cv2.resize(image_array_front, (img_size, img_size))

    if Num_colorChannel == 1:
        image_trans_front = cv2.cvtColor(image_trans_front, cv2.COLOR_RGB2GRAY)
        image_trans_front = np.reshape(image_trans_front, (img_size, img_size, 1))

    #image_trans_front = (image_trans_front - (255./2.)) / (255./2.)

    # ------------------------------------------------------------------
    # The current image from the camera of the car (rear)
    imgString_rear = data["rear_image"]
    image_rear = Image.open(BytesIO(base64.b64decode(imgString_rear)))
    image_array_rear = np.asarray(image_rear)
    # ---------------------- Image transformation ----------------------
    # image_array_rear = image_array_rear[55:130, 60:260,:]

    image_trans_rear = cv2.resize(image_array_rear, (img_size, img_size))

    if Num_colorChannel == 1:
        image_trans_rear = cv2.cvtColor(image_trans_rear, cv2.COLOR_RGB2GRAY)
        image_trans_rear = np.reshape(image_trans_rear, (img_size, img_size, 1))

    # image_trans_rear = (image_trans_rear - (255./2.)) / (255./2.)
    # ------------------------------------------------------------------

    # Initialization
    if Init == 0:
        observation_next_img = np.zeros([img_size, img_size, 2])
        observation_next_map = np.zeros([map_size, map_size, 1])

        observation_in_img = np.zeros([img_size, img_size, 1])

        for i in range(Num_stackFrame):
            observation_in_img = np.insert(observation_in_img, [1], image_trans_front, axis = 2)
            observation_in_img = np.insert(observation_in_img, [1], image_trans_rear , axis = 2)

        observation_in_img = np.delete(observation_in_img, [0], axis = 2)

        # Making observation set for img
        for i in range(Num_skipFrame * Num_stackFrame):
            observation_set_img.insert(0, observation_in_img[:,:,:2])

        observation_in_map = np.zeros([map_size, map_size, 1])

        for i in range(Num_stackFrame):
            observation_in_map = np.insert(observation_in_map, [1], Grid_map, axis = 2)

        observation_in_map = np.delete(observation_in_map, [0], axis = 2)

        # Making observation set for map
        for i in range(Num_skipFrame * Num_stackFrame):
            observation_set_map.insert(0, Grid_map)

        Vehicle_z_old = Vehicle_z

        Init = 1
        print('Initialization is Finished!')

    # Processing input data
    observation_next_img = np.zeros([img_size, img_size, 1])
    observation_next_img = np.insert(observation_next_img, [1], image_trans_front, axis = 2)
    observation_next_img = np.insert(observation_next_img, [1], image_trans_rear , axis = 2)
    observation_next_img = np.delete(observation_next_img, [0], axis = 2)

    observation_next_map = Grid_map

    del observation_set_img[0]
    del observation_set_map[0]
    observation_set_img.append(observation_next_img)
    observation_set_map.append(observation_next_map)

    observation_next_in_img = np.zeros([img_size, img_size, 1])
    observation_next_in_map = np.zeros([map_size, map_size, 1])

    # Stack the frame according to the number of skipping frame
    for stack_frame in range(Num_stackFrame):
        observation_next_in_img = np.insert(observation_next_in_img, [1], observation_set_img[-1 - (Num_skipFrame * stack_frame)], axis = 2)
        observation_next_in_map = np.insert(observation_next_in_map, [1], observation_set_map[-1 - (Num_skipFrame * stack_frame)], axis = 2)

    observation_next_in_img = np.delete(observation_next_in_img, [0], axis = 2)
    observation_next_in_map = np.delete(observation_next_in_map, [0], axis = 2)

    # Get data from Unity
    # reward = float(data["reward"])
    action_vehicle = float(data["Action_vehicle"])
    speed_vehicle = float(data["Speed"])

    # According to the last action, get reward.
    action_old_index = np.argmax(action_old)

    reward = speed_vehicle / 10
    reward_bad = -10

    if action_old_index == 1:
        reward += 1
    elif action_old_index == 2:
        reward -= 1
    elif action_old_index == 3:
        reward -= 1
    elif action_old_index == 4:
        reward -= 0.5

    # Get action with string
    action_str = ''

    if action_old_index == 0:
        action_str = 'Nothing'
    elif action_old_index == 1:
        action_str = 'Acc'
    elif action_old_index == 2:
        action_str = 'Dec'
    elif action_old_index == 3:
        action_str = 'Left'
    elif action_old_index == 4:
        action_str = 'Right'

    # If terminal is 1 ( = Collision), then reward is -100
    # terminal = terminal_connect
    terminal = 0
    if abs(Vehicle_z - Vehicle_z_old) > 1 and Vehicle_z_old < 21:
        print('Terminal!!')
        terminal = 1

    if terminal == 1 and step != 1:
        reward = reward_bad

        if len(Replay_memory) > 15:
            # Replay_memory[-1][3] = reward_bad

            RM_index = list(range(-15, 0))
            RM_index.reverse()
            RM_index_crash = -1

            right_action = np.zeros([5])
            right_action[4] = 1

            left_action = np.zeros([5])
            left_action[3] = 1

            if Was_right_changing == 1:
                for i_RM in RM_index:
                    if np.argmax(Replay_memory[i_RM][2]) == 4:
                        RM_index_crash = i_RM
                        break

                Replay_memory[RM_index_crash][3] = reward_bad

            if Was_left_changing == 1:
                for i_RM in RM_index:
                    if np.argmax(Replay_memory[i_RM][2]) == 4:
                        RM_index_crash = i_RM
                        break

                Replay_memory[RM_index_crash][3] = reward_bad

    # It shows action which is decided by random or Q network while training
    Action_from = ''

    # If step is less than Num_start_training, store replay memory
    if step <= Num_start_training:
        state = 'Observing'

        action = np.zeros([Num_action])
        action[random.randint(0, Num_action - 1)] = 1.0

    elif step <= Num_start_training + Num_training:
        state = 'Training'

        # Get action
        if random.random() < Epsilon:
            action = np.zeros([Num_action])
            action[random.randint(0, Num_action - 1)] = 1.0
            Action_from = 'Random'
        else:
            Q_value = output.eval(feed_dict={x_img: [observation_in_img], x_map: [observation_in_map]})
            action = np.zeros([Num_action])
            action[np.argmax(Q_value)] = 1
            Action_from = 'Q_network'

        # Select minibatch
        minibatch =  random.sample(Replay_memory, Num_batch)

        # Save the each batch data
        observation_batch_img      = [batch[0] for batch in minibatch]
        observation_batch_map      = [batch[1] for batch in minibatch]
        action_batch               = [batch[2] for batch in minibatch]
        reward_batch               = [batch[3] for batch in minibatch]
        observation_next_batch_img = [batch[4] for batch in minibatch]
        observation_next_batch_map = [batch[5] for batch in minibatch]
        terminal_batch 	           = [batch[6] for batch in minibatch]

        # Update target network according to the Num_update value
        if step % Num_update == 0:
            assign_network_to_target()

        ####################################### Double Q Learning part #######################################
        # Get Target value
        y_batch = []
        Q_batch = output_target.eval(feed_dict = {x_img: observation_next_batch_img, x_map: observation_next_batch_map})

        for i in range(len(minibatch)):
            if terminal_batch[i] == True:
                y_batch.append(reward_batch[i])
            else:
                y_batch.append(reward_batch[i] + Gamma * np.max(Q_batch[i]))

        ######################################################################################################

        train_step.run(feed_dict = {action_target: action_batch, y_prediction: y_batch, x_img: observation_batch_img, x_map: observation_batch_map})

        # save progress every certain steps
        if step % Num_step_save == 0:
            saver.save(sess, './saved_networks/Qarsim_Duel_DQN')
            print('Model is saved!!!')

    else:
        # Testing code
        state = 'Testing'
        Q_value = output.eval(feed_dict={x_img: [observation_in_img], x_map: [observation_in_map]})
        action = np.zeros([Num_action])
        action[np.argmax(Q_value)] = 1

        Epsilon = 0

    ## Saving the camera image
    # i_front = Image.fromarray(image_array_front, mode='RGB')
    # i_front.save("./Image_front/" + str(step) + '.jpg')

    # i_rear = Image.fromarray(image_array_rear, mode='RGB')
    # i_rear.save("./Image_rear/" + str(step) + '.jpg')

    # If replay memory is more than Num_replay_memory than erase one
    if state != 'Testing':
        if len(Replay_memory) > Num_replay_memory:
            del Replay_memory[0]

        observation_in_img = np.uint8(observation_in_img)
        observation_in_map = np.int8(observation_in_map)
        observation_next_in_img = np.uint8(observation_next_in_img)
        observation_next_in_map = np.int8(observation_next_in_map)

        # Save experience to the Replay memory  and TD_list
        Replay_memory.append([observation_in_img,observation_in_map, action_old, reward, \
                                observation_next_in_img, observation_next_in_map, terminal])
    # Send action to Unity
    action_in = np.argmax(action)
    send_control(action_in)

    # Print information
    print('Step: ' + str(step) + '  /  ' + 'Episode: ' + str(episode) + ' / ' + 'State: ' + state + '  /  ' + 'Action: ' + action_str + '  /  ' +
          'Reward: ' + str(reward) + ' / ' + 'Epsilon: ' + str(Epsilon) + '  /  ' + 'Action from: ' + Action_from + '\n')

    # Save step and reward for plotting
    if state != 'Observing':
        reward_x.append(step)
        reward_y.append(reward)

    # Plot the average reward and step
    if step % Num_step_plot == 0 and state != 'Observing':
        # plt.figure(2)
        plt.xlabel('Step')
        plt.ylabel('Average Reward')
        plt.title('Dueling Deep Q Network')
        plt.grid(True)

        plt.plot(np.average(reward_x), np.average(reward_y), hold = True, marker = '*', ms = 5)
        plt.draw()
        plt.pause(0.001)

        reward_x = []
        reward_y = []

    if terminal == 1:
        if state != 'Observing':
            episode += 1

    # Get current variables to old vatiables
    observation_in_img = observation_next_in_img
    observation_in_map = observation_next_in_map

    action_old = action
    speed_old = speed_vehicle
    img_front_old = image_array_front
    Was_left_changing = Is_left_lane_changing
    Was_right_changing = Is_right_lane_changing

    Vehicle_z_old = Vehicle_z
    # Update step number and decrease epsilon
    step += 1
    if Epsilon > Final_epsilon and state == 'Training':
        Epsilon -= First_epsilon / Num_training


# Connection with Unity
@sio.on('connect')
def connect(sid, environ):
	print("connect ", sid)
	send_control(-1)

# # Disconnect with Unity
# @sio.on('disconnect')
# def disconnect(sid):
# 	print('Client disconnected')

# Send control to Unity
num_connection = 0
def send_control(action):
	global num_connection

	if action == -1:
		num_connection += 1

	if num_connection > 500:
		num_connection = 0

	sio.emit("onsteer", data={
		'action': action.__str__()
		# 'num_connection': num_connection.__str__()
	}, skip_sid=True)


if __name__ == '__main__':
    # wrap Flask application with engineio's middleware
    app = socketio.Middleware(sio, app)

    # deploy as an eventlet WSGI server
    eventlet.wsgi.server(eventlet.listen(('', 4567)), app)
