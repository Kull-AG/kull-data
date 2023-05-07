#This is a docker file to generate a SQL server. It helps you to execute the integration tests and for development
#For more information about the possibility for SQL server on docker, check this site out: https://hub.docker.com/_/microsoft-mssql-server 

FROM mcr.microsoft.com/mssql/server:2019-latest as sqlserver

USER root

ENV ACCEPT_EULA=Y 
ENV SA_PASSWORD=abcDEF123#
ENV MSSQL_PID=Developer
ENV MSSQL_TCP_PORT=1433 



#copy all the sql files to the docker container
#COPY /Database/Scripts /database/ 
RUN ((/opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started") 

COPY . .

# install the dotnet environment
RUN apt-get update && \
    apt-get install -y dotnet-sdk-6.0 




#RUN dotnet test


#To create the image, execute the following command in the directory of the docker file 
#docker build . -t sqlserver_sqltoolsservice --no-cache 

#To execute the docker container, use the following command 
#docker run -d -p 1433:1433 --name sqlserver sqlserver_sqltoolsservice 

#Now you have a working SQL server for development and testing purpose

#stop docker
#docker stop sqlserver

#remove data
#docker rm -f sqlserver
