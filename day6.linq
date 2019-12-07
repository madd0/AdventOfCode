<Query Kind="Program" />

void Main()
{
	var graph = @"COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L";
	
	var reader = new StringReader(graph);
	
	var map = Parse(reader).Dump();
	map.Sum(b => b.Value.Count).Dump();
}

Dictionary<string, List<string>> Parse(TextReader graph)
{
	var map = new Dictionary<string, List<string>>();
	
	string line;
	
	while ((line = graph.ReadLine()) != null)
	{
		var bodies = line.Split(')');
		
		if (!map.TryGetValue(bodies[0], out var orbitingBodies))
		{
			orbitingBodies = new List<string>();
			map.Add(bodies[0], orbitingBodies);
		}
		
		orbitingBodies.Add(bodies[1]);
	}
	
	return map;
}

// Define other methods and classes here
