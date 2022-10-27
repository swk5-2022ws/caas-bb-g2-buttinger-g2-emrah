FROM mysql

MAINTAINER Xaver Buttinger S2010307005@fhooe.at

# copy the schema init script so mysql will use it to initialize the schema
ADD ./caas_schema.sql /docker-entrypoint-initdb.d

RUN chown -R mysql:root /var/lib/mysql/

# set necessary environment properties for mysql
ENV MYSQL_DATABASE=caas
ENV MYSQL_USER=service
ENV MYSQL_PASSWORD=mypass123
ENV MYSQL_ROOT_PASSWORD=mypass123

ADD caas_schema.sql /etc/mysql/data.sql

# initialize mysql db
RUN sed -i 's/MYSQL_DATABASE/'$MYSQL_DATABASE'/g' /etc/mysql/data.sql
RUN cp /etc/mysql/data.sql /docker-entrypoint-initdb.d

# expose the db port
EXPOSE 3306
EXPOSE 80/tcp
EXPOSE 80/udp
EXPOSE 443/tcp
EXPOSE 8081/udp