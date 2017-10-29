"""
Accesses game state, and runs Zork executable to run game.
"""

import subprocess
import os
from os import path

CONV_GAME_MAP = set()

def filter_message(message, first=False):
    """
    Filters out uneccessary text from console output
    """
    print("Filtering (first=%s) \"%s\"" % (str(first), message))

    lines = message.split('\n')[(1 if first else 5):]
    if not first:
        lines[0] = lines[0][1:]

    stop = 0
    while stop < len(lines):
        if lines[stop] == '>Saved.':
            break
        stop += 1

    if stop < len(lines):
        lines = lines[:stop]

    print("Filtered: \"%s\"" % '\n'.join(lines))
    return '\n'.join(lines)

def contains_conv(conv):
    """
    Checks if a given conversation already
    has an associated game
    """
    global CONV_GAME_MAP
    return conv in CONV_GAME_MAP

def create_conv(conv):
    """
    Creates the game for a new conversation
    """
    global CONV_GAME_MAP

    zorkp = subprocess.Popen([path.abspath('gamebin/zork'), path.abspath('saves/' + conv + '.dat')],
                             cwd=path.abspath('gamebin/'),
                             stdin=subprocess.PIPE, stdout=subprocess.PIPE)
    message = zorkp.communicate(input='save')[0]
    CONV_GAME_MAP |= {conv}

    return filter_message(message, True)

def execute_command_conv(conv, command):
    """
    Execute a Zork command for a given conversation
    """
    print("Binary: %s\nSave file:%s\nCWD: %s" % (path.abspath('gamebin/zork'), path.abspath('saves/' + conv + '.dat'), path.abspath('gamebin/')))
    zorkp = subprocess.Popen([path.abspath('gamebin/zork'), path.abspath('saves/' + conv + '.dat')],
                             cwd=path.abspath('gamebin/'),
                             stdin=subprocess.PIPE, stdout=subprocess.PIPE)
    message = zorkp.communicate(input='restore\n' + command + '\nsave\nquit\ny\n')[0]

    return filter_message(message)

def finish_conv(conv):
    """
    Finish a given conversation. Will delete from file and CONV_GAME_MAP
    """
    global CONV_GAME_MAP

    CONV_GAME_MAP -= {conv}
    try:
        os.remove(path.abspath('saves/' + conv + '.dat'))
    except OSError:
        pass
