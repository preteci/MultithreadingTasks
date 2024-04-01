namespace Task1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // playing the whole club
            NightClub club = new NightClub();
            club.Play();
        }
    }

    public class NightClub
    {
        static string[] musicTypes = { "Hardbass", "Latino", "Rock" };
        static string[] danceMoves = { "Elbow dance", "Hips dance", "Head dance" };
        int songTracker = 0;
        int[] trackList = GenerateRandomArray(1, 0, 2);

        public void Play()
        {
            
            //creation of dancer threads
            Thread[] dancers = new Thread[10];

            for(int i = 0; i < dancers.Length; i++)
            {
                dancers[i] = new Thread(() => Dance());
                dancers[i].Start();
            }

            for (int i = 0; i < trackList.Length; i++)
            {

                Console.WriteLine($"Playing: {musicTypes[trackList[songTracker]]}");
                Thread.Sleep(3000);
                songTracker++;
            }

            Console.WriteLine("All tracks played. Dancer will begin to leave now...");

            for (int i = 0; i < dancers.Length; i++)
            {
                dancers[i].Join();
            }

            Console.WriteLine("All dancers have left...");

        }

        // method for generating TrackList
        static int[] GenerateRandomArray(int lenght, int min, int max)
        {
            int[] array = new int[lenght];
            Random random = new Random();
            for (int i = 0; i < lenght; i++)
            {
                array[i] = random.Next(min, max + 1);
            }
            return array;
        }

        // method for dancing
        private void Dance()
        {
            while(true)
            {
                // check if we are done with the tracks
                if(songTracker > trackList.Length - 1)
                {
                    break;
                }

                // get the track so we can perform a dance
                string currentTrack = musicTypes[trackList[songTracker]];
                switch (currentTrack)
                {
                    case "Hardbass":
                        Console.WriteLine("Elbow dance!");
                        break;
                    case "Latino":
                        Console.WriteLine("Hips shake!");
                        break;
                    case "Rock":
                        Console.WriteLine("Headbanging!");
                        break;
                }
                // simulating dancing of a thread
                Thread.Sleep(2000);
            }
        }

    }

}
