#!/usr/bin/env bash 

set -e

/opt/puppetlabs/puppet/bin/puppet apply  --modulepath=/etc/puppet/modules /etc/puppet/manifests/start.pp

while [ ! -f /var/log/sess/internal-nlog.txt ]
do
  sleep 2
done
ls -l /var/log/sess/internal-nlog.txt

tail -n 0 -f /var/log/sess/internal-nlog.txt &
wait