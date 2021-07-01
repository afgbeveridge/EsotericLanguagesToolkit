call .\statistics-env\scripts\activate
celery -A async_statistics worker --loglevel=info