namespace AudioLoopbackTest;

public static class Utils
{
    public static double[] Normalize(double[] values)
    {
        double min = values.Min();
        double max = values.Max();
        double range = max - min;

        if (range == 0)
        {
            return values.Select(v => 0.0).ToArray(); // Avoid division by zero
        }

        // Perform Min-Max Normalization
        double[] normalizedValues = values.Select(v => (v - min) / range).ToArray();

        return normalizedValues;
    }
    
    /// Maps the input number to a range of outputs.
    /// </summary>
    /// <param name="value">The input number to map.</param>
    /// <param name="inputMin">The minimum value of the input range.</param>
    /// <param name="inputMax">The maximum value of the input range.</param>
    /// <param name="outputMin">The minimum value of the output range.</param>
    /// <param name="outputMax">The maximum value of the output range.</param>
    /// <returns>The mapped value within the output range.</returns>
    public static double MapToRange(double value, double inputMin, double inputMax, double outputMin, double outputMax)
    {
        // Ensure input range is not zero to avoid division by zero
        double inputRange = inputMax - inputMin;
        if (inputRange == 0)
        {
            throw new ArgumentException("Input range cannot be zero.");
        }

        // Calculate the proportion of the input value within the input range
        double proportion = (value - inputMin) / inputRange;

        // Apply the proportion to the output range
        double outputRange = outputMax - outputMin;
        double mappedValue = outputMin + (proportion * outputRange);

        return mappedValue;
    }
}