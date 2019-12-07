<Query Kind="Program" />

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	
	var space = new Space();

	var challengeData = File.ReadAllLines(Path.Combine(dir, "day3.txt"));

//	var wire1 = challengeData[0].Split(',');
//	var wire2 = challengeData[1].Split(',');
	var wire1 = "R75,D30,R83,U83,L12,D49,R71,U7,L72".Split(',');
	var wire2 = "U62,R66,U55,R34,D71,R55,D58,R83".Split(',');

	space.LayCable(wire1);
	space.LayCable(wire2);

	var file = Path.Combine(dir, "map.txt");
	using (var writer = new StreamWriter(File.OpenWrite(file)))
	{
		space.Draw(writer);
	}
	
	space.ClosestIntersection().Dump();
}

class Space
{
	Dictionary<int, Dictionary<int, (char, int)>> space = new Dictionary<int, Dictionary<int, (char, int)>>();

	(int min, int max) xSpaceBounds = (0, 0);
	(int min, int max) ySpaceBounds = (0, 0);
	
	List<(int,int,int)> intersections = new List<(int,int,int)>();
	
	int currentCable = 0;
	
	public Space()
	{
		LayCable(0, 0, 'o', 0);
	}
	
	public (int, int, int) ClosestIntersection()
	{
		var intersection = (0, 0, 0);
		
		foreach (var i in intersections)
		{
			if (intersection.Item3 == 0  || i.Item3 < intersection.Item3)
			{
				intersection = i;
			}
		}
		
		return intersection;
	}

	void LayCable(int x, int y, char v, int id)
	{
		if (!space.TryGetValue(y, out var ySpace))
		{
			ySpace = new Dictionary<int, (char, int)>();
			space.Add(y, ySpace);
		}
		
		if (ySpace.TryGetValue(x, out var c))
		{
			v = c.Item2 == id ? '+' : 'X';
			
			if (c.Item2 != id)
				intersections.Add((x, y, Math.Abs(x) + Math.Abs(y)));
		}
		
		space[y][x] = (v, id);
		
		xSpaceBounds.min = Math.Min(xSpaceBounds.min, x);
		xSpaceBounds.max = Math.Max(xSpaceBounds.max, x);
		ySpaceBounds.max = Math.Max(ySpaceBounds.max, y);
		ySpaceBounds.min = Math.Min(ySpaceBounds.min, y);
	}

	public void LayCable(string[] wire)
	{
		var currPos = new[] { 0, 0 };
		var cableId = currentCable++;
		
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
				
				LayCable(currPos[0], currPos[1], c, cableId);
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
					c = box.Item1;
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
