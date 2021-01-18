#!/bin/bash

set -e

docker build -f Conan.API/Dockerfile . -t registry.cn-hangzhou.aliyuncs.com/crucialize/conan:latest

echo
echo
echo 'build image success!'

docker push registry.cn-hangzhou.aliyuncs.com/crucialize/conan:latest

echo
echo
echo 'push success!'