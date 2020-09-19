#! /bin/bash
./build.sh
if [ ! -z "${1}" ]; then
  cdk deploy "${1}"
else
  cdk deploy "*"
fi
