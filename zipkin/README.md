# Exploring OpenZipkin
Run zipkin server

`docker run -d -p 9411:9411 openzipkin/zipkin`

Run Web API

`dotnet run`

Make request

`GET https://localhost:5001/weatherforecast`

Should see something like:

![Zipkin Server](zipkin.png)
