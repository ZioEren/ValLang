// Script written in Val Language by ZioEren.
import("console");

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