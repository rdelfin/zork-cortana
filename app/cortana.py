from flask import jsonify

def handler(json_body):
    return jsonify({"test": "hello world!"})
