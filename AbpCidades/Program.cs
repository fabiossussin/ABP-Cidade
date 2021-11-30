using System;
using System.Collections.Generic;
using System.Linq;

namespace AbpCidades
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Bem Vindo!");
            var loop = true;

            var tree = new Tree();
            while (loop)
            {
                Console.WriteLine("");
                Console.WriteLine("Ações diponíveis!");
                Console.WriteLine("1) Adicionar Cidade");
                Console.WriteLine("2) Remover Cidade");
                Console.WriteLine("3) Consultar Cidade");
                Console.WriteLine("4) Listar Cidades Crescente");
                Console.WriteLine("5) Listar Cidades Decrescente");
                Console.WriteLine("6) Listar Estados Crescente");
                Console.WriteLine("7) Listar Estados Decrescente");
                Console.WriteLine("8) Listar Cidades por Populção Crescente");
                Console.WriteLine("9) Listar Cidades por Populção Decrescente");
                Console.WriteLine("10) Listar Cidades do Estado");
                Console.WriteLine("11) Cidade Mais Populosa");
                Console.WriteLine("12) Sair");
                Console.WriteLine("");

                var actionStr = Console.ReadLine();
                if (!int.TryParse(actionStr, out int result))
                    Console.WriteLine("Ação indiponível, digite um número de 1 a 12");
                else
                    switch (result)
                    {
                        case 1:
                            RegisterCity(tree);
                            break;
                        case 2:
                            RemoveCity(tree);
                            break;
                        case 3:
                            GetCity(tree);
                            break;
                        case 4:
                            DataList(tree, Data.City, DataOrder.Default);
                            break;
                        case 5:
                            DataList(tree, Data.City, DataOrder.Reverse);
                            break;
                        case 6:
                            DataList(tree, Data.State, DataOrder.Default);
                            break;
                        case 7:
                            DataList(tree, Data.State, DataOrder.Reverse);
                            break;
                        case 8:
                            PopulationList(tree, DataOrder.Default);
                            break;
                        case 9:
                            PopulationList(tree, DataOrder.Reverse);
                            break;
                        case 10:
                            CitiesList(tree);
                            break;
                        case 11:
                            MostPopularCity(tree);
                            break;
                        case 12:
                        default:
                            loop = false;
                            break;

                    }
            }

            Console.WriteLine("");
            Console.WriteLine("Volte Sempre!");
        }

        public static void RegisterCity(Tree tree)
        {
            Console.Write("Cidade: ");
            var cityName = Console.ReadLine();
            Console.Write("Estado: ");
            var state = Console.ReadLine();
            Console.Write("Latitude: ");
            var lat = Console.ReadLine();
            Console.Write("Longitude: ");
            var lon = Console.ReadLine();
            Console.Write("População: ");
            var populationStr = Console.ReadLine();

            if (!long.TryParse(populationStr, out long population))
            {
                Console.WriteLine("Populção inválida informada");
                return;
            }

            var city = new City(cityName, state, lat, lon, population);
            if (tree.Root == null)
                tree.Root = new Node(city);
            else
                tree.Insert(city);
        }

        public static void RemoveCity(Tree tree)
        {
            Console.Write("Nome da Cidade a ser removida: ");
            tree.DeleteCity(tree.Root, Console.ReadLine());
        }

        public static void GetCity(Tree tree)
        {
            Console.Write("Nome da Cidade: ");
            Console.WriteLine(tree.GetCityData(Console.ReadLine(), tree.Root));
        }

        public static void DataList(Tree tree, Data data, DataOrder order)
        {
            var result = tree.GetData(order, data);
            Console.WriteLine(string.IsNullOrEmpty(result) ? "Nenhuma cidade cadastrada" : result);
        }

        public static void PopulationList(Tree tree, DataOrder order)
        {
            var result = tree.GetPopulation(order);
            Console.WriteLine(string.IsNullOrEmpty(result) ? "Nenhuma cidade cadastrada" : result);
        }

        public static void CitiesList(Tree tree)
        {
            Console.Write("Estado das Cidades: ");
            var result = tree.GetCities(Console.ReadLine());
            Console.WriteLine(string.IsNullOrEmpty(result) ? "Nenhuma cidade cadastrada" : result);
        }

        public static void MostPopularCity(Tree tree)
        {
            var result = tree.MostPopularCity();
            Console.WriteLine(string.IsNullOrEmpty(result) ? "Nenhuma cidade cadastrada" : result);
        }


        public class City
        {
            public City(string name, string state, string lat, string lon, long population)
            {
                Name = name;
                State = state;
                Latitude = lat;
                Longitude = lon;
                Population = population;
            }

            public string Name { get; set; }
            public string State { get; set; }
            public long Population { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }

            public string CityData() => $"Nome: {Name}. Estado: {State}. População: {Population}. Latitude: {Latitude}. Longitude: {Longitude}.";
        }

        public class Node
        {
            public Node(City value)
            {
                Value = value;
            }

            public City Value { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
        }

        public class Tree
        {
            public Node Root;
            public void Insert(City value) => Insert(null, value);

            public Tree() { }
            public Tree(City value) => Root = new Node(value);

            public Node DeleteCity(Node root, string key)
            {
                if (root == null)
                    return null;

                if (string.Compare(root.Value.Name, key) < 0)
                    root.Left = DeleteCity(root.Left, key);
                else if (string.Compare(root.Value.Name, key) > 0)
                    root.Right = DeleteCity(root.Right, key);
                else
                {
                    if (root.Left == null && root.Right == null)
                        return null;
                    else if (root.Left == null)
                        root = root.Right;
                    else if (root.Right == null)
                        root = root.Left;
                    else
                    {
                        Node min = MinValue(root.Right);
                        root.Value = min.Value;
                        root.Right = DeleteCity(root.Right, min.Value.Name);
                    }
                }
                return root;
            }

            public string GetCityData(string element, Node root) =>
                root == null ? "Cidade não encontrada" : element == root.Value.Name ?
                root.Value.CityData() : GetCityData(element, string.Compare(root.Value.Name, element) < 0 ? root.Left : root.Right);

            public string GetData(DataOrder order, Data data)
            {
                var result = new List<string>();
                result.AddRange(GetData(Root, data));
                return string.Join(',', order == DataOrder.Default ? result.OrderBy(x => x) : result.OrderByDescending(x => x));
            }

            public string GetPopulation(DataOrder order)
            {
                var cities = new List<City>();
                cities.AddRange(GetCity(Root));

                return string.Join(", ", order == DataOrder.Default ? cities.OrderBy(x => x.Population).Select(x => $"{x.Name} população: {x.Population}") : cities.OrderByDescending(x => x.Population).Select(x => $"{x.Name} população: {x.Population}"));
            }

            public string GetCities(string data)
            {
                var cities = new List<City>();
                cities.AddRange(GetCity(Root));

                return string.Join(',', cities.Where(x => x.State == data).Select(x => x.Name));
            }

            public string MostPopularCity()
            {
                var cities = new List<City>();
                cities.AddRange(GetCity(Root));

                return "Cidade mais populosa: " + cities.OrderByDescending(x => x.Population).FirstOrDefault()?.Name;
            }

            private List<string> GetData(Node node, Data dataEnum)
            {
                var result = new List<string>();
                if (node == null)
                    return result;

                var data = node;
                result.Add(dataEnum == Data.City ? data.Value.Name : data.Value.State);
                if (data.Left != null)
                    result.AddRange(GetData(data.Left, dataEnum));
                if (data.Right != null)
                    result.AddRange(GetData(data.Right, dataEnum));

                return result;
            }

            private List<City> GetCity(Node node)
            {
                var result = new List<City>();
                if (node == null)
                    return result;
                
                var data = node;
                result.Add(data.Value);
                if (data.Left != null)
                    result.AddRange(GetCity(data.Left));
                if (data.Right != null)
                    result.AddRange(GetCity(data.Right));

                return result;
            }

            private static Node MinValue(Node root)
            {
                var minv = root;
                while (root.Left != null)
                {
                    minv = root.Left;
                    root = root.Left;
                }
                return minv;
            }

            private void Insert(Node node, City value)
            {
                if (node == null)
                    node = Root;

                if (string.Compare(node.Value.Name, value.Name) < 0)
                {
                    if (node.Left == null)
                        node.Left = new Node(value);
                    else
                        Insert(node.Left, value);
                }
                else
                {
                    if (node.Right == null)
                        node.Right = new Node(value);
                    else
                        Insert(node.Right, value);
                }
            }

        }

        public enum DataOrder
        {
            Default,
            Reverse
        }

        public enum Side
        {
            Left,
            Right
        }

        public enum Data
        {
            City,
            State
        }
    }
}