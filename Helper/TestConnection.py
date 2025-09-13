import time
from rcon import Client
import random

#Sends a single command
def sendcommand(command):
    #Change this to what you put in your startup options
    port = 27015
    password = "hello"
    with Client("127.0.0.1", port, passwd=password) as client:
        response = client.run(command)
        print(response)

sendcommand("status")