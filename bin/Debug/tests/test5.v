// Script written in Val Language by ZioEren.
import("console");

var favourite_food;
print("What's your favourite food?");
favourite_food = input();

switch (favourite_food)
{
	case "ananas":
		print("I love that!");
		break;
	case "pizza":
		print("The Italian food is the best in the world!");
		break;
	case "banana":
		print("Mh... you like bananas :)")
		break;
	case "apple":
		print("You're so smart!");
		break;
	case "sushi":
		print("Bruh, you're surely the most loved person in the world.");
		break;
	default:
		print("Why don't you eat good food?");
		break;
}