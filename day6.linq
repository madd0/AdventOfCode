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
K)L
K)YOU
I)SAN";
	
	TextReader reader = new StringReader(graph);
	
	var map = Parse(reader).Dump();
	CountOrbits(map).Dump();
	FindSanta(map).Dump();

	reader = new StreamReader(File.OpenRead(Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "day6.txt")));
	
	map = Parse(reader);
	CountOrbits(map).Dump();
	FindSanta(map).Dump();
}

int FindSanta(Dictionary<string, string> map)
{
	var youMap = Map(map, "YOU");
	var santaMap = Map(map, "SAN");
	
	string youParent, santaParent;
	
	do
	{
		youParent = youMap.Pop();
		santaParent = santaMap.Pop();
	}
	while (youParent == santaParent);
	
	youMap.Push(youParent);
	santaMap.Push(santaParent);
		
	return youMap.Count + santaMap.Count;
}

Stack<string> Map(Dictionary<string, string> map, string body)
{
	var path = new Stack<string>();
		
	string parent = body;
	
	while ((parent = map[parent]) != null)
	{
		path.Push(parent);
	}
	
	return path;
}

int CountOrbits(Dictionary<string, string> map)
{
	var orbits = 0;
	
	foreach (var element in map)
	{
		orbits += IndirectCount(map, element.Value);
	}
	
	return orbits;
}

int IndirectCount(Dictionary<string, string> map, string value)
{
	return value == null ? 0 : 1 + IndirectCount(map, map[value]);
}

Dictionary<string, string> Parse(TextReader graph)
{
	var map = new Dictionary<string, string>
	{
		["COM"] = null
	};
	
	string line;
	
	while ((line = graph.ReadLine()) != null)
	{
		var bodies = line.Split(')');
		
		map[bodies[1]] = bodies[0];
	}
	
	return map;
}
