<center><img src="https://cdn.discordapp.com/attachments/851093660720300034/858446436055384084/Group_1.png" width="1732" height="1732"></center>

# ValLang - A new interpreted scripting programming language

Val Language is a high-level interpreted scripting object-oriented programming language made entirely in C#.<br/>
This language has the fundamental statements and functionalities of every common high-level language.<br/>
The objective of that language is to combine the advantages of common languages to make a simple, understandable and versatile programming language.

# What is implemented in Val Language?

In Val Language there are global variables, constant variables, if/else/elif statements, switch statements, do while statements, while statements, for statements, foreach statements, structs, functions and lots lots more.<br/><br/>
Also, this language is not strictly-typed so every variable that you declare can immediately become a variable with another type & value at the same time, also this language does not need to specify the type but probably I'll add that feature in the future to facilitate the work to some developers.<br/><br/>
I want to make this programming language as more accessible to developers as I can, so I am trying to implement different implementations of structures and objects similar to the other common languages like Python, Java, C++, C# & more.<br/><br/>
I hope that you appreciate this work and if you have any issue with the language, do not hesitate to open an issue. And if you want to know more or give suggestions, you can DM me on Discord: ZioEren#5517.

# Example of script

```c#
// Script written in Val Language by ZioEren.
import "console";

var favourite_food;
println("What's your favourite food?");
favourite_food = inputString();

switch (favourite_food)
{
    case "ananas":
        println("I love that!");
        break;
    case "pizza":
        println("The Italian food is the best in the world!");
        break;
    case "banana":
        println("Mh... you like bananas :)")
        break;
    case "apple":
        println("You're so smart!");
        break;
    case "sushi":
        println("Bruh, you're surely the most loved person in the world.");
        break;
    default:
        println("Why don't you eat good food?");
        break;
}
```

# TODO

**1)** Add enums.<br/>
**2)** Add pointers and address references with symbols like '&' or '@'.<br/>
**3)** Add the feature to sum two functions into a unique function.<br/>
**4)** Add error management with the data structure 'try', adding also 'catch' and 'finally'.<br/>
**5)** Add 'this' operator for the actual global context.<br/>
**6)** Add object type 'class' with constructor and initialization. So this mean adding a new keyword 'new'.<br/>
**7)** Add the common semplification of the 'if' structure, using '?' and ':'.<br/>
**8)** Add the 'elif' keyword that semplifies the 'else if' expression.<br/>
**9)** Add static methods to classes.<br/>
**10)** Add ereditariety and polimorphism to classes.<br/>
**11)** Add "unlimited" parameters possibility to a function by using '...'.<br/>
**12)** Add boolean value type, 'true' and 'false' that are replacing the common '1' and '0' (integers).<br/>
**13)** Add dictionaries:


```c#
var x = {"ciao":"lol", "miao":"sos"};
println(x["ciao"]); // OUTPUT: "lol".
```

**14)** Add a new type of 'for' constructor:

```c#
for (var i = 0, var j = 0; i < 10, j < 10; i++, j++)
{
    println(i + " " + j);
}
```

**15)** Add a simplification of 'foreach' in the 'for' constructor like Java does:

```c#
var lista = [5, 3, 2];

for (elemento: lista)
{
    println(elemento);
}
```

**16)** Add hexadecimal values, like '0x0' (0 in decimal) and '0x5' (5 in decimal) or '0xA' (10 in decimal).<br/>
**17)** Add a feature to write the numbers in different base:

```c#
var x = 0;

x = 0D; // DECIMAL (INTEGER)
x = 0d; // DECIMAL (INTEGER)

x = 0.0F; // DECIMAL (FLOAT)
x = 0.0f; // DECIMAL (FLOAT)

x = 0H; // HEXADECIMAL
x = 0h; // HEXADECIMAL

x = 0101B; // BINARY
x = 0101b; // BINARY

x = 0O; // OCTAL
x = 0o; // OCTAL
```

**18)** Actually to declare the instance of a 'struct' you need to assign the identifier name to a value to create new instance. Add so a new keyword called 'new' so for a struct you'll need to do this following:

```c#
import "console";

struct ciao
{
    var x = 2, y = 5, z = 9;
    
    fun printAll()
    {
        println(x + " " + y + " " + z);
    }
}

var lol = new ciao();
lol.printAll();
```

**19)** Add multi-access to objects (structs, namespaces and classes). What I mean is the following example:

```c#
import "console";

struct s1
{
    var x = 2, y = 5, z = 7;
}

struct s2
{
    var miao = new s1();
    
    fun printThings()
    {
        println(miao.x + " " + miao.y + " " + miao.z);
    }
}

var declared = new s2();
declared.printThings();
println(declared.miao.x + " " + declared.miao.y + " " + declared.miao.z);
```

**20)** Fix built-in namespace problem:

```c#
import "console";

println(Math.pi);
Math.sin(5);

var x = Math;

println(x.pi);
x.sin(5);
```

**21)** Also fix this problem of built-in namespaces:

```c#
import "console";
use Math;

println(Math.pi);
println(pi);

println(Math.sin(5));
println(sin(5));
```

**22)** Fix namespaces:


```c#
import "console";

namespace ciao
{
    var x = 5, y = 3, z = 2;
	
    fun printIt()
    {
        println(x + " " + y + " " + z);
    }
}

println(ciao.x + " " + ciao.y + " " + ciao.z);
ciao.printIt();

var x = ciao;

println(x.x + " " + x.y + " " + x.z);
x.printIt();
```
