
from sanic import Sanic
from sanic.response import json
from sanic.response import raw
from raw_statistics_repo import RawStatisticsRepository
from functools import partial

app = Sanic("Eso statistics")

# languages known to have stats, with links
@app.route('/stats')
async def top_level_summary(request):
    repo = RawStatisticsRepository()
    return raw(repo.statistics_summary(), content_type = 'application/json')

# language stats
@app.route('/stats/<language>')
async def language_stats(request, language):
    repo = RawStatisticsRepository()
    only_raw = request.args.get("style") == 'raw'
    src, summary = repo.statistics_summary(language)
    return json(src) if only_raw else json(summary)

if __name__ == '__main__':
    app.run()