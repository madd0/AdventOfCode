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

	var map = SpaceMap.Parse(text).Dump();
	
	var neighbors = NeighborMap.MakeNeighborMap(map).Dump();
	neighbors.Max.Dump("Max");
}

class SpaceMap : Map<char>
{
	public static SpaceMap Parse(TextReader reader)
	{
		var buffer = new List<char[]>();

		var map = new SpaceMap();

		char[] lineBuffer;
		while ((lineBuffer = reader.ReadLine()?.ToCharArray()) != null)
		{
			buffer.Add(lineBuffer);
		}

		map.Space = new char[buffer.Count][];

		for (var i = 0; i < buffer.Count; i++)
		{
			map.Space[i] = buffer[i];
		}

		return map;
	}
}

class Map<T>
{	
	public int Width => this.Space[0].Length;
	
	public int Height => this.Space.Length;

	public T[][] Space { get; protected set; }

	public object ToDump()
	{
		var builder = new StringBuilder();
		
		foreach (var line in this.Space)
		{
			foreach (var element in line)
			{
				builder.Append(element);
			}

			builder.AppendLine();
		}
		
		builder.AppendLine();

		return builder.ToString();
	}
}

class NeighborMap : Map<int>
{
	public int Max { get; private set; }
	
	public static NeighborMap MakeNeighborMap(SpaceMap map)
	{
		var result = new NeighborMap();
		
		var neighborCounts = result.Space = new int[map.Space.Length][];

		for (var i = 0; i < neighborCounts.Length; i++)
		{
			neighborCounts[i] = new int[map.Space[i].Length];
		}

		for (int row = 0; row < map.Space.Length; row++)
		{
			for (int col = 0; col < map.Space[row].Length; col++)
			{
				if (map.Space[row][col].IsEmpty())
				{
					continue;
				}
				
				var neighborCount = VerticalNeighbors(map, row, col);

				neighborCounts[row][col] = neighborCount;

				if (neighborCount > result.Max)
				{
					result.Max = neighborCount;
				}
			}
		}

		return result;
	}

	private static int VerticalNeighbors(SpaceMap map, int row, int col)
	{		
		var neighborCount = 0;

		for (int i = row - 1; i >= 0; i--)
		{
			if (map.Space[i][col].IsAsteroid())
			{
				neighborCount++;
				break;
			}
		}

		for (int i = row + 1; i < map.Height; i++)
		{
			if (map.Space[i][col].IsAsteroid())
			{
				neighborCount++;
				break;
			}
		}

		return neighborCount;
	}

}

public static class CharExtensions
{
	public static bool IsEmpty(this char c)
	{
		return c == '.';
	}
	
	public static bool IsAsteroid(this char c)
	{
		return c == '#';
	}
}