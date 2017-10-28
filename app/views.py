from flask import render_template, send_from_directory, request
from app import app

from app import cortana

@app.route('/')
@app.route('/index', methods=['POST'])
def index():
    return cortana.handler(request.get_json())

@app.route('/static/<path:path>')
def send_static(path):
    return send_from_directory('static', path)
