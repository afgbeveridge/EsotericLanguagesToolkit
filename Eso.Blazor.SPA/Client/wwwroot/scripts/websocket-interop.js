
var socket;

window.websocketInterop = {

        connect: function (helper, address) {
                console.log("Connecting to address " + address);
                socket = new WebSocket("ws://" + address);
                console.log(socket);
                socket.onmessage = function (event) {
                        console.debug("WebSocket message received:", event);
                        helper.invokeMethod("OnMessage", event.data);
                };
                socket.onclose = function (evt) {
                        console.log("Socket closed. Re-open");
                        window.websocketInterop.connect(helper, address);
                }
                console.log("Ready....");
        },

        send: function (msg) {
                console.log("Sending:" + msg);
                socket.send(msg);
        }

};