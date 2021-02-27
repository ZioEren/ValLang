// Script written in Val Language by ZioEren.
import("console");
import("collections");

var list = [], num = 0;

println("Insert the number of the numbers you want to insert:");
num = inputInteger();

for (i = 0 to num)
{
	println("Insert the " + (i + 1) + " number:");
	var element = inputInteger();
	list += element;
}

var sum = 0, average = 0;

foreach (element in list)
{
	sum += element;
}

average = sum / getListLength(list);

println("The sum of all the elements of the list is: " + sum + ".");
println("The arithmetic average of all the elements of the list is: " + average + ".");