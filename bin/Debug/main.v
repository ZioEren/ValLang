import("console");
import("lists");

var list = [5, 3, 2, "ciao a tutti!", 44];

println("\r\nCHE DIREBBI: \r\n");

foreach (elemento in list)
{
	println(elemento);
}

println("\r\nLOL: \r\n");

listReverse(list);

foreach (elemento in list)
{
	println(elemento);
}