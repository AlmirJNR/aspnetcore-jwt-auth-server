FROM postgres:alpine

COPY ./Initial.sql /docker-entrypoint-initdb.d/
