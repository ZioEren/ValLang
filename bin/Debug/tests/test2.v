// Script written in Val Language by ZioEren.
import("console");
import("lists");

var list = [], num = 0;

print("Insert the number of the numbers you want to insert:");
num = input_int();

for (i = 0 to num)
{
	print("Insert the " + (i + 1) + " number:");
	var element = input_int();
	list += element;
}

var sum = 0, average = 0;

foreach (element in list)
{
	sum += element;
}

average = sum / len(list);

print("The sum of all the elements of the list is: " + sum + ".");
print("The arithmetic average of all the elements of the list is: " + average + ".");