
import json
from json.decoder import JSONDecodeError
import datetime
import io
from constants import *
from repos.language_repository import LanguageRepository
from integration.abstract_queue_handler import AbstractQueueHandler

class ExecutionQueueHandler(AbstractQueueHandler):

    def __init__(self, nature): 
        super().__init__(nature)

    def must_exist(self):
        return True

    def process(self, payload, definition):
        definition[EXECUTION_LAST] = datetime.datetime.now()
        cnt = definition[EXECUTION_COUNT]
        definition[EXECUTION_COUNT] = int(cnt) + 1 if cnt is not None else 1

