// Script written in Val Language by ZioEren.
import("console");

struct Position
{
	var x, y, z;
	
	fun getSum()
	{
		return x + y + z;
	}
}

var pos = Position; // Create a new istance of Position structure.
var a, b, c;

print("Insert x coordinate: ");
a = input_int();

print("Insert y coordinate: ");
b = input_int();

print("Insert z coordinate: ");
c = input_int();

pos.x = a;
pos.y = b;
pos.z = c;

print("The sum of all coordinates is: " + pos.getSum());