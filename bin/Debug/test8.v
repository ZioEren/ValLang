// Script written in Val Language by ZioEren.
import("console");

var x = 5;

fun repeat_5_times()
{
	if (x > 0)
	{
		println("Hello, World!");
		x--;
		repeat_5_times();
	}
}

repeat_5_times();