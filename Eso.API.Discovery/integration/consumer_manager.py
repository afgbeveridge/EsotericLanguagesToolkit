from integration.queue_consumer import QueueConsumer
from integration.abstract_queue_handler import AbstractQueueHandler

class ConsumerManager() :

    def __init__(self):
        #super(self).__init__(*args, **kwargs)
        self._consumers = []

    def start(self):
        print("Start {} consumers".format(len(self._consumers)))
        for thread in self._consumers:
            print('For host {}, consume from {}'.format(thread._host, thread._queue_name))
            thread.start()

    def add(self, host, queue_name):
        self._consumers.append(QueueConsumer(host, queue_name))
        return self

    def from_array(self, consumers):
        for consumer in consumers:
            self._consumers.append(QueueConsumer(consumer['host'], consumer['queue'], consumer['user'], consumer['pwd']))
        return self

    def associate_handler(self, queue_name, handler):
        c = next((x for x in self._consumers if x._queue_name == queue_name), None)
        assert c is not None, 'No consumer for queue {}'.format(queue_name)
        c.set_handler(handler)
        return self

    def stop(self):
        print("Stopping...")
        for thread in self._consumers:
            thread.stop()

