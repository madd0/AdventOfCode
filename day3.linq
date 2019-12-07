<Query Kind="Program" />

void Main()
{
	var space = new Space();
	
	
	var wire1 = "U7,R6,D4,L4".Split(',');
	
	space.LayCable(wire1);
	
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
		if (!space.TryGetValue(y, out var xSpace))
		{
			xSpace = new Dictionary<int, char>();
			space.Add(y, xSpace);
		}
		
		if (!xSpace.TryGetValue(y, out var c))
		{
			xSpace.Add(x, 'X');
		}
		
		space[y][x] = v;
	}

	public void LayCable(string[] wire)
	{
		var currPos = (x: 0, y: 0);
		
		foreach (var element in wire)
		{
			var direction = element[0];
			var amount = int.Parse(element.Substring(1));
		}
	}

	public void Draw()
	{
		var drawing = new StringBuilder();
		
		for (var row = ySpaceBounds.max; row >= ySpaceBounds.min; row--)
		{
			for (var col = xSpaceBounds.min; col <= xSpaceBounds.max; col++)
			{
				drawing.Append(space[row][col]);
			}
			
			drawing.AppendLine();
		}
		
		drawing.ToString().Dump();
	}
}
