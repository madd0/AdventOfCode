<Query Kind="Program" />

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, "day10.txt");
	TextReader text = new StreamReader(File.OpenRead(file));

	text = new StringReader(@".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##");

	var map = SpaceMap.Parse(text).Dump();

	var neighbors = NeighborMap.MakeNeighborMap(map).Dump();
	$"{neighbors.MaxCol},{neighbors.MaxRow}: {neighbors.Max}".Dump("Max");
	neighbors.Graph.Select(g => g.Value.Count).Max().Dump();
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
	
	public virtual string Format => "{0}";

	public object ToDump()
	{
		var builder = new StringBuilder();
		
		foreach (var line in this.Space)
		{
			foreach (var element in line)
			{
				builder.AppendFormat(this.Format, element);
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
	
	public int MaxRow { get; private set; }
	
	public int MaxCol { get; private set; }

	public override string Format => $"{{0:{Max}}}";
	
	public Dictionary<int, HashSet<int>> Graph { get; private set; } = new Dictionary<int, HashSet<int>>();

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
				
				result.CountNeighbors(map, row, col);
			}
		}

		return result;
	}
	
	private static int GetKey(int row, int col)
	{
		return col * 1000 + row;
	}

	private void CountNeighbors(SpaceMap map, int row, int col)
	{
		this.CountVerticalNeighbors(map, row, col);
		this.CountHorizontalNeighbors(map, row, col);
		this.CountOtherNeighbors(map, row, col);

		if (this.Space[row][col] > this.Max)
		{
			this.Max = this.Space[row][col];
			this.MaxRow = row;
			this.MaxCol = col;
		}
	}
	
	private void CountOtherNeighbors(SpaceMap map, int row, int col)
	{
		for (int candidateRow = map.Height - 1; candidateRow > row; candidateRow--)
		{
			var startCol = candidateRow == row ? col + 1 : 0;
			
			for (int candidateCol = startCol; candidateCol < map.Width; candidateCol++)
			{
				if (candidateCol == col || map.Space[candidateRow][candidateCol].IsEmpty())
				{
					continue;
				}
				
				var slope = (double)(candidateRow - row) / (double)(candidateCol - col);
				var origin = row - slope * col;
				Func<int, double> line = (int x) => x * slope + origin;

				var intercepted = false;
				
				var interceptorStart = Math.Min(candidateCol + 1, col + 1);
				var interceptorEnd = Math.Max(candidateCol, col);
				for (var interceptorCol = interceptorStart; interceptorCol < map.Width && interceptorCol < interceptorEnd; interceptorCol++)
				{
					var interceptorRow = line(interceptorCol);

					if (interceptorRow.IsInteger() &&
					    interceptorRow >= 0 && 
						interceptorRow < map.Height &&
						map.Space[(int)interceptorRow][interceptorCol].IsAsteroid())
					{
						intercepted = true;
						break;
					}
				}
				
				if (!intercepted)
				{
					this.AddNeighbor(row, col, candidateRow, candidateCol);
					this.Space[row][col]++;
					this.Space[candidateRow][candidateCol]++;
				}
			}
		}
	}

	private void CountVerticalNeighbors(SpaceMap map, int row, int col)
	{
		for (int i = row + 1; i < map.Height; i++)
		{
			if (map.Space[i][col].IsAsteroid())
			{
				this.AddNeighbor(row, col, i, col);
				this.Space[row][col]++;
				this.Space[i][col]++;
				break;
			}
		}
	}

	private void CountHorizontalNeighbors(SpaceMap map, int row, int col)
	{
		for (int i = col + 1; i < map.Width; i++)
		{
			if (map.Space[row][i].IsAsteroid())
			{
				this.AddNeighbor(row, col, row, i);
				this.Space[row][col]++;
				this.Space[row][i]++;
				break;
			}
		}
	}

	private void AddNeighbor(int row, int col, int nRow, int nCol)
	{
		var key = GetKey(row, col);
		var nKey = GetKey(nRow, nCol);
		
		if (!this.Graph.TryGetValue(key, out var currentSet))
		{
			this.Graph[key] = currentSet = new HashSet<int>();
		}
		
		if (!this.Graph.TryGetValue(nKey, out var neighborSet))
		{
			this.Graph[nKey] = neighborSet = new HashSet<int>();
		}
		
		currentSet.Add(nKey);
		neighborSet.Add(key);
	}
}

public static class Extensions
{
	public static bool IsEmpty(this char c)
	{
		return c == '.';
	}

	public static bool IsAsteroid(this char c)
	{
		return c == '#';
	}

	public static bool IsInteger(this double d)
	{
		return Math.Abs(d % 1) <= (Double.Epsilon * 100);
	}
}