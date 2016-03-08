using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genetics;
using Matricies;

namespace NeuralNetworks
{
    public class Network
    {
        #region Properties

        /// <summary>
        /// Column matrix of values for inputs neurons
        /// </summary>
        public double[,] input { get; set; }
        /// <summary>
        /// Column matrix of values for hidden neurons
        /// </summary>
        public double[,] hidden { get; set; }
        /// <summary>
        /// Column matrix of values for output neurons
        /// </summary>
        public double[,] output { get; set; }

        /// <summary>
        /// Z values - before sigma - of hidden layer
        /// </summary>
        public double[,] zHid { get; set; }
        /// <summary>
        /// Z values - before sigma - of output layer
        /// </summary>
        public double[,] zOut { get; set; }

        /// <summary>
        /// Matrix of Weights from input to hidden layer.
        /// Format: [Hidden layer neuron, Input layer neuron]
        /// </summary>
        public double[,] wHid { get; set; } //[Hidden, Input]
        /// <summary>
        /// Matrix of Weights from hidden to output layer. 
        /// Format: [Output layer neuron, Hidden layer neuron]
        /// </summary>
        public double[,] wOut { get; set; } //[Output, Hidden]

        /// <summary>
        /// Column matrix of biases for hidden layer
        /// </summary>
        public double[,] bHid { get; set; } //[Hidden, Input]
        /// <summary>
        /// Column matrix of biases for the output layer
        /// </summary>
        public double[,] bOut { get; set; } //[Output, Hidden]

        /// <summary>
        /// Derivative of input to hidden weights
        /// </summary>
        public double[,] DwHid { get; set; }
        /// <summary>
        /// Derivative of hidden to output weights
        /// </summary>
        public double[,] DwOut { get; set; }

        /// <summary>
        /// Derivative of the biases in the hidden layer
        /// </summary>
        public double[,] DbHid { get; set; }
        /// <summary>
        /// Derivative of the biases in the output layer
        /// </summary>
        public double[,] DbOut { get; set; }

        #endregion

        /// <summary>
        /// Initializes a network with a specified number of neurons for each layer
        /// with random weights and biases
        /// </summary>
        /// <param name="numInp">Number of input neurons</param>
        /// <param name="numHid">Number of hidden neurons</param>
        /// <param name="numOut">Number of output neurons</param>
        public Network(int numInp, int numHid, int numOut)
        {
            #region Initializers
            // Set the number of neurons at each level to what the user specifies
            input = new double[numInp, 1];
            hidden = new double[numHid, 1];
            output = new double[numOut, 1];

            //Create z values for the hidden and output layer
            zHid = new double[numHid, 1];
            zOut = new double[numOut, 1];

            //Create weights between every nueron
            wHid = new double[numHid, numInp];
            wOut = new double[numOut, numHid];

            //Create derivatives for each weight
            DwHid = new double[numHid, numInp];
            DwOut = new double[numOut, numHid];

            //Create biases for the hidden and output layer
            bHid = new double[numHid, 1];
            bOut = new double[numOut, 1];

            //Create derivatives for biases in the hidden and output layer
            DbHid = new double[numHid, 1]; //Hidden layer biases derivatives
            DbOut = new double[numOut, 1]; //Output layer biases derivatives
            #endregion

            #region Random Initializers
            Random rnd = new Random();

            //Four loops below set all weights and biases to a random number between -2.5 and 2.5
            for (int i = 0; i < numHid; i++)
            {
                for (int j = 0; j < numInp; j++)
                {
                    wHid[i, j] = Convert.ToDouble(rnd.Next(-2500, 2500)) / 1000;
                }
            }

            for (int i = 0; i < numOut; i++)
            {
                for (int j = 0; j < numHid; j++)
                {
                    wOut[i, j] = Convert.ToDouble(rnd.Next(-2500, 2500)) / 1000;
                }
            }

            for (int i = 0; i < numHid; i++)
            {
                bHid[i, 0] = Convert.ToDouble(rnd.Next(-2500, 2500)) / 1000;
            }

            for (int i = 0; i < numOut; i++)
            {
                bOut[i, 0] = Convert.ToDouble(rnd.Next(-2500, 2500)) / 1000;
            }
            #endregion
        }

