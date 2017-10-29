"""
Accesses game state, and runs Zork executable to run game.
"""

import subprocess
import os
from os import path

CONV_GAME_MAP = set()

def filter_message(message):
    """
    Filters out uneccessary text from console output
    """
    return message

def contains_conv(conv):
    """
    Checks if a given conversation already
    has an associated game
    """
    return conv in CONV_GAME_MAP

def create_conv(conv):
    """
    Creates the game for a new conversation
    """
    zorkp = subprocess.Popen(['gamebin/zork', path.abspath('saves/' + conv + '.dat')],
                             cwd=path.abspath('gamebin/'), stdin=subprocess.PIPE, stdout=subprocess.PIPE)
    message = zorkp.communicate(input='save')[0]
    zorkp.kill()
    CONV_GAME_MAP |= {conv}

    return filter_message(message)

def execute_command_conv(conv, command):
    """
    Execute a Zork command for a given conversation
    """
    zorkp = subprocess.Popen(['gamebin/zork', path.abspath('saves/' + conv + '.dat')],
                             cwd=path.abspath('gamebin/'), stdin=subprocess.PIPE, stdout=subprocess.PIPE)
    message = zorkp.communicate(input='restore\n' + command + '\nsave\nquit\ny\n')[0]
    
    return filter_message(message)

def finish_conv(conv):
    """
    Finish a given conversation. Will delete from file and CONV_GAME_MAP
    """
    CONV_GAME_MAP -= {conv}
    try:
        os.remove(path.abspath('saves/' + conv + '.dat'))
    except OSError:
        pass
