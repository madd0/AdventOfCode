<Query Kind="Program" />

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, "day10.txt");
	TextReader text = new StreamReader(File.OpenRead(file));

	text = new StringReader(@".#..#
.....
#####
....#
...##");

	var map = Map.Parse(text);
	
	map.Dump();
}

// Define other methods and classes here
class Map
{
	private char[][] _space;
	
	public int Width => _space[0].Length;
	
	public int Height => _space.Length;
	
	public static Map Parse(TextReader reader)
	{
		var buffer = new List<char[]>();
		
		var map = new Map();
		
		char[] lineBuffer;
		while ((lineBuffer = reader.ReadLine()?.ToCharArray()) != null)
		{
			buffer.Add(lineBuffer);
		}
		
		map._space = new char[buffer.Count][];
		
		for (var i = 0; i < buffer.Count; i++)
		{
			map._space[i] = buffer[i];
		}
		
		return map;
	}
	
	public object ToDump()
	{
		var builder = new StringBuilder();
		
		foreach (var line in _space)
		{
			foreach (var element in line)
			{
				builder.Append(element);
			}
			
			builder.AppendLine();
		}
		
		return builder.ToString();
	}
}