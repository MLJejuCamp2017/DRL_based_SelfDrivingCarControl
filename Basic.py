# Basic Communication Code
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

# Unity connection
sio = socketio.Server()
app = Flask(__name__)

@sio.on('telemetry')
def telemetry(sid, data):
    send_control(action)

# Connection with Unity
@sio.on('connect')
def connect(sid, environ):
	print("connect ", sid)
	send_control(-1)

# Send control to Unity
def send_control(action):
	sio.emit("onsteer", data={
		'action': action.__str__()
		# 'num_connection': num_connection.__str__()
	}, skip_sid=True)


if __name__ == '__main__':
    # wrap Flask application with engineio's middleware
    app = socketio.Middleware(sio, app)

    # deploy as an eventlet WSGI server
    eventlet.wsgi.server(eventlet.listen(('', 4567)), app)
