using System;
using System.Collections.Generic;
using System.Linq;

namespace AbpCidades
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new Tree(new City("Caxias", "RS", "", "", 0));
            tree.Insert(new City("Recife", "PE", "", "", 0));
            tree.Insert(new City("Bage", "RS", "", "", 0));
            tree.Insert(new City("Florianopolis", "SC", "", "", 0));
            tree.Insert(new City("São Paulo", "SP", "", "", 0));
            var t = tree;
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
            public readonly Node Root;
            public Tree(City value) => Root = new Node(value);
            public void Insert(City value) => Insert(null, value);

            public Node DeleteRec(Node root, string key)
            {
                if (root == null)
                    return root;

                if (string.Compare(root.Value.Name, key) < 0)
                    root.Left = DeleteRec(root.Left, key);
                else if (string.Compare(root.Value.Name, key) > 0)
                    root.Right = DeleteRec(root.Right, key);
                else
                {
                    if (root.Left == null)
                        return root.Right;
                    else if (root.Right == null)
                        return root.Left;

                    root.Value.Name = MinValue(root.Right);
                    root.Right = DeleteRec(root.Right, root.Value.Name);
                }
                return root;
            }

            public string GetCityData(string element, Node root) =>
                root == null ? "Cidade não encontrada" : element == root.Value.Name ?
                root.Value.CityData() : GetCityData(element, string.Compare(root.Value.Name, element) < 0 ? root.Left : root.Right);

            public string MostPopularCity(Node root) => root.Right == null ? "Cidade mais populosa: " + root.Value.Name : MostPopularCity(root.Right);

            public string GetCities(DataOrder order)
            {
                var cities = new List<string>();
                cities.AddRange(GetData(Side.Left, Data.City));
                cities.AddRange(GetData(Side.Right, Data.City));

                return string.Join(',', order == DataOrder.Default ? cities.OrderBy(x => x) : cities.OrderByDescending(x => x));
            }

            public string GetStates(DataOrder order)
            {
                var states = new List<string>();
                states.AddRange(GetData(Side.Left, Data.State));
                states.AddRange(GetData(Side.Right, Data.State));

                return string.Join(',', order == DataOrder.Default ? states.OrderBy(x => x) : states.OrderByDescending(x => x));
            }

            public string GetPopulation(DataOrder order)
            {
                var cities = new List<City>();
                cities.AddRange(GetCity(Side.Left));
                cities.AddRange(GetCity(Side.Right));

                return string.Join(',', order == DataOrder.Default ? cities.OrderBy(x => x.Population).Select(x => x.Name) : cities.OrderByDescending(x => x.Population).Select(x => x.Name));
            }

            public string GetCities(string data)
            {
                var cities = new List<City>();
                cities.AddRange(GetCity(Side.Left));
                cities.AddRange(GetCity(Side.Right));

                return string.Join(',', cities.Where(x=> x.State == data));
            }

            private List<string> GetData(Side order, Data dataEnum)
            {
                var result = new List<string>();
                var data = Root;
                while (data?.Value != null)
                {
                    result.Add(dataEnum == Data.City ? data.Value.Name : data.Value.State);
                    data = order == Side.Right ? data.Right : data.Left;
                }

                return result;
            }

            private List<City> GetCity(Side order)
            {
                var result = new List<City>();
                var data = Root;
                while (data?.Value != null)
                {
                    result.Add(data.Value);
                    data = order == Side.Right ? data.Right : data.Left;
                }

                return result;
            }

            private static string MinValue(Node root)
            {
                string minv = root.Value.Name;
                while (root.Left != null)
                {
                    minv = root.Left.Value.Name;
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