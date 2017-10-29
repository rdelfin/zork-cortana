"""
Accesses game state, and runs Zork executable to run game.
"""

import subprocess

CONV_GAME_MAP = set()

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
    zorkp = subprocess.Popen(['gamebin/zork', conv + '.dat'], stdin=subprocess.PIPE, stdout=subprocess.PIPE)
    zorkp.communicate(input='save')[0]
    zorkp.kill()
    CONV_GAME_MAP |= {conv}

def execute_command_conv(conv):
    """
    Execute a Zork command for a given conversation
    """
    pass
