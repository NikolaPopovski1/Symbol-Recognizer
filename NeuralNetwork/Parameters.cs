namespace SymbolRecogniser.NeuralNetwork
{
    static class Parameters
    {
        public static int INPUT_LAYER_SIZE = 784;
        public static int HIDDEN_LAYER_SIZE = 64; // zdi se mi, da je 11 zadnjih slojev najbolj optimalno (7 za številke + 4 za črke) 
        public static int NUM_OF_POINTS = 256; // reduction or increase to
        public static int OUTPUT_LAYER_SIZE = 10;
        public static double LEARNING_RATE = 0.01;
        public static int EPOCHS = 1000;
        public static double MOMENTUM = 0.9;
        public static double REGULARIZATION_LAMBDA = 0.01;
    }
}
