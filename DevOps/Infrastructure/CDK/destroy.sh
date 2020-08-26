#! /bin/bash
./build.sh
if [ ! -z "${1}" ]; then
  cdk destroy "${1}"
else
  cdk destroy "*"
fi
