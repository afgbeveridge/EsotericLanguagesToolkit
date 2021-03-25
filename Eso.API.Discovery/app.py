"""
This script runs the application using a development server.
It contains the definition of routes and views for the application.
"""

from flask import Flask, abort, jsonify, url_for, request
from repos.language_repository import LanguageRepository
from integration.consumer_manager import ConsumerManager
from integration.update_queue_handler import UpdateQueueHandler
from integration.execution_queue_handler import ExecutionQueueHandler
import yaml
import signal
import sys
from signal_handler import SignalHandler
from flask_restful import Api, MethodView
from flask_cors import CORS

app = Flask(__name__)
api = Api(app)
CORS(app) 

mgr = ConsumerManager()

# Make the WSGI interface available at the top level so wfastcgi can get it.
wsgi_app = app.wsgi_app


def _get_host_name(req):
    host = req.host_url
    return host[:-1] if host.endswith('/') else host


def configure_manager(mgr: ConsumerManager):
    mgr.associate_handler('LangUpdate', UpdateQueueHandler('language updated'))
    mgr.associate_handler('LangExecution',
                          ExecutionQueueHandler('language execution'))


class LanguageListResource(MethodView):
    def get(self):
        languages = LanguageRepository().known_languages()
        for lang in languages:
            lang['detail'] = '{}{}'.format(_get_host_name(request),
                                           url_for('languageresource',
                                                   name=lang['name']))
        return jsonify(languages)


class LanguageResource(MethodView):
    def get(self, name):
        lang = LanguageRepository().language_definition(name)
        return abort(404) if lang is None else jsonify(lang)


api.add_resource(LanguageListResource, '/languages')
api.add_resource(LanguageResource, '/languages/<string:name>')

if __name__ == '__main__':

    import os
    cfg = yaml.safe_load(open("config.yaml", "r"))
    host = cfg['app']['host']
    try:
        port = int(cfg['app']['port'])
    except ValueError:
        port = 5555

    configure_manager(mgr.from_array(cfg['rabbit']))
    mgr.start()
    SignalHandler()\
                 .handle([signal.SIGINT, signal.SIGTERM])\
                 .using(lambda: mgr.stop() if mgr is not None else False)
    print("App {} on {}".format(host, port))
    app.run(host, port)
