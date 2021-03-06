FROM ffquintella/docker-aspnetcore:runtime-2.2.4

MAINTAINER Felipe Quintella 

LABEL version="1.1.6"
LABEL description="This image contains the sess api."


ENV LANG=en_US.UTF-8
ENV LANGUAGE=en_US.UTF-8
ENV LC_ALL=en_US.UTF-8


#ENV FACTER_XXX


ENV FACTER_PRE_RUN_CMD ""
ENV FACTER_EXTRA_PACKS ""

# Puppet stuff all the instalation is donne by puppet
# Just after it we clean up everthing so the end image isn't too big

RUN  mkdir -p /opt/scripts
COPY cfg/puppet/manifests /etc/puppet/manifests/
COPY cfg/puppet/modules /etc/puppet/modules/
COPY start-service.sh /opt/scripts/start-service.sh
COPY entrypoint.sh /usr/local/bin/entrypoint
#RUN chmod +x /opt/scripts/start-service.sh ; ln -s /opt/scripts/start-service.sh /usr/local/bin/start-service  
RUN chmod +x /opt/scripts/start-service.sh ; chmod +x /usr/local/bin/entrypoint ; ln -s /opt/scripts/start-service.sh /usr/local/bin/start-service ;\
 /opt/puppetlabs/puppet/bin/puppet apply  --modulepath=/etc/puppet/modules /etc/puppet/manifests/base.pp  ;\
 yum clean all ; rm -rf /tmp/* ; rm -rf /var/cache/* ; rm -rf /var/tmp/* ; rm -rf /var/opt/staging

RUN mkdir /app
COPY artifacts/app /app

# Aspnet webserver
EXPOSE 5000/tcp
#EXPOSE 5001/tcp

WORKDIR /app

# Configurations folder, install dir
#VOLUME  XXX

#CMD /opt/puppetlabs/puppet/bin/puppet apply -l /tmp/puppet.log  --modulepath=/etc/puppet/modules /etc/puppet/manifests/start.pp
CMD /bin/bash /usr/local/bin/entrypoint
