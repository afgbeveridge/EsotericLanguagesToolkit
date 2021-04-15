
from sanic import Sanic
from sanic.response import json

app = Sanic("Eso statistics")

# languages known to have stats, with links
@app.route('/stats')
async def test(request):
    return json({'stats': null})

# languages known to have stats, with links
@app.route('/stats')
async def test(request):
    return json({'stats': null})

# language stats
@app.route('/stats/<language>')
async def test(request, language):
    return json({'stats': null})

if __name__ == '__main__':
    app.run()