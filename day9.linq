<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, ".\\day9.txt");
	var text = File.ReadAllText(file);
	text = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
	var program = Program.Parse(text);
	program.IO = new StdIo();
	
	program.Run();
}

class Program
{
	private readonly MemoryAddress position = new MemoryAddress();

	private Memory Memory { get; set; }

	public IO IO { get; set; } = new IO();

	public static Program Parse(string memory)
	{
		var p = new Program();
		p.Memory = new Memory(memory.Split(',').Select(long.Parse));

		return p;
	}

	public bool Run()
	{
		Execute();

		return position.Position == Memory.Length;
	}

	void Execute()
	{
		var breakRequested = false;
		
		while (position.Position < this.Memory.Length)
		{
			var instruction = Instruction.MakeInstruction(this.Memory, position);

			if (breakRequested && instruction.Opcode != 99)
			{
				break;
			}

			instruction.Execute(IO);
			
			breakRequested = instruction.Exit;
		}
	}
}

class Memory
{
	private Dictionary<int, long> memory;

	public Memory(IEnumerable<long> enumerable)
	{
		this.memory = enumerable.Select((v, i) => new { Index = i, Value = v }).ToDictionary(e => e.Index, e => e.Value);
	}

	public long this[int position]
	{
		get 
		{
			if (!this.memory.TryGetValue(position, out var result))
			{
				this.memory[position] = result = 0;
			}
			
			return result;
		}
		set
		{
			this.memory[position] = value;
		}
	}

	public int Length
	{
		get
		{
			return this.memory.Keys.Max();
		}
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

	public override long Output { get => base.Output; set => Console.WriteLine(base.Output = value); }
}

class IO
{
	public virtual Queue<int> Input { get; private set; } = new Queue<int>();
	public virtual long Output { get; set; }
}

class MemoryAddress
{
	public int Position { get; set; }
	public int BaseAddress { get; set; }
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
		[9] = 1,
		[99] = 0
	};

	public int Opcode { get; private set; }

	public Memory Memory { get; private set; }

	public MemoryAddress Address { get; private set; }

	public int[] Parameters { get; private set; }

	public int ParameterCount => instructionParameters[Opcode];

	public Func<IO, int> Operation
	{
		get
		{
			var next = Address.Position + ParameterCount + 1;

			switch (Opcode)
			{
				case 1:
					return (buffers) => { Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] + Memory[this.GetParamAddress(1)]; return next; };
				case 2:
					return (buffers) => { Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] * Memory[this.GetParamAddress(1)]; return next; };
				case 3:
					return (buffers) => { Memory[this.GetParamAddress(0)] = buffers.Input.Dequeue(); return next; };
				case 4:
					return (buffers) => { buffers.Output = Memory[this.GetParamAddress(0)]; this.Exit = false; return next; };
				case 5:
					return (buffers) => Memory[this.GetParamAddress(0)] != 0 ? (int)Memory[this.GetParamAddress(1)] : next;
				case 6:
					return (buffers) => Memory[this.GetParamAddress(0)] == 0 ? (int)Memory[this.GetParamAddress(1)] : next;
				case 7:
					return (buffers) => { Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] < Memory[this.GetParamAddress(1)] ? 1 : 0; return next; };
				case 8:
					return (buffers) => { Memory[this.GetParamAddress(2)] = Memory[this.GetParamAddress(0)] == Memory[this.GetParamAddress(1)] ? 1 : 0; return next; };
				case 9:
					return (buffers) => { this.Address.BaseAddress += this.GetParamAddress(0); return next; };
				default:
					return (buffers) => next;
			}
		}
	}

	int GetParamAddress(int paramNumber)
	{
		var paramAddress = this.Address.Position + paramNumber + 1;

		if (this.Parameters[paramNumber] == 0)
		{
			return (int)this.Memory[paramAddress];
		}
		else if (this.Parameters[paramNumber] == 2)
		{
			return this.Address.BaseAddress + (int)this.Memory[paramAddress];
		}
		else
		{
			return paramAddress;
		}
	}

	public void Execute(IO io)
	{
		if (this.Opcode == 99)
		{
			this.Address.Position = this.Memory.Length;
		}

		this.Address.Position = this.Operation(io);
	}

	public static Instruction MakeInstruction(Memory memory, MemoryAddress address)
	{
		var opCode = (int)(memory[address.Position] % 100);

		var instruction = new Instruction
		{
			Opcode = opCode,
			Memory = memory,
			Address = address,
			Parameters = new int[instructionParameters[opCode]]
		};

		var modes = (int)(memory[address.Position] / 100);

		for (int i = 0; modes > 0; i++)
		{
			instruction.Parameters[i] = modes % 10;
			modes = modes / 10;
		}

		return instruction;
	}
}