import("console");

namespace ciao
{
	fun add(a, b)
	{
		return a + b;
	}

	fun add(a, b, c, d = 3)
	{
		return a + b + c + d;
	}

	fun add(a)
	{
		return a + 1;
	}

	fun add()
	{
		return 5 + 5;
	}
}

println(ciao::add(5, 6));
println(ciao::add(6, 6, 6));
println(ciao::add(5));
println(ciao::add());
println(ciao::add(6, 6, 6, 7));