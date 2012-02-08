#!/bin/bash
# Build script for Operations-Research for Mono.
clear
if [ -d ./Release ]
then
	echo "Cleaning up..."
	rm -rf ./Release
fi

mkdir ./Release

echo "Compiling..."
gmcs @./Source/Console.rsp
