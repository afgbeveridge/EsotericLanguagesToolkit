
import os
import datetime
import json
import glob
import pandas as pd
import pymongo
from pymongo import MongoClient
from pandas import json_normalize

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
        self._raw_collection().insert_one(doc)

    def statistics_summary(self, language = None):
        return self._raw_statistics_head() if language is None else self._raw_statistics_for_language(language)

    def _raw_statistics_head(self):
        src, df = self._get_raw_statistics()
        print('Dump json')
        print(df.head())
        return df.head().to_json()

    def _raw_statistics_for_language(self, language):
        src, df = self._get_raw_statistics(language)
        scs = self._get_single_column_stats(['source', 'Code', 'Length'], df)
        exeTime = self._get_single_column_stats(['execution', 'Time', 'Milliseconds' ], df)
        basis = { "name": language, "count": len(df.index) }
        return src, {**basis, **scs, **exeTime}

    def _get_raw_statistics(self, language = None):
        coll = self._raw_collection()
        print('Query')
        results = coll.find({}, {'_id': False}) if language is None else coll.find({"name": language}, {'_id': False})
        src = list(results)
        print('Raw source')
        print(src)
        df = json_normalize(src)
        print('Normalized')
        print(df)
        return src, df

    def _get_single_column_stats(self, tags, df):
        d = {}
        queryable_title_tag = ''.join([tag for tag in tags])
        readable_title_tag = ''.join([tag.title() for tag in tags])
        d['mean{}'.format(readable_title_tag)] = df[queryable_title_tag].mean().item()
        d['min{}'.format(readable_title_tag)] = df[queryable_title_tag].min().item()
        d['max{}'.format(readable_title_tag)] = df[queryable_title_tag].max().item()
        print(d)
        return d

    def _raw_collection(self):
        return self._client[RawStatisticsRepository.DB][RawStatisticsRepository.RAW_COLLECTION]
