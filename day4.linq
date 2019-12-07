<Query Kind="Program" />

void Main()
{
	int min = 147981;
	int max = 691423;
	
	int matches = 0;
	
	for (int i = min; i <= max; i++)
	{
		if (IsMatch(i))
		{
			matches++;
		}
	}
	
	matches.Dump();
}

bool IsMatch(int i)
{
	var digits = GetDigits(i);
	
	var isIncreasing = true;
	var hasDouble = false;
	
	var digitCount = new byte[10];
	
	digitCount[digits[0]]++;
	
	for (int pos = 1; pos < digits.Length; pos++) 
	{
		hasDouble |= digits[pos] == digits[pos - 1];
		isIncreasing &= digits[pos] >= digits[pos - 1];
		digitCount[digits[pos]]++;
	}
	
	return isIncreasing && hasDouble && digitCount.Any(c => c == 2);
}

byte[] GetDigits(int i)
{
	var result = new byte[6];
	
	var digit = 5;
	while (i != 0)
	{
		result[digit--] = (byte)(i % 10);
		i = i / 10;
	}
	
	return result;
}
