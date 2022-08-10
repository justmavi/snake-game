using SnakeGame;
using System.Timers;
using Timer = System.Timers.Timer;

const int PlaygroundSquare = 25;
const int SnakeInitialLength = 3;

const int SnakeNodeNumber = 1;
const int EatNumber = -1;
const int EmptyCellNumber = 0;

var playgroundMatrix = new int[PlaygroundSquare, PlaygroundSquare];
var snakeNodes = new LinkedList<SnakeNode>();
var snakeDirection = SnakeDirection.Right;
var gameTimer = new Timer()
{
    Interval = 200,
    AutoReset = true
};
gameTimer.Elapsed += OnGameTimerElapsed;

for (int i = 0; i < SnakeInitialLength; i++)
{
    snakeNodes.AddLast(new SnakeNode(0, SnakeInitialLength - i - 1));
    playgroundMatrix[0, SnakeInitialLength - i - 1] = SnakeNodeNumber;
}
SetEat();
gameTimer.Start();


while (true)
{
    var keyInfo = Console.ReadKey();
    
    switch (keyInfo.Key)
    {
        case ConsoleKey.UpArrow:
            if (snakeDirection != SnakeDirection.Down)
            {
                snakeDirection = SnakeDirection.Up;
            }

            break;
        case ConsoleKey.DownArrow:
            if (snakeDirection != SnakeDirection.Up)
            {
                snakeDirection = SnakeDirection.Down;
            }

            break;
        case ConsoleKey.LeftArrow:
            if (snakeDirection != SnakeDirection.Right)
            {
                snakeDirection = SnakeDirection.Left;
            }

            break;
        case ConsoleKey.RightArrow:
            if (snakeDirection != SnakeDirection.Left)
            {
                snakeDirection = SnakeDirection.Right;
            }

            break;
    }
}


void SetEat()
{
    int? rndX, rndY;
    var random = new Random();

    do
    {
        rndX = random.Next(0, PlaygroundSquare);
        rndY = random.Next(0, PlaygroundSquare);

        if (playgroundMatrix[rndX.Value, rndY.Value] != EmptyCellNumber)
        {
            rndX = rndY = null;
        }

    } while (rndX == null || rndY == null);

    playgroundMatrix[rndX.Value, rndY.Value] = EatNumber;
}


void OnGameTimerElapsed(object? sender, ElapsedEventArgs args)
{
    var headNode = snakeNodes.First;
    var head = headNode?.Value;
    LinkedListNode<SnakeNode>? currentNode = null;
    int oldNodeX, oldNodeY;
    int temp; 

    if (head is not null)
    {
        int newX, newY;

        switch (snakeDirection)
        {
            case SnakeDirection.Down:
                newY = head.Y;
                newX = head.X + 1;
                break;
            case SnakeDirection.Up:
                newY = head.Y;
                newX = head.X - 1;
                break;
            case SnakeDirection.Left:
                newY = head.Y - 1;
                newX = head.X;
                break;
            case SnakeDirection.Right:
                newY = head.Y + 1;
                newX = head.X;
                break;
            default:
                throw new Exception("Unable to understand snake direction");
        }

        if (newX >= PlaygroundSquare || newY >= PlaygroundSquare || newX < 0 || newY < 0 || playgroundMatrix[newX, newY] == SnakeNodeNumber)
        {
            Console.WriteLine($"\nGame over. Score: {snakeNodes.Count - SnakeInitialLength}.");
            gameTimer.Stop();

            return;
        }

        int? lastNodeX = null, lastNodeY = null;
        if (playgroundMatrix[newX, newY] == EatNumber)
        {
            lastNodeX = snakeNodes.Last?.Value.X;
            lastNodeY = snakeNodes.Last?.Value.Y;
        }

        playgroundMatrix[newX, newY] = SnakeNodeNumber;
        playgroundMatrix[head.X, head.Y] = EmptyCellNumber;

        oldNodeX = head.X;
        oldNodeY = head.Y;

        head.X = newX;
        head.Y = newY;

        currentNode = headNode?.Next;
        while (currentNode is not null)
        {
            playgroundMatrix[oldNodeX, oldNodeY] = SnakeNodeNumber;
            playgroundMatrix[currentNode.Value.X, currentNode.Value.Y] = EmptyCellNumber;

            temp = currentNode.Value.X;
            currentNode.Value.X = oldNodeX;
            oldNodeX = temp;

            temp = currentNode.Value.Y;
            currentNode.Value.Y = oldNodeY;
            oldNodeY = temp;

            currentNode = currentNode.Next;
        }

        if (lastNodeX is not null && lastNodeY is not null)
        {
            playgroundMatrix[lastNodeX.Value, lastNodeY.Value] = SnakeNodeNumber;
            snakeNodes.AddLast(new SnakeNode(lastNodeX.Value, lastNodeY.Value));
            SetEat();
        }

        PrintPlayground();
    }
}

void PrintPlayground()
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.White;

    for (int i = 0; i < PlaygroundSquare + 2; i++)
    {
        Console.Write('_');
    }
    Console.WriteLine();

    for (int i = 0; i < PlaygroundSquare; i++)
    {
        Console.Write("|");

        for (int j = 0; j < PlaygroundSquare; j++)
        {
            if (playgroundMatrix[i, j] == SnakeNodeNumber)
            {
                Console.Write('O');
            }
            else if (playgroundMatrix[i, j] == EatNumber)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write('ᴏ');
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.Write(' ');
            }

            if (j == PlaygroundSquare - 1)
            {
                Console.Write('|');
            }
        }

        Console.WriteLine();
    }

    for (int i = 0; i < PlaygroundSquare + 2; i++)
    {
        Console.Write('_');
    }
}