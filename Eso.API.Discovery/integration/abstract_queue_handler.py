
from abc import ABC, abstractmethod
import json
from json.decoder import JSONDecodeError
import datetime
import io
from constants import *
from repos.language_repository import LanguageRepository

class AbstractQueueHandler:

    def __init__(self, nature): 
        self._nature = nature 

    @abstractmethod
    def must_exist(self):
        pass

    @abstractmethod
    def process(self, definition):
        pass

    def handle(self, body):
        print("Abstract queue handler ({}) received {}".format(self._nature, body))
        try:
            content = json.loads(body)
            repo = LanguageRepository()
            name = content['name']
            definition = repo.language_definition(name)
            assert definition is not None or not self.must_exist(), 'Unknown language {}'.format(name)
            if definition is None:
                print('New language {}'.format(name))
                definition = content
            else:
                self.process(definition)
            repo.write_language_definition(name, definition)
        except KeyError:
            print('Key error handling update')
        except JSONDecodeError as e:
            print('JSON format error')
            print("Failed to parse json in {}:{}".format(e.msg, e.pos))
        except TypeError as e:
            print("Unable to serialize JSON {}".format(str(e)))
        except IOError as e:
            print("I/O error({0}): {1}".format(e.errno, e.strerror))
        except Exception as e:
            print('Exception occurred handling update {}'.format(str(e)))
