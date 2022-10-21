FROM mysql

ADD ./caas_schema.sql /docker-entrypoint-initdb.d
# COPY ./caas_schema.sql /tmp/caas_schema.sql

#CMD [ "mysqld", "--init-file=/tmp/caas_schema.sql" # https://www.baeldung.com/ops/docker-mysql-container
FROM mysql

RUN chown -R mysql:root /var/lib/mysql/

# ARG MYSQL_DATABASE
# ARG MYSQL_USER
# ARG MYSQL_PASSWORD
# ARG MYSQL_ROOT_PASSWORD

ENV MYSQL_DATABASE=caas
ENV MYSQL_USER=service
# ENV MYSQL_PASSWORD=mypass123
ENV MYSQL_ROOT_PASSWORD=mypass123

ADD caas_schema.sql /etc/mysql/data.sql

RUN sed -i 's/MYSQL_DATABASE/'$MYSQL_DATABASE'/g' /etc/mysql/data.sql
RUN cp /etc/mysql/data.sql /docker-entrypoint-initdb.d

EXPOSE 3306

# ADD ./caas_schema.sql /docker-entrypoint-initdb.d
# COPY ./caas_schema.sql /tmp/caas_schema.sql

#CMD [ "mysqld", "--init-file=/tmp/caas_schema.sql" ]