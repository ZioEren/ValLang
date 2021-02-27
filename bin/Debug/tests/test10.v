// Script written in Val Language by ZioEren.
import("console");

var x = 10;

repeat_10_times:

if (x > 0)
{
	x--;
	println("Hello, World!");
	goto repeat_10_times;
}