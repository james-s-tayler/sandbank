#! /bin/bash
docker-compose build && docker-compose -f docker-compose.yml -f docker-compose.localstack.yml up
