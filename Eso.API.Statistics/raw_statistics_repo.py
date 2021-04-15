
import os
import datetime
import json
import glob
from pathlib import Path
import pymongo
from pymongo import MongoClient

def _date_converter(obj):
    if isinstance(obj, datetime.datetime):
        print('JSON serialize, date time formatting')
        return obj.isoformat()

class RawStatisticsRepository():

    DB = 'eso-statistics'
    RAW_COLLECTION = 'Raw'

    def __init__(self):
        self._client = MongoClient()

    def known_languages(self):
        pass

    def add(self, doc):
        self._rawCollection().insert_one(doc)

    def statisticsFor(self, language):
        pass
    
    def _rawCollection(self):
        return self._client[RawStatisticsRepository.DB][RawStatisticsRepository.RAW_COLLECTION]
