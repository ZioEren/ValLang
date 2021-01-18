// Script written in Val Language by ZioEren.
import("console");

fun repeat_50_times()
{
	for (i = 0 to 50)
	{
		print("This is the " + (i + 1) + " time.");
	}
}

var answer = "";

do
{
	print("Do you want to start repeating 50 times (y/n) ?");
	answer = input();
}
while (answer != 'y' && answer != 'n');

if (answer == 'y')
{
	repeat_50_times();
}
else
{
	exit();
	return;
}

answer = "";
del repeat_50_times; // Delete the function from the memory.

do
{
	print("Do you want to restart repeating 50 times (y/n) ?");
	answer = input();
}
while (answer != 'y' && answer != 'n');

if (answer == 'y')
{
	repeat_50_times(); // This will surely generate an error.
}
else
{
	exit();
	return;
}