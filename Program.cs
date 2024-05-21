// See https://aka.ms/new-console-template for more information

public class Program {

	public static void Main(string[] args) {
		Game game = new();
		game.TickRate = 50;
		game.Run();

	}
}

public enum Direction {
	up = 0,
	right = 1,
	down = 2,
	left = 3
}

public class Game {
	public int BufferWidth { get; set; }
	public int BufferHeight { get; set; }
	public int TickRate { get; set; }
	public DotSnake Snake { get; set; }
	public DotFood Food { get; set; }
	private bool running { get; set; }

	public Game() {
		BufferWidth = Console.BufferWidth;
		BufferHeight = Console.BufferHeight;
		TickRate = 100;
		Snake = new();
		Food = new();
	}

	public int Run() {
		running = true;
		Console.CursorVisible = false;
		BufferWidth = Console.BufferWidth;
		BufferHeight = Console.BufferHeight;
		Random random = new();
		Food.x = random.Next(0, BufferWidth-1);
		Food.y = random.Next(0, BufferHeight-1);
		Console.Clear();
		Food.Draw();

		while (running) {
			var task = Task.Delay(TickRate);
			if (EatingFood()) {
				Snake.Grow();	
				random = new();
				Food.x = random.Next(1, BufferWidth-2);
				Food.y = random.Next(1, BufferHeight-2);
				Food.Draw();
			}
			Snake.Next();
			if (!InBounds() || EatingSelf()) {
				return Snake.Length;
			}
			Snake.Draw();
			while (!task.IsCompleted) {
				KeyLoop();
			}
		}
		return 0;	
	}

	public void KeyLoop() {
		while(running && Console.KeyAvailable) {
			var key = Console.ReadKey();
			switch (key.Key) {
				case ConsoleKey.UpArrow:
					if (Snake.LastDirection != Direction.down) Snake.Facing = Direction.up;
					break;
				case ConsoleKey.DownArrow:
					if (Snake.LastDirection != Direction.up) Snake.Facing = Direction.down;
					break;
				case ConsoleKey.LeftArrow:
					if (Snake.LastDirection != Direction.right) Snake.Facing = Direction.left;
					break;
				case ConsoleKey.RightArrow:
					if (Snake.LastDirection != Direction.left) Snake.Facing = Direction.right;
					break;
				default: 
					break;
			}
		}
	}

	public bool InBounds() {
		if (Snake.x < 0) 						  return false;
		if (Snake.y < 0) 						  return false;
		if (Snake.x >= BufferWidth) 	return false;
		if (Snake.y >= BufferHeight)  return false;
		return true;
	}

	public bool EatingSelf() {
		if (Snake.Tail.Contains(new(Snake.x, Snake.y)))
			return true;
		return false;
	}

	public bool EatingFood() {
		if (Snake.x == Food.x && Snake.y == Food.y)
			return true;
		return false;
	}

}

public class DotSnake {
	public int x { get; set; }
	public int y { get; set; }
	public int Length { get; set; }
	public bool AddOnNext { get; set; }
	public Direction Facing { get; set; }
	public Direction LastDirection { get; set; }
	public List<Tuple<int, int>> Tail { get; set; }
	public Tuple<int, int> RemovedTail { get; set; }

	public DotSnake() {
		Tail = new();
		Tail.Add(new(5, 4));
		x = 5; y = 5;
		Length = 1;
		AddOnNext = false;
		Facing = Direction.down;
		LastDirection = Direction.down;
		RemovedTail = new(5,4);
	}

	public bool Next() {

		Tail.Insert(0, new(x, y));

		switch (Facing) {
			case Direction.up: 		
				y--; 	
				break;
			case Direction.down: 	
				y++; 	
				break;
			case Direction.left: 	
				x--; 	
				break;
			case Direction.right: 
				x++; 	
				break;
			default: 										
				break;
		}
		LastDirection = Facing;

		if (!AddOnNext) {
			RemovedTail = Tail[Length];
			Tail.RemoveAt(Length);
		}
		else {
			RemovedTail = new(0,0);
			AddOnNext = false;
		}

		return true;
	}

	public void Grow() {
		AddOnNext = true;
		Length++;
	}

	public void Draw() {
		Console.SetCursorPosition(x, y);
		Console.Write('8');
		foreach (var position in Tail)
		{
			Console.SetCursorPosition(position.Item1, position.Item2);
			Console.Write('8');
		}
		Console.SetCursorPosition(RemovedTail.Item1, RemovedTail.Item2);
		Console.Write(' ');
	}
}

public class DotFood {
	public int x { get; set; }
	public int y { get; set; }
	
	public DotFood() {
	}

	public void Draw() {
		Console.SetCursorPosition(x, y);
		Console.Write('f');
	}
}


