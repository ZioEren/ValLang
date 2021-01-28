import("console");
import("network");

fun del_webhook(webhook)
{
	var http = HttpClient;
	http.set(webhook, "DELETE");
	http.send();
}

print("Inserisci il webhook: ")
var theWebhook = input();
del_webhook(theWebhook);
println("Il webhook è stato eliminato!");