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

	public override string Format => $"{{0,{this.Max.ToString().Length}}} ";

	public Dictionary<int, HashSet<int>> Graph { get; private set; } = new Dictionary<int, HashSet<int>>();

	private NeighborMap(SpaceMap map)
	{
		var neighborCounts = this.Space = new int[map.Space.Length][];

		for (var i = 0; i < neighborCounts.Length; i++)
		{
			neighborCounts[i] = new int[map.Space[i].Length];
		}
	}

	public static NeighborMap MakeNeighborMap(SpaceMap map)
	{
		var result = new NeighborMap(map);

		result.CountNeighbors(map);

		return result;
	}

	private static int GetKey(int row, int col)
	{
		return col * 1000 + row;
	}
	
	private static (int row, int col) SplitKey(int key)
	{
		return (key % 1000, key / 1000);
	}

	private void CountNeighbors(SpaceMap map)
	{
		for (int row = map.Space.Length - 1; row >= 0; row--)
		{
			for (int col = map.Space[row].Length - 1; col >= 0; col--)
			{
				if (map.Space[row][col].IsEmpty())
				{
					continue;
				}

				this.CountNeighbors(map, row, col);
			}
		}
	}

	private void CountNeighbors(SpaceMap map, int row, int col)
	{
		var skipCurrentCol = false;
		
		this.Graph[GetKey(row, col)] = new HashSet<int>();

		for (int candidateRow = row; candidateRow < map.Height; candidateRow++)
		{
			var startCol = candidateRow == row ? col + 1 : 0;

			for (int candidateCol = startCol; candidateCol < map.Width; candidateCol++)
			{					
				if ((row == candidateRow && col == candidateCol) ||
				    map.Space[candidateRow][candidateCol].IsEmpty() ||
					(skipCurrentCol && candidateCol == col))
				{
					continue;
				}

				if (candidateRow == row)
				{
					this.AddNeighbor(row, col, candidateRow, candidateCol);
					break;
				}
				else if (candidateCol == col)
				{
					this.AddNeighbor(row, col, candidateRow, candidateCol);
					skipCurrentCol = true;
				}
				else
				{
					var slope = (decimal)(candidateRow - row) / (decimal)(candidateCol - col);
					var origin = row - slope * col;
					Func<int, decimal> line = (int x) => x * slope + origin;

					var isCandidateSeen = false;

					foreach (var element in this.Graph[GetKey(row, col)])
					{
						if (IsOnLine(line, element))
						{
							isCandidateSeen = HasSeen(element, candidateRow, candidateCol);
							break;
						}
					}

					if (!isCandidateSeen)
					{
						this.AddNeighbor(row, col, candidateRow, candidateCol);
					}
				}
			}
		}
	}

	private bool HasSeen(int element, int candidateRow, int candidateCol)
	{
		var candidateKey = GetKey(candidateRow, candidateCol);
		
		return this.Graph[element].Contains(candidateKey) ||
			this.Graph[element].Any(neighbor => HasSeen(neighbor, candidateRow, candidateCol));
	}

	private bool IsOnLine(Func<int, decimal> line, int element)
	{
		var (row, column) = SplitKey(element);
		
		var elementRow = line(column);
		
		return Math.Abs(row - elementRow) < 0.0000000001m;
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

		this.Space[row][col]++;
		this.Space[nRow][nCol]++;
		
		this.SetMax(row, col);
		this.SetMax(nRow, nCol);

		currentSet.Add(nKey);
		neighborSet.Add(key);
	}
	
	private void SetMax(int row, int col)
	{
		if (this.Space[row][col] > this.Max)
		{
			this.Max = this.Space[row][col];
			this.MaxRow = row;
			this.MaxCol = col;
		}
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
}