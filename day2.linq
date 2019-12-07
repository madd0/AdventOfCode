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
		"1,1,1,4,99,5,6,0,99"
	};

	foreach (var element in data)
	{
		Run(Parse(element));
	}
	
	for (var noun = 0; noun < 100; noun++)
	{
		for (var verb = 0; verb < 100; verb++)
		{
			var challengeData = Parse(text);
			challengeData[1] = noun;
			challengeData[2] = verb;
			
			var result = Execute(challengeData);
			
			if (result == 19690720)
			{
				$"Noun: {noun}, Verb: {verb}, Result: {100 * noun + verb}".Dump();
			}
		}
	}
	
}

void Run(int[] memory) 
{
	var result = Execute(memory);
	string.Join(",", memory.Select(i => i.ToString())).Dump($"Result: {result}");
}

int[] Parse(string memory)
{
	return memory.Split(',').Select(int.Parse).ToArray();
}

int Execute(int[] memory)
{
	var start = 0;
	
	do
	{
		var instruction = memory[start];
		
		if (instruction == 99)
		{
			break;
		}
		
		Func<int, int, int> operation;

		if (instruction == 1)
		{
			operation = (a, b) => a + b;
		}
		else
		{
			operation = (a, b) => a * b;
		}
		
		memory[memory[start + 3]] = operation(memory[memory[start + 1]], memory[memory[start + 2]]);
		
		start += 4;
	}
	while (start + 4 < memory.Length);
	
	return memory[0];
}