#! /bin/bash
./build.sh
if [ ! -z "${1}" ]; then
  cdk synth "${1}"
else
  cdk synth "*"
fi
