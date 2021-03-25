import json
from json.decoder import JSONDecodeError
import datetime
import io
from constants import *
from repos.language_repository import LanguageRepository
from integration.abstract_queue_handler import AbstractQueueHandler

class UpdateQueueHandler(AbstractQueueHandler):

    def __init__(self, nature): 
        super().__init__(nature) 

    def must_exist(self):
        return False

    def process(self, definition):
        definition[UPDATE_URL] = content[UPDATE_URL]
        definition[UPDATE_MODIFIED] = content[UPDATE_MODIFIED]
        definition[UPDATE_TURING_COMPLETE] = content[UPDATE_TURING_COMPLETE]
        print('Last update was {}'.format(content[UPDATE_LAST_UPDATE]))
        definition[UPDATE_LAST_UPDATE] = datetime.datetime.now()
