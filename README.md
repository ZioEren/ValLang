# ValLang - A new interpreted scripting programming language

Val Language is a high-level interpreted scripting object-oriented programming language made entirely in C#.
This language has the fundamental statements and functionalities of every high-level language.
The objective of that language is to combine the advantages of languages to make a simple, understandable, versatile, and fluid language.

# What is implemented in Val Language?

In Val Language there are global variables, constant variables, if/else/elif statements, switch statements, do while statements, while statements, for statements, foreach statements, structs, functions and lots lots more.
Also, there are no types in this language. Every variable can be an integer/float, a string, a list or a struct.

# Example of script

```c#
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
a = inputNumber();

println("Insert y coordinate: ");
b = inputNumber();

println("Insert z coordinate: ");
c = inputNumber();

pos.x = a;
pos.y = b;
pos.z = c;

println("The sum of all coordinates is: " + pos.getSum());
```
