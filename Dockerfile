FROM mcr.microsoft.com/mssql/server:2019-latest as sqlserver

USER root

ENV ACCEPT_EULA=Y 
ENV SA_PASSWORD=abcDEF123#
ENV MSSQL_PID=Developer
ENV MSSQL_TCP_PORT=1433 


RUN ((/opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started") 