ECHO RUN THIS SCIRPT AFTER caas-dev-db container is running!
docker run --name caas-dev-myphpadmin -d --link caas-dev-db:db -p 8081:80 phpmyadmin/phpmyadmin
ECHO login: root password: mypass123