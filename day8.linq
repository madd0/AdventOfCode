<Query Kind="Program" />

void Main()
{
	var dir = Path.GetDirectoryName(Util.CurrentQueryPath);
	var file = Path.Combine(dir, "day8.txt");
	
	TextReader reader = new StringReader("123456789012"); // ;new StreamReader(File.OpenRead(file))
	
	var image = Image.Parse(3, 2, reader);
	
	image.Dump();
	
	var stats = this.MakeLayerStats(image);
	
	stats.Dump();

	reader = new StreamReader(File.OpenRead(file));

	image = Image.Parse(25, 6, reader);

	stats = this.MakeLayerStats(image);

	byte[] smallestLayer = null;
	
	foreach (var layer in stats)
	{
		if (smallestLayer == null || layer[0] < smallestLayer[0])
		{
			smallestLayer = layer;
		}
	}
	
	(smallestLayer[1] * smallestLayer[2]).Dump();
	
	image.Render().Dump();
}

byte[][] MakeLayerStats(Image image)
{
	var stats = new byte[image.Layers.Length][];
	
	for (int i = 0; i < stats.Length; i++)
	{
		stats[i] = new byte[10];
	}
	
	for (int layer = 0; layer < image.Layers.Length; layer++)
	{
		foreach (var element in image.Layers[layer])
		{
			stats[layer][element]++;
		}
	}
	
	return stats;
}

class Image
{
	public byte[][] Layers { get; private set; }
	
	public int Width { get; private set; }
	
	public int Height { get; private set; }
	
	private Image(int width, int height, byte[] data)
	{
		this.Width = width;
		this.Height = height;
		
		if (data.Any(d => d < 0 || d > 9))
		{
			throw new ArgumentException("Invalid image data");
		}
		
		this.MakeLayers(data);
	}
	
	public static Image Parse(int width, int height, TextReader data)
	{
		var byteData = new List<byte>();
		var buffer = new char[width * height];
		
		while (data.ReadBlock(buffer, 0, width * height) > 0)
		{
			byteData.AddRange(buffer.Select(c => (byte)(c - '0')));
		}
		
		return new Image(width, height, byteData.ToArray());
	}
	
	public string Render()
	{
		var white = '\u25af';
		var black = '\u25ae';
				
		var builder = new StringBuilder();
		
		for (int pixel = 0; pixel < this.Layers[0].Length; pixel++)
		{
			var color = 2;
			
			for (int layer = this.Layers.Length - 1; layer > 0; layer--)
			{
				if (this.Layers[layer][pixel] != 2)
				{
					color = this.Layers[layer][pixel];
				}
			}
			
			if (color == 0)
			{
				builder.Append(white);
			}
			else
			{
				builder.Append(black);
			}
			
			if ((pixel + 1) % this.Width == 0)
			{
				builder.AppendLine();
			}
		}
		
		return builder.ToString();
	}

	private void MakeLayers(byte[] data)
	{
		var layerSize = this.Width * this.Height;
		
		
		this.Layers = new byte[data.Length / layerSize][];

		for (var i = 0; i < this.Layers.Length; i++)
		{
			this.Layers[i] = new byte[layerSize];
		}
		
		for (int i = 0, layer = 0; i < data.Length; i += layerSize, layer++)
		{
			Array.Copy(data, i, this.Layers[layer], 0, layerSize);
		}
	}
}

// Define other methods and classes here
