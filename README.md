# StripeToSqs

ASP.NET MVC4 bridge application to accept Stripe webhook events and pass them to an Amazon SQS queue.

## Configuration

Configuration is done using the `<appSettings>` section in web.config.

```xml

<appSettings>
	<!-- 
		Optional security ID.  
		By default, the webhook path would be /Stripe/Webhook.
		If specified, add this security code to the end of the path, such as /Stripe/Webhook/<securityId>
		-->
	<add key="SecurityId" value="" />
		
	<!-- 
		Optional AWS access key & secret.  If left blank, then the SDK will attempt to get the credentials from
		the current AWS IAM "role" that the running EC2 instance is running as.  Otherwise, specify them here.
		-->
	<add key="AWSAccessKey" value="" />
	<add key="AWSSecretKey" value="" />

	<!-- 
		Required URL for the SQS queue.
		-->
	<add key="SqsQueueUrl" value="https://sqs.us-east-1.amazonaws.com/123456789012/my-queue"/>
		
	<!-- 
		Optional endpoint for the SQS command.
		-->
	<add key="SqsEndpoint" value="" />
</appSettings>

```
## Building

Requires **Visual Studio 2012**.

1. Open StripeToSqs.sln
2. Configure using web.config
3. Build solution
