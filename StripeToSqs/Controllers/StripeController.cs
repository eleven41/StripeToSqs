using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace StripeToSqs.Controllers
{
    public class StripeController : Controller
    {
        //
        // POST: /Stripe/Webhook

		[HttpPost]
        public ActionResult Webhook(string id)
		{
			string securityId = ConfigurationManager.AppSettings["SecurityId"];
			if (!String.IsNullOrEmpty(securityId))
			{
				if (securityId != id)
					return new HttpStatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
			}

			// Verify the content type is json
			if (!this.Request.ContentType.StartsWith("application/json"))
				return new HttpStatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);

			// TODO: Verify the user agent

			// Parse the JSON content simply to validate it as proper JSON
			this.Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
			var textReader = new System.IO.StreamReader(this.Request.InputStream);

			{
				Newtonsoft.Json.JsonReader reader = new Newtonsoft.Json.JsonTextReader(textReader);
				JObject jObject = JObject.Load(reader);
			}

			// All is OK, so seek back to the start
			this.Request.InputStream.Seek(0, System.IO.SeekOrigin.Begin);
			// and re-read the content
			string messageBody = textReader.ReadToEnd();

			string queueUrl = ConfigurationManager.AppSettings["SqsQueueUrl"];
			if (String.IsNullOrEmpty(queueUrl))
				throw new Exception("Null or empty SqsQueueUrl setting.");

			Amazon.SQS.AmazonSQS sqsClient;

			string endPointName = ConfigurationManager.AppSettings["SqsEndpoint"];
			if (!String.IsNullOrEmpty(endPointName))
			{
				Amazon.RegionEndpoint endPoint = Amazon.RegionEndpoint.GetBySystemName(endPointName);
				if (endPoint == null)
					throw new Exception("Invalid Amazon AWS endpoint name: " + endPointName);

				sqsClient = new Amazon.SQS.AmazonSQSClient(endPoint);
			}
			else
			{
				sqsClient = new Amazon.SQS.AmazonSQSClient();
			}

			// Build our request
			var request = new Amazon.SQS.Model.SendMessageRequest()
				.WithMessageBody(messageBody)
				.WithQueueUrl(queueUrl);

			// Send to SQS
			var response = sqsClient.SendMessage(request);
			string messageId = response.SendMessageResult.MessageId;

			return new EmptyResult();
		}

    }
}
