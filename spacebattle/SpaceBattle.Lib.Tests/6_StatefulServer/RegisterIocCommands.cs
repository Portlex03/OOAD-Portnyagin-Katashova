namespace SpaceBattle.Lib.Tests;

using System.Collections.Concurrent;
using Hwdtech;

using QueueDict = Dictionary<int, System.Collections.Concurrent.BlockingCollection<Hwdtech.ICommand>>;
using ThreadDict = Dictionary<int, ServerThread>;


