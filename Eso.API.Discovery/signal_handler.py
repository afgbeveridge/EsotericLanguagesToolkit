
import signal
import sys

class SignalHandler():

    def handle(self, signals):
        print("Capturing signals...{}".format(str(signals)[1:-1]))
        for s in signals:
            signal.signal(s, self._exit_gracefully)
        return self

    def using(self, handler):
        print("Associate signal handler...")
        self._handler = handler
        return self

    def _exit_gracefully(self, signum, frame):
        print("Handle exit")
        if self._handler is not None: self._handler() 
        sys.exit(0)
