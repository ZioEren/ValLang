// Script written in Val Language by ZioEren.
import("console");

fun print_all(set)
{
	foreach (element in set)
	{
		println(element);
	}
}

print_all({5, 3, 3, 3, 2, 7});