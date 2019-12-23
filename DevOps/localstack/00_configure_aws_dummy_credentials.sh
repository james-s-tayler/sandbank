#! /bin/bash

echo "installing aws-cli"
aws_dir="/root/.aws"
if [[ -d "$aws_dir" ]]
then
    echo "'${aws_dir}' already exists, skipping aws configuration with dummy credentials"
else
   mkdir /root/.aws

    # https://linuxhint.com/bash-heredoc-tutorial/
    NewFile=aws-dummy-credentials-temp
    (
cat <<'AWSDUMMYCREDENTIALS'
[default]
AWS_ACCESS_KEY_ID = dummy
AWS_SECRET_ACCESS_KEY = dummy
AWSDUMMYCREDENTIALS
    ) > ${NewFile}
    mv aws-dummy-credentials-temp /root/.aws/credentials

    NewFile=aws-config-temp
    (
cat <<'AWSCONFIG'
[default]
region = us-east-1
AWSCONFIG
    ) > ${NewFile}
    mv aws-config-temp /root/.aws/config

fi
