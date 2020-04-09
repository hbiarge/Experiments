# Sample messaging solution

Implements a complex system using messaging, a process manager, external comunication with Http and gRPC.

## Dependencies

The system is configured to use some resources in Azure. In order to run the sample, you need:

- An Azure Service Bus (Standard). You should provide the connection string in the Shared/Constants.cs file
- An Azure Storage with a blob containar called *databus*. You should provide the connection string in the Shared/Constants.cs file
- An ApplicationInsights. You should provide the instrumentation key in the Shared/Constants.cs file

## Starting a Saga

Do a POST call to http://localhost:5000/Estimations with this Json body

```
{
	"CallBackUri": "http://localhost:5004/Notification",
	"imageUrls": [
	  "http://localhost:5001/images/image01.png",
	  "http://localhost:5001/images/image02.jpg"
	]
}
```

The response returns a ticket. You can query the ticket status doing a GET call to http://localhost:5000/Estimations/<ticket-received>