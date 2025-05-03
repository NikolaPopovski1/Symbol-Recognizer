namespace SymbolRecogniser.NeuralNetwork
{
    static class Parameters
    {
        public const int NUM_OF_LAYERS = 4; // input layer + hidden layers + output layer
        public const int FIRST_HIDDEN_LAYER_SIZE = 64; // zdi se mi, da je 11 zadnjih slojev najbolj optimalno (7 za številke + 4 za črke) 
        public const int SECOND_HIDDEN_LAYER_SIZE = FIRST_HIDDEN_LAYER_SIZE / 2; // why not?
        public const int NUM_OF_POINTS = 128; // reduction to num of points aka. vectors
        public const int INPUT_LAYER_SIZE = NUM_OF_POINTS * 2; // two times NUM_OF_POINTS because of x and y coordinates
        public const int MIN_OUTPUT_LAYER_COUNT = 3; // minimum number of different symbols to start learning
        public const int MAX_OUTPUT_LAYER_COUNT = 128; // minimum number of different symbols to start learning

        public const int MIN_RANDOM_WEIGHT = -100; // random weight range
        public const int MAX_RANDOM_WEIGHT = 100; // random weight range
        public const int MIN_RANDOM_BIAS = -100; // random bias range
        public const int MAX_RANDOM_BIAS = 100; // random bias range

        public const double LEARNING_RATE = 0.01;
        public const int EPOCHS = 10000;
        public const double MOMENTUM = 0.9;
        public const double REGULARIZATION_LAMBDA = 0.01;
    }
}
