namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
        }

        public static async Task DoTreeAsync<T>(Tree<T> tree, Action<T> action)
        {
            if (tree == null) return;

            var leftTask = DoTreeAsync(tree.Left, action);
            var rightTask = DoTreeAsync(tree.Right, action);

            action(tree.Data);

            await Task.WhenAll(leftTask, rightTask);
        }

        public class Tree<T>
        {
            public Tree<T> Left;
            public Tree<T> Right;
            public T Data;
        }

    }
}
