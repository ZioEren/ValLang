// Script written in Val Language by ZioEren.
import("console");
import("network");

fun del_webhook(webhook)
{
	var http = HttpClient;
	http.set(webhook, "DELETE");
	http.send();
}

print("Insert the webhook to delete here: ")
var theWebhook = inputString();
del_webhook(theWebhook);
println("The webhook has been deleted!");