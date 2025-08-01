/// @func approach(val1, val2, amount)
/// @arg val1
/// @arg val2
/// @arg amount
// Moves "a" towards "b" by "amount" and returns the result

function approach(val1, val2, amount)
{
	if (val1 < val2)
	{
		return min(val1 + amount, val2);
	}
	else
	{
		return max(val1 - amount, val2);
	}
}