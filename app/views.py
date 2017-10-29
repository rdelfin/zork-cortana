from flask import render_template, send_from_directory, request, jsonify
from app import app

import game

@app.route('/index', methods=['POST'])
@app.route('/', methods=['POST'])
def index():
    """
    Main webhook for responses to JSON objects
    """
    json_obj = request.get_json()
    conv = json_obj["conversation_id"];
    command = json_obj["command"] if "command" in json_obj else ""
    
    if game.contains_conv(conv):
        return jsonify({"response": game.execute_command_conv(conv, command)})
    else:
        return jsonify({"response": game.create_conv(conv)})

