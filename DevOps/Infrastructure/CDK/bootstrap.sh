#! /bin/bash

#stands up the infrastruture from scratch

./build.sh && cdk deploy sandbank-api-build-stack --require-approval never && ./run-codebuild.sh && cdk deploy postgres-db-stack --require-approval never && cdk deploy sandbank-api-stack --require-approval never

