
from celery import Celery
from celery import bootsteps
from kombu import Consumer, Exchange, Queue
from statistics_message_processor import process

app = Celery(broker='amqp://guest@localhost//')

#app.conf.task_default_queue = 'LangStatistics'
#app.conf.task_default_exchange_type = 'direct'
#app.conf.task_default_queue = 'default'

stats_queue = Queue('LangStatistics', Exchange('amq.fanout'), '', no_declare = True, durable=True)

app.conf.task_queues = (
    stats_queue,
)

class StatisticsConsumerStep(bootsteps.ConsumerStep):

    def get_consumers(self, channel):
        return [Consumer(channel,
                         queues=[stats_queue],
                         callbacks=[self.handle_message],
                         accept=['json'])]

    def handle_message(self, body, message):
        message.ack()
        print('Message acknowledged')
        print('{0!r}'.format(message))
        process(body)

app.steps['consumer'].add(StatisticsConsumerStep)

