<Query Kind="Program" />

void Main()
{
	var space = new Space();
	
	
	var wire1 = "U7,R6,D4,L4".Split(',');
	var wire2 = "R8,U5,L5,D3".Split(',');

	space.LayCable(wire1);

	space.Draw();
	space.LayCable(wire2);

	space.Draw();
}

class Space
{
	Dictionary<int, Dictionary<int, char>> space = new Dictionary<int, Dictionary<int, char>>();

	(int min, int max) xSpaceBounds = (0, 0);
	(int min, int max) ySpaceBounds = (0, 0);
	
	public Space()
	{
		LayCable(0, 0, 'o');
	}

	void LayCable(int x, int y, char v)
	{
		if (!space.TryGetValue(y, out var ySpace))
		{
			ySpace = new Dictionary<int, char>();
			space.Add(y, ySpace);
		}
		
		if (ySpace.TryGetValue(x, out var c))
		{
			v = 'X';
		}
		
		space[y][x] = v;
		
		xSpaceBounds.min = Math.Min(xSpaceBounds.min, x);
		xSpaceBounds.max = Math.Max(xSpaceBounds.max, x);
		ySpaceBounds.max = Math.Max(ySpaceBounds.max, y);
		ySpaceBounds.min = Math.Min(ySpaceBounds.min, y);
	}

	public void LayCable(string[] wire)
	{
		var currPos = new[] { 0, 0 };
		
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
				
				LayCable(currPos[0], currPos[1], c);
			}
		}
	}

	public void Draw()
	{
		var drawing = new StringBuilder();
		
		for (var row = ySpaceBounds.max + 1; row >= ySpaceBounds.min - 1; row--)
		{
			for (var col = xSpaceBounds.min - 1; col <= xSpaceBounds.max + 1; col++)
			{
				var c = '.';
				
				if (space.TryGetValue(row, out var xSpace) &&
				    !xSpace.TryGetValue(col, out c))
				{
					c = '.';
				}
				drawing.Append(c);
			}
			
			drawing.AppendLine();
		}

		drawing.ToString().Dump();
	}
}
