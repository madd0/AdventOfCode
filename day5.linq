<Query Kind="Program" />

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, ".\\day5.txt");
	var text = File.ReadAllText(file);
	
	var data = new []
	{
		"1002,4,3,4,33",
		"1,0,0,0,99",
		"2,3,0,3,99",
		"2,4,4,5,99,0",
		"1,1,1,4,99,5,6,0,99"
	};

	foreach (var element in data)
	{
		Run(Parse(element));
	}
	
	Run(Parse(text));
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
		var instruction = Instruction.MakeInstruction(memory, start);
		
		start = instruction.Execute();
	}
	while (start < memory.Length);
	
	return memory[0];
}

class Instruction
{
	public static Instruction Exit { get; } = new Instruction { Opcode = 99 };
	
	private static readonly Dictionary<int, int> instructionParameters = new Dictionary<int, int>
	{
		[1] = 3,
		[2] = 3,
		[3] = 1,
		[4] = 1,
		[99] = 0
	};

	public int Opcode { get; private set; }
	
	public int[] Memory { get; private set; }
	
	public int Address { get; private set; }
	
	public int[] Parameters { get; private set; }

	public int ParameterCount => instructionParameters[Opcode];

	public Action Operation
	{
		get
		{
			switch (Opcode)
			{
				case 1:
					return () => Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] + Memory[this.GetParamAddress(1)];
				case 2:
					return () => Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] * Memory[this.GetParamAddress(1)];
				case 3:
					return () => Memory[this.GetParamAddress(0)] = int.Parse(Util.ReadLine());
				case 4:
					return () => Memory[this.GetParamAddress(0)].Dump();
				default:
					return () => { };
			}
		}
	}
	
	int GetParamAddress(int paramNumber)
	{
		var paramAddress = this.Address + paramNumber + 1;
		
		if (this.Parameters[paramNumber] == 0) {
			return this.Memory[paramAddress];
		}
		else {
			return paramAddress;
		}
	}

	public int Execute()
	{
		if (this.Opcode == 99)
		{
			return this.Memory.Length;
		}
		
		this.Operation();
		
		return Address + ParameterCount + 1;
	}
	
	public static Instruction MakeInstruction(int[] memory, int address)
	{
		var opCode = memory[address] % 100;

		var instruction = new Instruction
		{
			Opcode = opCode,
			Memory = memory,
			Address = address,
			Parameters = new int[instructionParameters[opCode]]
		};
		
		var modes = memory[address] / 100;
		
		for (int i = 0; modes > 0; i++)
		{
			instruction.Parameters[i] = modes % 10;
			modes = modes / 10;
		}
		
		return instruction;
	}
}