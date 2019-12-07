<Query Kind="Program" />

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, ".\\day2.txt");
	var text = File.ReadAllText(file);
	
	var data = new []
	{
		"1,0,0,0,99",
		"2,3,0,3,99",
		"2,4,4,5,99,0",
		"1,1,1,4,99,5,6,0,99",
		text
	};

	foreach (var element in data)
	{
		Run(Parse(element));
	}
}

void Run(int[] memory) 
{
	var result = Execute(memory);
	string.Join(",", memory.Select(i => i.ToString())).Dump($"Result: {memory[0]}");
}

int[] Parse(string memory)
{
	return memory.Split(',').Select(int.Parse).ToArray();
}

int Execute(int[] memory)
{
	var start = 0;
	var length = Math.Min(memory.Length, 4);
	
	
	
	return memory[0];
}