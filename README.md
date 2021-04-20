# Overview

A generalized interpreter toolkit for esoteric languages, written in/using C# 8, .NET Core, Blazor, Python, 
Docker, mysql, Rabbitmq, mongodb, sanic, pandas. 

You can create your own esoteric language based on changing the keyword mappings of a generalized eso language.

Some languages are supplied as standard, others can be created using the Blazor client.

Directly supported:

* FALSE language (http://strlen.com/false/false.txt)
* BrainFuck (recent, stable) (http://en.wikipedia.org/wiki/Brainfuck)
* Befunge-93 (minor defects present) (http://catseye.tc/projects/befunge93/doc/befunge93.html)
* WARP (specification at [url:http://esolangs.org/wiki/WARP])
* Deadfish (specification at [url:http://esolangs.org/wiki/Deadfish])

Site maintainers blog: http://tb-it.blogspot.com/

Please be aware that this software is released under an MIT license.

# Set up

See setup.txt for environment details.

# Microcosm
Note that this project is also a microcosm of microservices (if you will).

For example, the statistics and discovery services are both written in python. However, they approach storage and RabbitMq connectivity differently.

The stats api (which is still a WIP), uses Celery to start a worker, and MongoDB as its persistent store. The discovery API uses JSON files for storage, and uses Python threads to 
handle RabbitMq communications.

The execution service, on the other hand, is a .net core c# 8 API, using web sockets.