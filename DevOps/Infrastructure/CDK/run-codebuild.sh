#! /bin/bash

current_branch=$(git rev-parse --abbrev-ref HEAD)
echo "building ${current_branch}"
aws codebuild start-build --project-name sandbank-api --source-version ${current_branch}
