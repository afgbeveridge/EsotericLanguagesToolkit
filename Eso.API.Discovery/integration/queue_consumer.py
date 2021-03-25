import threading
import pika

class QueueConsumer(threading.Thread):
    def __init__(self, host, queue_name, user, pwd, *args, **kwargs):
        super(QueueConsumer, self).__init__(*args, **kwargs)

        self._host = host
        self._queue_name = queue_name
        self._credentials = pika.PlainCredentials(user, pwd)
        self._handler = None

    # Not necessarily a method.
    def callback_func(self, channel, method, properties, body):
        decoded_body = body.decode()
        print("QC: {} received '{}'".format(self.name, decoded_body))
        self.default_handler.handle(decoded_body) if self._handler is None else self._handler.handle(decoded_body)

    def default_handler(self, body):
        print("QC: NOP handler executing on {}".format(body))

    def set_handler(self, handler):
        self._handler = handler;
        return self

    def run(self):
        print("QC: Starting...")

        connection = pika.BlockingConnection(
            pika.ConnectionParameters(host=self._host,
                                      credentials=self._credentials))

        print("QC: Open channel...")
        self._channel = connection.channel()

        self._channel.basic_consume(self._queue_name, self.callback_func,
                              auto_ack=True)

        self._channel.start_consuming()

    def stop(self):
        print("QC: Being asked to stop ({}, {})".format(self._host, self._queue_name))
        self._channel.stop_consuming()