import json
from json.decoder import JSONDecodeError
from raw_statistics_repo import RawStatisticsRepository

def process(body):
    print('CUSTOM: Received message: {0!r}'.format(body))
    # Dissect, insert into mongo
    try:
        doc = json.loads(body)
        print('CUSTOM: language {}'.format(doc['name']))
        repo = RawStatisticsRepository()
        repo.add(doc)
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
    

