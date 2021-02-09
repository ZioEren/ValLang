// Script written in Val Language by ZioEren.
import("console");

namespace test
{
	fun add(a, b)
	{
		return a + b;
	}
	
	fun add(a, b, c)
	{
		return a + b + c;
	}
}

println(test::add(5, 6));
println(test::add(6, 6, 6));