        /// <summary>
        /// Initializes a network with a specified number of neurons for each layer
        /// with random weights and biases
        /// </summary>
        /// <param name="numInp">Number of input neurons</param>
        /// <param name="numHid">Number of hidden neurons</param>
        /// <param name="numOut">Number of output neurons</param>
        public Network(int numInp, int numHid, int numOut, Genome chromosome)
        {
            #region Initializers
            // Set the number of neurons at each level to what the user specifies
            input = new double[numInp, 1];
            hidden = new double[numHid, 1];
            output = new double[numOut, 1];

            //Create biases for the hidden and output layer
            zHid = new double[numHid, 1];
            zOut = new double[numOut, 1];

            //Create weights between every nueron
            wHid = new double[numHid, numInp];
            wOut = new double[numOut, numHid];

            //Create biases for the hidden and output layer
            bHid = new double[numHid, 1];
            bOut = new double[numOut, 1];
            #endregion

            //Transcribing the genome into the coefficients for the neural net
            #region Transcription

            for (int i = 0; i < bHid.Length; i++)
            {
                bHid[i, 0] = Convert.ToDouble(Convert.ToInt16(chromosome.genes.Substring(i * 16, 16), 2)) / 32768 / 4;
            }

            for (int i = 0; i < bOut.Length; i++)
            {
                bOut[i, 0] = Convert.ToDouble(Convert.ToInt16(chromosome.genes.Substring(i * 16 + 16 * (bHid.Length + 1), 16), 2)) / 32768 / 4;
            }

            for (int i = 0; i < wHid.GetLength(0); i++)
            {
                for (int j = 0; j < wHid.GetLength(1); j++)
                {
                    wHid[i, j] = Convert.ToDouble(Convert.ToInt16(
                        chromosome.genes.Substring((i * wHid.GetLength(1) + j) * 16 + 16 * (bHid.Length + bOut.Length + 1), 16)
                        , 2)) / 32768 / 4;
                }
            }

            for (int i = 0; i < wOut.GetLength(0); i++)
            {
                for (int j = 0; j < wOut.GetLength(1); j++)
                {
                    wOut[i, j] = Convert.ToDouble(Convert.ToInt16(
                        chromosome.genes.Substring((i * wOut.GetLength(1) + j) * 16 + 16 * (bHid.Length + bOut.Length + wHid.GetLength(0) * wHid.GetLength(1) + 1), 16)
                        , 2)) / 32768 / 4;
                }
            }

            #endregion
        }

        /// <summary>
        /// Runs calculations for the network given an input. Fills the hidden layer and output layer.
        /// </summary>
        /// <param name="input">Array of inputs to be put into the input layer</param>
        public void feedforward(double[,] input)
        {
            MatrixOp ops = new MatrixOp();

            #region Number Crunching
            //This loop cycles through each hidden neuron and sums up the input activations going to it
            //multiplied by a weight and then added to a bias -> Sigma (w*a + b)
            for (int cntrHid = 0; cntrHid < this.hidden.Length; cntrHid++) //For each hidden neuron
            {
                double sum = new double(); //Sum of input weights and activations, plus bias

                for (int cntrInp = 0; cntrInp < this.input.Length; cntrInp++) //For each input neuron
                {
                    sum += input[cntrInp, 0] * wHid[cntrHid, cntrInp]; //Add the weights * activations
                }

                zHid[cntrHid, 0] = sum; //Put sum in to Z values for each hidden neuron

                sum += bHid[cntrHid, 0]; //Add the bias of the neuron

                hidden[cntrHid, 0] = 1 / (1 + Math.Pow(2.718281828, -sum)); //Apply the sigma function and put into the hidden neuron layer
            }



            //This loop is the same as the one above, but cycles through the outputs and takes the sum of
            //the hidden neuron activations
            for (int cntrOut = 0; cntrOut < this.output.Length; cntrOut++) //For each output neuron
            {
                double sum = new double();

                for (int cntrHid = 0; cntrHid < this.hidden.Length; cntrHid++)// For each hidden neuron
                {
                    sum += hidden[cntrHid, 0] * wOut[cntrOut, cntrHid]; //Add the weights * activations
                }

                zOut[cntrOut, 0] = sum; //Put sum in to Z values for each output neuron

                sum += bOut[cntrOut, 0]; //Add the bias of the neuron

                output[cntrOut, 0] = 1 / (1 + Math.Pow(2.718281828, -sum)); //Apply the sigma function
            }
            #endregion
        }

        /// <summary>
        /// Runs backpropogation for the network after feedforward has been applied and calculates the derivatives for this network
        /// </summary>
        /// <param name="correct">The correct or desired output</param>
        public void backpropogate(double[,] correct)
        {
            MatrixOp ops = new MatrixOp();
            double[,] deltaO = new double[output.GetLength(0), 1]; //Create matrix of delta 3 values
            double[,] deltaH = new double[hidden.GetLength(0), 1]; //Create matrix of delta 2 values

            deltaO = ops.hadamard(ops.derivSigma(zOut), ops.subtract(output, correct)); //Compute delta 3 values
            deltaH = ops.hadamard(ops.multiply(ops.transpose(wOut), deltaO), ops.derivSigma(zHid)); //Compute delta 2 values

            DbOut = deltaO; //Compute bias derivatives for output layer
            DbHid = deltaH; //Compute bias derivatives for hidden layer

            DwOut = ops.multiply(deltaO, ops.transpose(hidden)); //Compute weight derivatives from hidden to output layer
            DwHid = ops.multiply(deltaH, ops.transpose(input)); //Compute weight derivatives from input to hidden layer
        }

        public void update(double[,] delBOut, double[,] delBHid, double[,] delWOut, double[,] delWHid)
        {
            for (int cntrOut = 0; cntrOut < output.GetLength(0); cntrOut++)
            {
                bOut[cntrOut, 1] += delBOut[cntrOut, 1];

                for (int cntrHid = 0; cntrHid < hidden.GetLength(0); cntrHid++)
                {
                    wOut[cntrOut, cntrHid] += delWOut[cntrOut, cntrHid];
                }
            }

            for (int cntrHid = 0; cntrHid < hidden.GetLength(0); cntrHid++)
            {
                bHid[cntrHid, 1] += delBHid[cntrHid, 1];

                for (int cntrInp = 0; cntrInp < input.GetLength(0); cntrInp++)
                {
                    wHid[cntrHid, cntrInp] += delWHid[cntrHid, cntrInp];
                }
            }
        }
    }
}
