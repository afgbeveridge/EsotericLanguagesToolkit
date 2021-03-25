
import os
import datetime
import json
import glob
from constants import *
from pathlib import Path

def _date_converter(obj):
    if isinstance(obj, datetime.datetime):
        print('JSON serialize, date time formatting')
        return obj.isoformat()

class LanguageRepository():

    def known_languages(self):
        result = []
        print('Fetch languages')
        for f in glob.glob('{}/*{}'.format(LANG_DIRECTORY, FILE_EXTENSION)):
            print(f)
            canonicalName = f.replace(FILE_EXTENSION, "")
            print(canonicalName)
            with open(f) as def_file:
                lang = json.load(def_file)
            detail = { 'name': lang['name'], 'description': lang['url'], "isCustom": lang["isCustom"] } 
            result.append(detail)
        return result

    def language_definition(self, name):
        full_name = self.__qualified_name(name)
        print('Asked to find {}'.format(full_name))
        target = Path(full_name)
        lang = None
        if target.is_file():
            with open(full_name) as def_file:
                lang = json.load(def_file)
        print('Found? {}'.format(lang is not None))
        return lang

    def write_language_definition(self, name, dict):
        full_name = self.__qualified_name(name)
        print('Asked to update {}'.format(full_name))
        with open(full_name, 'w') as outfile:
            json.dump(dict, outfile, default=_date_converter) 

    def __qualified_name(self, simple_name):
        return "{}/{}{}".format(LANG_DIRECTORY, simple_name, FILE_EXTENSION)

