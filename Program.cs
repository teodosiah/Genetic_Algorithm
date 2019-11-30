using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    
    public class Program
    {
        private const int TERMINATE_INDEX = 500;
        private const int NUMBER_OF_ITEMS = 24;
        private static int TOTAL_DECIMAL;  //24 x 1
        private const int NUMBER_OF_POPULATION = 100;
        private const double SELECTION_PERCENTAGE = 0.2;
        private const double BETTER_PERCENTAGE = 0.1;
        private const double MUTATION_PERCENTAGE = 0.05;

        public static Dictionary<int, Data> DATA = new Dictionary<int, Data>();

        static void Main(string[] args)
        {
            CreateData(DATA);
            TOTAL_DECIMAL = calc_total_decimal(NUMBER_OF_ITEMS);
            //better part count 
            int Selected_Count = (int)(NUMBER_OF_POPULATION - NUMBER_OF_POPULATION * SELECTION_PERCENTAGE); 
            CompareChromosomes comparator = new CompareChromosomes();
            List<PopulationItem> Population = new List<PopulationItem>();

            genereatePopulation(Population, DATA);

            for(int i = 0; i < TERMINATE_INDEX; i++)
            {
                Population.Sort(comparator);
                //remove last
                Population.RemoveRange(Selected_Count, (int)(NUMBER_OF_POPULATION * SELECTION_PERCENTAGE));
                int ParentsCount = (int)(NUMBER_OF_POPULATION * BETTER_PERCENTAGE);
                for(int j = 0; j < ParentsCount; j++)
                {
                    Random rnd = new Random();
                    int secondParent_index = rnd.Next(ParentsCount + 1, Selected_Count);
                    CrossoverFunction(Population[j].Chrom, Population[secondParent_index].Chrom, Population);
                }
                //after crossover sort again
                Population.Sort(comparator);
                Mutation(Population);
            }
            Population.Sort(comparator);
            print(Population.First().Chrom);           
           
        }
        private static void genereatePopulation(List<PopulationItem> population, Dictionary<int, Data> data)
        {
            Random r = new Random();
            int dec = 0;
            for (int i = 0; i < NUMBER_OF_POPULATION; i++)
            {                
                dec = r.Next(0, TOTAL_DECIMAL);
                PopulationItem pop = new PopulationItem();
                pop.Chrom = new Chromosome(NUMBER_OF_ITEMS,dec, data);
                pop.FitnessValue = calcFitness(pop.Chrom);

                population.Add(pop);
            }            
        }
        //in one point
        private static void CrossoverFunction(Chromosome mother, Chromosome father, List<PopulationItem> population)
        {
            //get point
            Random r = new Random();
            int point = r.Next(1, NUMBER_OF_ITEMS);

            PopulationItem child_1 = new PopulationItem();
            PopulationItem child_2 = new PopulationItem();

            Chromosome ch1 = new Chromosome(NUMBER_OF_ITEMS);
            Chromosome ch2 = new Chromosome(NUMBER_OF_ITEMS);

            for(int i = 0; i < NUMBER_OF_ITEMS; i++)
            {
                if(i <= point)
                {
                    ch1.Genes[i] = mother.Genes[i];
                    ch2.Genes[i] = father.Genes[i];
                }
                else
                {
                    ch1.Genes[i] = father.Genes[i];
                    ch2.Genes[i] = mother.Genes[i];
                }
            }
            ch1.cost_modifier(5000, DATA);
            ch2.cost_modifier(5000, DATA);

            child_1.Chrom = ch1;
            child_1.FitnessValue = calcFitness(ch1);

            child_2.Chrom = ch2;
            child_2.FitnessValue = calcFitness(ch2);

            population.Add(child_1);
            population.Add(child_2);
        }
        public static int calcFitness(Chromosome chromosome)
        {
            int score = 0;
            var gene = chromosome.Genes;
            for (int i = 0; i < gene.Length; i++)
            {
                Data current_Item = new Data();
                if (DATA.TryGetValue(i+1, out current_Item))
                {
                    score += gene[i] * current_Item.Value;
                }
            }
            return score;
        }
        private static void Mutation(List<PopulationItem> population)
        {
            int countOfChromosomesMutants = (int)(NUMBER_OF_POPULATION * MUTATION_PERCENTAGE);
            //get indexes of random chromosomes from the lorst part of the population
            List<int> mutantIndexes = genIndexes(countOfChromosomesMutants, (int)(NUMBER_OF_POPULATION * BETTER_PERCENTAGE), NUMBER_OF_POPULATION);
           
           foreach(int index in mutantIndexes)
            {
                MutationFunction(population[index]);
            }

        }
        //change value of little part of genes
        private static void MutationFunction(PopulationItem item)
        {
            Random r = new Random();
            int bits_count = r.Next(1, (int)(NUMBER_OF_ITEMS/2 - 1));
            List<int> indexes = genIndexes(bits_count, 0, NUMBER_OF_ITEMS);
            foreach(int index in indexes)
            {
                if(item.Chrom.Genes[index] == 0)
                {
                    item.Chrom.Genes[index] = 1;
                }
                else
                {
                    item.Chrom.Genes[index] = 0;
                }
            }
            item.Chrom.cost_modifier(5000, DATA);
            item.FitnessValue = calcFitness(item.Chrom);
        }
        private static void print(Chromosome chromosome)
        {
            for(int i = 0; i < NUMBER_OF_ITEMS; i++)
            {
                if(chromosome.Genes[i] == 1)
                {
                    Data item = new Data();
                    if(DATA.TryGetValue(i+1, out item))
                    {
                        Console.WriteLine(item.Item);
                    }                    
                }
            }
            Console.ReadKey();
        }
        private static int calc_total_decimal(int number_of_1)
        {
            string bin_total = new string('1', number_of_1);
            return Convert.ToInt32(bin_total, 2);
        }
        private static List<int> genIndexes(int countOfElements, int begin, int end)
        {
            List<int> list = new List<int>();
            Random r = new Random();
            while (list.Count != countOfElements)
            {
                int index = r.Next(begin, end);
                if (! list.Contains(index))
                {
                    list.Add(index);
                }
            }
            return list;
        }
        private static void CreateData(Dictionary<int, Data> dictionary)
        {
            int i = 1;

            dictionary.Add(i++, new Data() { Item = "map", Cost = 100, Value = 150 });
            dictionary.Add(i++, new Data() { Item = "compass", Cost = 120, Value = 40 });
            dictionary.Add(i++, new Data() { Item = "Magic water", Cost = 1600, Value = 200 });
            dictionary.Add(i++, new Data() { Item = "Magic boots", Cost = 700, Value = 160 });
            dictionary.Add(i++, new Data() { Item = "Magic Bow", Cost = 150, Value = 60 });
            dictionary.Add(i++, new Data() { Item = "Flying carpet", Cost = 680, Value = 45 });
            dictionary.Add(i++, new Data() { Item = "Magic Ring", Cost = 270, Value = 60 });
            dictionary.Add(i++, new Data() { Item = "Magic Glasses", Cost = 385, Value = 48 });
            dictionary.Add(i++, new Data() { Item = "Life elixir", Cost = 230, Value = 30 });
            dictionary.Add(i++, new Data() { Item = "Magic animal", Cost = 520, Value = 10 });
            dictionary.Add(i++, new Data() { Item = "Sword", Cost = 1700, Value = 400 });
            dictionary.Add(i++, new Data() { Item = "Shield", Cost = 500, Value = 300 });
            dictionary.Add(i++, new Data() { Item = "Teleport", Cost = 240, Value = 15 });
            dictionary.Add(i++, new Data() { Item = "Magic berries", Cost = 480, Value = 10 });
            dictionary.Add(i++, new Data() { Item = "Magic umbrella", Cost = 730, Value = 40 });
            dictionary.Add(i++, new Data() { Item = "flower", Cost = 420, Value = 70 });
            dictionary.Add(i++, new Data() { Item = "Knife", Cost = 430, Value = 75 });
            dictionary.Add(i++, new Data() { Item = "Tent", Cost = 220, Value = 80 });
            dictionary.Add(i++, new Data() { Item = "Garlic", Cost = 70, Value = 20 });
            dictionary.Add(i++, new Data() { Item = "Silver elixir", Cost = 180, Value = 12 });
            dictionary.Add(i++, new Data() { Item = "Magic Hat", Cost = 40, Value = 50 });
            dictionary.Add(i++, new Data() { Item = "magic beer", Cost = 300, Value = 10 });
            dictionary.Add(i++, new Data() { Item = "Silver Arrows", Cost = 900, Value = 20 });
            dictionary.Add(i++, new Data() { Item = "Mystic Orb", Cost = 2000, Value = 150 });
        }
    }
    public class PopulationItem
    {
        public Chromosome Chrom { get; set; }
        public int FitnessValue { get; set; }
    }
    public class Chromosome
    {
        private int[] _genes;
        public int[] Genes
        {
            get
            {
                return _genes;
            }
            set
            {
                _genes = value;
            }
        }
       public Chromosome(int length)
        {
            _genes = new int[length];
        } 
       public Chromosome(int length, int dec, Dictionary<int, Data> data)
        {
            string binary = Convert.ToString(dec, 2);
            _genes = new int[length];                  
                        
            for (int j = 0; j < binary.Length; j++)
            {                
                _genes[j] = (int)binary[j] - (int)'0';                
            }
            cost_modifier(5000, data);
        }
        public void cost_modifier(int less_than, Dictionary<int, Data> data)
        {
            double current_cost = 0;
            int break_index = 0;
            for(int i = 0; i < _genes.Length; i++)
            {
                Data item = new Data();
                if(data.TryGetValue(i+1, out item))
                {
                    current_cost += _genes[i] * item.Cost;
                }
                if(current_cost > less_than)
                {
                    break_index = i;
                    break;
                }
            }
            if (break_index == 0)
                return;

            for(int i = break_index; i < _genes.Length; i++)
            {
                _genes[i] = 0;
            }
        }
        
    }
    class CompareChromosomes : IComparer<PopulationItem>
    {
        public int Compare(PopulationItem x, PopulationItem y)
        {
            return -1 * x.FitnessValue.CompareTo(y.FitnessValue); //descending sort
        }
    }
    public class Data
    {
        public string Item { get; set; }
        public double Cost { get; set; }
        public int Value { get; set; }
    }
}
