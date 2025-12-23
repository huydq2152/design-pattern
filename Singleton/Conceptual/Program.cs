using Conceptual;

static void TestSingleton(string value)
{
    var singleton = Singleton.GetInstance(value);
    singleton.PrintSingletonValue();
}

var process1 = new Thread(() => TestSingleton("process1"));
var process2 = new Thread(() => TestSingleton("process2"));

process1.Start();
process2.Start();

process1.Join();
process2.Join();

// The output will always be "process1" or "process2" depending on which thread executed first,
// demonstrating that only one instance of the singleton is created.