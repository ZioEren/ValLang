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
```

# TODO

- Labels and "goto" keyword
- "this" keyword
- Pointers and references
- New class object type
- New enum object type
