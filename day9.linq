<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, ".\\day9.txt");
	var text = File.ReadAllText(file);

	var program = Program.Parse(text);
	program.IO = new StdIo();
	
	program.Run();
}

class Program
{
	private int position;

	private int[] Memory { get; set; }

	public IO IO { get; set; } = new IO();

	public static Program Parse(string memory)
	{
		var p = new Program();
		p.Memory = memory.Split(',').Select(int.Parse).ToArray();

		return p;
	}

	public bool Run()
	{
		var memoryPosition = Execute(Memory);

		return memoryPosition == Memory.Length;
	}

	int Execute(int[] memory)
	{
		var breakRequested = false;
		
		while (position < memory.Length)
		{
			var instruction = Instruction.MakeInstruction(memory, position);

			if (breakRequested && instruction.Opcode != 99)
			{
				break;
			}

			position = instruction.Execute(IO);
			
			breakRequested = instruction.Exit;
		}

		return position;
	}
}

class StdIo : IO
{
	public override Queue<int> Input
	{
		get
		{
			var input = int.Parse(Util.ReadLine());
			base.Input.Enqueue(input);
			return base.Input;
		}
	}

	public override int Output { get => base.Output; set => Console.WriteLine(base.Output = value); }
}

class IO
{
	public virtual Queue<int> Input { get; private set; } = new Queue<int>();
	public virtual int Output { get; set; }
}

class Instruction
{
	public bool Exit { get; private set; }

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

	public Func<IO, int> Operation
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
					return (buffers) => { Memory[this.GetParamAddress(0)] = buffers.Input.Dequeue(); return next; };
				case 4:
					return (buffers) => { buffers.Output = Memory[this.GetParamAddress(0)]; this.Exit = true; return next; };
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

	public int Execute(IO io)
	{
		if (this.Opcode == 99)
		{
			return this.Memory.Length;
		}

		return this.Operation(io);
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