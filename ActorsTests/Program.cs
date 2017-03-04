using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using Octokit;
using Octokit.Internal;

namespace ActorsTests
{
    public class CountCommitsActor : ReceiveActor
    {
        public CountCommitsActor()
        {
            var login = "michal-franc";
            var github = new GitHubClient(new ProductHeaderValue("AkkaCounter"), new InMemoryCredentialStore(new Credentials("<hidden>")));

            this.Receive<string>(x =>
            {
                try
                {
                    var commits = github.Repository.Commit.GetAll(login, x).Result;
                    var authorCommits = commits.Where(c => c.Author.Login == "michal-franc").ToList();
                    this.Sender.Tell(authorCommits.Count);
                }
                catch (Exception)
                {
                    this.Sender.Tell(0);
                }
            });
        }
    }

    public class WritetToConsoleActor : ReceiveActor
    {
        public WritetToConsoleActor()
        {
            this.Receive<string>(x => Console.WriteLine(x));
        }
    }

    public class MainActorSystem : ReceiveActor
    {
        public MainActorSystem()
        {
            IActorRef notifyActor = Context.ActorOf(Props.Create(() => new WritetToConsoleActor()));

            IActorRef comitCounterActors = Context.ActorOf(Props.Create(() => new CountCommitsActor())
                                          .WithRouter(new RoundRobinPool(1)), "CommitCounters");

            var github = new GitHubClient(new ProductHeaderValue("mfranc_workshop"), new InMemoryCredentialStore(new Credentials("<hidden>")));

            var repos = github.Repository.GetAllForUser("michal-franc").Result;

            notifyActor.Tell($"Found {repos.Count} repos.");

            foreach (var repository in repos)
            {
                comitCounterActors.Tell(repository.Name);
            }

            var countedCommits = 0;

            this.Receive<int>(x =>
            {
                countedCommits += x;
                notifyActor.Tell($"Received {x} commits count.\r\n CurrentCount : {countedCommits}");
            });
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("main");

            var githubRepoActor = actorSystem.ActorOf(Props.Create(() => new MainActorSystem()), "MainSystem");

            githubRepoActor.Tell("test");

            Task.WaitAll(actorSystem.WhenTerminated);
        }
    }
}
