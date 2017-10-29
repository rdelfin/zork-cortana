from flask import render_template, send_from_directory, request, jsonify
from app import app

import hashlib, uuid

import game

from app import __config__ as config

def compare_password(password, correct_hash):
    """
    Compares password with hash
    """
    hashed_password = hashlib.sha512(password).hexdigest()
    return hashed_password == correct_hash

@app.route('/index', methods=['POST'])
@app.route('/', methods=['POST'])
def index():
    """
    Main webhook for responses to JSON objects
    """
    json_obj = request.get_json()
    conv = json_obj["conversation_id"];
    command = json_obj["command"] if "command" in json_obj else ""
    password = request.headers.get('X-Password')

    if not compare_password(password, config.hashed_password):
        return jsonify({"error": "401 Unauthorized: Password is invalid"}), 401
    
    if game.contains_conv(conv):
        return jsonify({"response": game.execute_command_conv(conv, command)})
    else:
        return jsonify({"response": game.create_conv(conv)})

