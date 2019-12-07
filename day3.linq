<Query Kind="Program" />

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	
	var space = new Space();

	var challengeData = File.ReadAllLines(Path.Combine(dir, "day3.txt"));

	var wire1 = challengeData[0].Split(',');
	var wire2 = challengeData[1].Split(',');
//		var wire1 = "R8,U5,L5,D3".Split(',');
//		var wire2 = "U7,R6,D4,L4".Split(',');
	//	var wire1 = "R75,D30,R83,U83,L12,D49,R71,U7,L72".Split(',');
	//	var wire2 = "U62,R66,U55,R34,D71,R55,D58,R83".Split(',');

	space.LayCable(wire1);
	space.LayCable(wire2);

	if (space.Size < 1000)
	{
		var file = Path.Combine(dir, "map.txt");
		using (var writer = new StreamWriter(File.Open(file, FileMode.Truncate)))
		{
			space.Draw(writer);
		}
	}

	space.ClosestIntersection().Dump();
	space.ClosestIntersectionByWireDistance().Dump();
}

class Point
{
	public char Char { get; set; }
	public int Row { get; set; }
	public int Col { get; set; }
	public int Distance => Math.Abs(Row) + Math.Abs(Col);
	public int WireDistance => Wires.Sum(w => w.Value);
	
	public Dictionary<int,int> Wires {get; private set;} = new Dictionary<int,int>();
	
	public bool AddWire(int id, int distance)
	{
		var intersection = false;
		
		var wireExists = Wires.ContainsKey(id);
		
		if (!wireExists)
		{
			Wires.Add(id, distance);
		}
		else
		{
			Char = '+';
		}
		
		if (Wires.Count > 1)
		{
			Char = 'X';
			intersection = true;
		}
		
		return intersection;
	}
}

class Space
{
	Dictionary<int, Dictionary<int, Point>> space = new Dictionary<int, Dictionary<int, Point>>();

	(int min, int max) xSpaceBounds = (0, 0);
	(int min, int max) ySpaceBounds = (0, 0);
	
	List<Point> intersections = new List<Point>();
	
	int currentCable = 0;
	
	public int Size => Math.Max(Math.Abs(xSpaceBounds.min) + Math.Abs(xSpaceBounds.max), Math.Abs(ySpaceBounds.min) + Math.Abs(ySpaceBounds.max));
	
	public Space()
	{
		LayCable(0, 0, 'o', 0, 0);
	}

	public Point ClosestIntersection()
	{
		var intersection = default(Point);

		foreach (var i in intersections)
		{
			if (intersection == null || i.Distance < intersection.Distance)
			{
				intersection = i;
			}
		}

		return intersection;
	}

	public Point ClosestIntersectionByWireDistance()
	{
		var intersection = default(Point);

		foreach (var i in intersections)
		{
			if (intersection == null || i.WireDistance < intersection.WireDistance)
			{
				intersection = i;
			}
		}

		return intersection;
	}

	void LayCable(int x, int y, char v, int id, int distance)
	{
		if (!space.TryGetValue(y, out var ySpace))
		{
			ySpace = new Dictionary<int, Point>();
			space.Add(y, ySpace);
		}
		
		if (!ySpace.TryGetValue(x, out var c))
		{
			c = new Point { Char = v, Row = y, Col = x };
		}

		var isIntersection = c.AddWire(id, distance);

		if (isIntersection)
			intersections.Add(c);

		space[y][x] = c;
		
		xSpaceBounds.min = Math.Min(xSpaceBounds.min, x);
		xSpaceBounds.max = Math.Max(xSpaceBounds.max, x);
		ySpaceBounds.max = Math.Max(ySpaceBounds.max, y);
		ySpaceBounds.min = Math.Min(ySpaceBounds.min, y);
	}

	public void LayCable(string[] wire)
	{
		var currPos = new[] { 0, 0 };
		var cableId = currentCable++;
		var distance = 0;
		
		foreach (var element in wire)
		{
			var direction = element[0];
			var amount = int.Parse(element.Substring(1));
			
			var c = direction == 'R' || direction == 'L' ? '-' : '|';
			var mult = direction == 'L' || direction == 'D' ? -1 : 1;
			var dir = direction == 'R' || direction == 'L' ? 0 : 1;
			
			for (int coord = 1; coord <= amount; coord++)
			{
				currPos[dir] += mult;
				
				if (coord == amount) {
					c = '+';
				}
				
				LayCable(currPos[0], currPos[1], c, cableId, ++distance);
			}
		}
	}

	public void Draw(TextWriter writer)
	{		
		for (var row = ySpaceBounds.max + 1; row >= ySpaceBounds.min - 1; row--)
		{
			for (var col = xSpaceBounds.min - 1; col <= xSpaceBounds.max + 1; col++)
			{
				char c;
				
				if (space.TryGetValue(row, out var xSpace) &&
				    xSpace.TryGetValue(col, out var box))
				{
					c = box.Char;
				}
				else
				{
					c = ' ';
				}
				
				writer.Write(c);
			}
			
			writer.WriteLine();
		}
	}
}