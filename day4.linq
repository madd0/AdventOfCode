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
	return false;
}
