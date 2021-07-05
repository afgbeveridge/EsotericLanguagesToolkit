Write-Host "Removing all known ELT containers"
docker rm -f elt-local-rabbit
docker rm -f elt-eso-mysql
docker rm -f elt-eso-mongo
docker volume rm eltmongodata