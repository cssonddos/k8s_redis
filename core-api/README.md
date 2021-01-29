Dotnetcore 3.1 API for redis tests.

# How to use, run a port-forward command on service:
kubectl port-forward svc/core-api -n core-api 8080:80

# Available endpoints 
- GET http://localhost:8080/hit - Increment "hit" key
- GET http://localhost:8080/key?key=test1 - Get a string key
- POST http://localhost:8080/key?key=test1 - Insert a string key


Logs all the events thrown by redis` client