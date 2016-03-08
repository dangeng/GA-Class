using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetics
{
    public class Genome
    {
        public string genes { get; set; }

        /// <summary>
        /// Create a new random Genome with a ceratin number of base pairs.
        /// </summary>
        /// <param name="numBases"></param>
        public Genome(int numBases)
        {
            #region Create Random Genes

            string strGenes = null;

            System.Security.Cryptography.RNGCryptoServiceProvider prov = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] rand = new byte[1];

            prov.GetBytes(rand);

            Random rnd = new Random(Convert.ToInt32(rand[0]));

            for (int i = 0; i < numBases; i++)
            {
                strGenes += rnd.Next(0, 2);
            }

            genes = strGenes;

            #endregion
        }

        /// <summary>
        /// Create a genome using an existing string of genes
        /// </summary>
        /// <param name="inputGenes"></param>
        public Genome(string inputGenes)
        {
            //Set genes in genome equal to input string
            genes = inputGenes;
        }

        /// <summary>
        /// Given a mate, creates genes (string) for an offspring after a double crossover.
        /// </summary>
        /// <param name="mate"></param>
        /// <param name="start">Start of crossover</param>
        /// <param name="length">Length of crossover</param>
        /// <returns></returns>
        public string dblCross(Genome mate, int start, int length)
        {
            string offspring = null;

            offspring += this.genes.Substring(0, start - 1);
            offspring += mate.genes.Substring(start - 1, length);
            offspring += this.genes.Substring(start + length - 1, genes.Length - start - length + 1);

            return offspring;
        }

        /// <summary>
        /// Given a mate, returns genes (string) of an offspring after a single crossover
        /// </summary>
        /// <param name="mate"></param>
        /// <param name="start">Start of crossover</param>
        /// <returns></returns>
        public string sngCross(Genome mate, int start)
        {
            string offspring = null;

            offspring += this.genes.Substring(0, start - 1);
            offspring += mate.genes.Substring(start - 1, genes.Length - start + 1);

            return offspring;
        }

        /// <summary>
        /// Returns the mutated genes (string) of this Genome's genes
        /// </summary>
        /// <param name="rate">Mutation rate, between 0 and 1</param>
        /// <returns></returns>
        public string mutate(double rate)
        {
            System.Security.Cryptography.RNGCryptoServiceProvider prov = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] rand = new byte[1];

            prov.GetBytes(rand);

            Random rnd = new Random(Convert.ToInt32(rand[0]));

            StringBuilder offspring = new StringBuilder(genes);

            for (int i = 0; i < genes.Length; i++)
            {
                if (rnd.Next(0, 10001) <= rate * 10000)
                {
                    if (genes.ElementAt(i) == 49) //Char 49 = '1'
                    {
                        offspring.Insert(i, "0");
                    }
                    else
                    {
                        offspring.Insert(i, "1");
                    }

                    offspring.Remove(i + 1, 1);
                }
            }

            return offspring.ToString();
        }

        /// <summary>
        /// Creates an offspring between another mate
        /// </summary>
        /// <param name="mate"></param>
        /// <param name="dblRate">Rate of double crossover, between 0 and 1 (rate of single crossover = 1 - dblRate)</param>
        /// <param name="mutRate">Rate of mutations, between 0 and 1</param>
        /// <returns></returns>
        public Genome offspring(Genome mate, double dblRate, double mutRate)
        {
            Random rnd = new Random();

            string childsGenes = "";
            childsGenes = genes;


            if (rnd.Next(1, 10001) < dblRate * 10000)
            {
                int start = rnd.Next(1, genes.Length);
                childsGenes = dblCross(mate, start, rnd.Next(1, genes.Length - start));
            }
            else
            {
                childsGenes = sngCross(mate, rnd.Next(1, genes.Length));
            }


            Genome child = new Genome(childsGenes);

            child.genes = child.mutate(mutRate);

            return child;
        }
    }
}
