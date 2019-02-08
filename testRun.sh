#!/bin/bash 

argc=$#

if (( $argc < 1 )); then 
  echo 'Please use testRun.sh <<version>>'
else

 cmd="docker run -p 5000:5000 -p 5001:5001 --name sess --rm -ti ffquintella/sess:${1} /bin/bash"
 
 echo 'Running: '$cmd

 eval $cmd

fi