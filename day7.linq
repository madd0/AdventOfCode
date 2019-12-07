<Query Kind="Program" />

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, ".\\day7.txt");
	var text = File.ReadAllText(file);

	var maxSignal = int.MinValue;
	var sequence = string.Empty;

	int maxRange = 4;
	int minRange = 0;
	
	for (int tenks = maxRange; tenks >= minRange; tenks--)
	{
		for (int ks = maxRange; ks >= minRange; ks--)
		{
			if (ks == tenks) continue;

			for (int hs = maxRange; hs >= minRange; hs--)
			{
				if (hs == tenks || hs == ks) continue;

				for (int ts = maxRange; ts >= minRange; ts--)
				{
					if (ts == tenks || ts == ks || ts == hs) continue;

					for (int us = maxRange; us >= minRange; us--)
					{
						if (us == tenks || us == ks || us == hs || us == ts) continue;
												
						var buffers = new[] { 0, 0, 0, 0 };
						buffers[1] = tenks;
						buffers[2] = Run(Parse(text), buffers);
						buffers[1] = ks;
						buffers[2] = Run(Parse(text), buffers);
						buffers[1] = hs;
						buffers[2] = Run(Parse(text), buffers);
						buffers[1] = ts;
						buffers[2] = Run(Parse(text), buffers);
						buffers[1] = us;
						var signal = Run(Parse(text), buffers);
												
						if (signal > maxSignal)
						{
							maxSignal = signal;
							sequence = $"{tenks}{ks}{hs}{ts}{us}";
						}
					}
				}
			}
		}
	}
	
	maxSignal.Dump("Signal");
	sequence.Dump("Sequence");
}

int Run(int[] memory, int[] buffers)
{
	buffers[0] = 0;
	buffers[buffers.Length - 1] = 0;
	
	var result = Execute(memory, buffers);

	return buffers[buffers.Length - 1];
}

int[] Parse(string memory)
{
	return memory.Split(',').Select(int.Parse).ToArray();
}

int Execute(int[] memory, int[] buffers)
{
	var start = 0;

	do
	{
		var instruction = Instruction.MakeInstruction(memory, start);

		start = instruction.Execute(buffers);
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
		[5] = 2,
		[6] = 2,
		[7] = 3,
		[8] = 3,
		[99] = 0
	};

	public int Opcode { get; private set; }

	public int[] Memory { get; private set; }

	public int Address { get; private set; }

	public int[] Parameters { get; private set; }

	public int ParameterCount => instructionParameters[Opcode];

	public Func<int[], int> Operation
	{
		get
		{
			var next = Address + ParameterCount + 1;

			switch (Opcode)
			{
				case 1:
					return (buffers) => { Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] + Memory[this.GetParamAddress(1)]; return next; };
				case 2:
					return (buffers) => { Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] * Memory[this.GetParamAddress(1)]; return next; };
				case 3:
					return (buffers) => { Memory[this.GetParamAddress(0)] = buffers[++buffers[0]]; return next; };
				case 4:
					return (buffers) => { buffers[buffers.Length - 1] = Memory[this.GetParamAddress(0)]; return next; };
				case 5:
					return (buffers) => Memory[this.GetParamAddress(0)] != 0 ? Memory[this.GetParamAddress(1)] : next;
				case 6:
					return (buffers) => Memory[this.GetParamAddress(0)] == 0 ? Memory[this.GetParamAddress(1)] : next;
				case 7:
					return (buffers) => { Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] < Memory[this.GetParamAddress(1)] ? 1 : 0; return next; };
				case 8:
					return (buffers) => { Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] == Memory[this.GetParamAddress(1)] ? 1 : 0; return next; };
				default:
					return (buffers) => next;
			}
		}
	}

	int GetParamAddress(int paramNumber)
	{
		var paramAddress = this.Address + paramNumber + 1;

		if (this.Parameters[paramNumber] == 0)
		{
			return this.Memory[paramAddress];
		}
		else
		{
			return paramAddress;
		}
	}

	public int Execute(int[] buffers)
	{
		if (this.Opcode == 99)
		{
			return this.Memory.Length;
		}

		return this.Operation(buffers);
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