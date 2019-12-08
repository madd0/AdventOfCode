<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, ".\\day7.txt");
	var text = File.ReadAllText(file);

	var maxSignal = int.MinValue;
	var sequence = string.Empty;

	int maxRange = 9;
	int minRange = 5;

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

						var ampA = new Amplifier(Program.Parse(text), tenks, 'A');
						var ampB = new Amplifier(Program.Parse(text), ks, 'B');
						var ampC = new Amplifier(Program.Parse(text), hs, 'C');
						var ampD = new Amplifier(Program.Parse(text), ts, 'D');
						var ampE = new Amplifier(Program.Parse(text), us, 'E');

						ampA.Link(ampB);
						ampB.Link(ampC);
						ampC.Link(ampD);
						ampD.Link(ampE);
						ampE.Link(ampA);

						ampA.Run(0);
						var signal = await ampE.Output();

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

class Program
{
	private int position;

	private int[] Memory { get; set; }

	public IO IO { get; private set; } = new IO();

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

class Amplifier
{
	private readonly Program program;
	private readonly int phase;
	private readonly TaskCompletionSource<int> tcs;
	private readonly char name;
	private Amplifier next;

	public Amplifier(Program program, int phase, char name)
	{
		this.program = program;
		this.program.IO.Input.Enqueue(phase);
		this.phase = phase;
		this.name = name;
		this.tcs = new TaskCompletionSource<int>();
	}

	public void Link(Amplifier next)
	{
		this.next = next;
	}

	public Task<int> Output()
	{
		return tcs.Task;
	}

	public void Run(int signal)
	{
		program.IO.Input.Enqueue(signal);
		var ranToEnd = program.Run();

		//$"{name}: {program.IO.Output}".Dump();
		
		if (ranToEnd)
		{
			if (!tcs.TrySetResult(program.IO.Output))
			{
				return;
			}
		}

		if (this.next != null)
		{
			this.next.Run(program.IO.Output);
		}
	}
}

class IO
{
	public Queue<int> Input { get; private set; } = new Queue<int>();
	public int Output { get; set; }
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