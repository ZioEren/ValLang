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

println("Insert x coordinate: ");
a = input_int();

println("Insert y coordinate: ");
b = input_int();

println("Insert z coordinate: ");
c = input_int();

pos.x = a;
pos.y = b;
pos.z = c;

println("The sum of all coordinates is: " + pos.getSum());