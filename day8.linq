<Query Kind="Program" />

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, "day8.txt");
	
	TextReader reader = new StringReader("123456789012");
	
	var image = Image.Parse(3, 2, reader);
	
	image.Dump();
}

class Image
{
	public char[][] Layers { get; private set; }
	
	public int Width { get; private set; }
	
	public int Height { get; private set; }
	
	private Image(int width, int height, char[] data)
	{
		this.Width = width;
		this.Height = height;
		
		this.MakeLayers(data);
	}
	
	public static Image Parse(int width, int height, TextReader data)
	{
		var byteData = new List<char>();
		var buffer = new char[width * height];
		
		while (data.ReadBlock(buffer, 0, width * height) > 0)
		{
			byteData.AddRange(buffer);
		}
		
		return new Image(width, height, byteData.ToArray());
	}

	private void MakeLayers(char[] data)
	{
		var layerSize = this.Width * this.Height;
		
		
		this.Layers = new char[data.Length / layerSize][];

		for (var i = 0; i < this.Layers.Length; i++)
		{
			this.Layers[i] = new char[layerSize];
		}
		
		for (int i = 0, layer = 0; i < data.Length; i += layerSize, layer++)
		{
			Array.Copy(data, i, this.Layers[layer], 0, layerSize);
		}
	}
}

// Define other methods and classes here
